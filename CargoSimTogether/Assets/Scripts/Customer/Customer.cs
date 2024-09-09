using System;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[System.Serializable]
public class OrderItem
{
    public string itemName;
    public string MealName;
    public string DrinkName;
    public string SnackName;

    public OrderItem(string name, string mealName, string drinkName, string snackName)
    {
        itemName = name;
        MealName = mealName;
        DrinkName = drinkName;
        SnackName = snackName;
    }

    public override bool Equals(object obj)
    {
        if (obj is OrderItem)
        {
            var other = (OrderItem)obj;
            return this.itemName == other.itemName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}

public class Customer : NetworkBehaviour
{
    [Header("View")] [SyncVar] public int AppearanceValue;
    public List<GameObject> Appearance = new List<GameObject>();

    [Header("Orders")] [SyncVar] public bool WaitingForOrder;
    [SyncVar] public int OrderCount;
    public Datas RecipeData;
    public List<OrderItem> orderItems = new List<OrderItem>();

    [Header("Transforms")] public Transform OrderStayPos;
    public Transform Hand;

    [Header("Agent & Anim")] public Animator anims;
    [SyncVar] public bool isStop;
    [SerializeField] public NavMeshAgent CustomerAgent;
    [SyncVar] public bool isFinish;

    public Vector3 TargetPos;
    [SyncVar] public int customerQueue;
    [SyncVar] public bool isEat;
    [SyncVar] public bool isChair;
    [SyncVar] public bool isToilet;
    [SyncVar] public bool GoToToilet;
    public GameObject ChairSelected;
    public GameObject ToiletSelected;

    // oturup bekleme süresi
    [SyncVar] public float SitTime;
    [SyncVar] public float MaxSitTime;

    // tuvalet bekleme süresi
    [SyncVar] public float ToiletTime;
    [SyncVar] public float MaxToiletTime;

    // yeme süresi belirleme
    [SyncVar] public float eatTime;
    [SyncVar] public float MaxeatTime;

    [Header("Random Pos")] public float minX = -5;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("Wait Time")] [SyncVar] public float OrderWaitTimer;

    [SyncVar] public float OrderWaitTimeHappy = 0;
    [SyncVar] public float OrderWaitNormal = 6;
    [SyncVar] public float OrderWaitSad = 11;

    [SyncVar] public float OrderWaitMax;
    [SyncVar] public float OrderWaitMin;

    [Header("Data")] [SerializeField] public DailyStatistics dailyStatistics;

    [SyncVar] public int HappySystem;

    public TrayInteract _TrayInteract;
    public Tray _Tray;

    public BurgerScreen _BurgerScreen;
    public SnackScreen _SnackScreen;


    void Start()
    {
        dailyStatistics = DailyStatistics.instance;
        if (isServer)
        {
            ServerStart();
            ServerSelectGoPos();
            ServerGoToPos();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
        ServerAppiranceValue(); // görünüm ayarlama
        ServerSelectOrder(); // sipariş Seçimi
        //randomToilet(); // tuvalet durumu olacak mı
    }

    private void randomToilet()
    {
        if (CustomerManager.instance.Toilet.Count > 0)
        {
            int randomInt = Random.Range(5, 5);
            if (randomInt == 5)
            {
                isToilet = true;
                RandomToiletPosSelect();
            }
        }
        else
        {
            isToilet = false;
        }
    }

    [ClientRpc]
    private void RandomToiletPosSelect()
    {
        int randomToilet = Random.Range(0, CustomerManager.instance.Toilet.Count);
        ToiletSelected = CustomerManager.instance.Toilet[randomToilet];
        CustomerManager.instance.RemoveToilet(ToiletSelected);
    }

    [ClientRpc]
    private void RpcStart()
    {
        CustomerAgent = GetComponent<NavMeshAgent>();

        OrderStayPos = GameObject.FindWithTag("OrderStayPos").transform;

        GameObject BurgermonitorObj = GameObject.Find("BurgerMonitor");
        _BurgerScreen = BurgermonitorObj.GetComponent<BurgerScreen>();

        GameObject SnackmonitorObj = GameObject.Find("DrinkAndSnackMonitor");
        _SnackScreen = SnackmonitorObj.GetComponent<SnackScreen>();

        this.transform.SetParent(null);
    }

    // görünümü ata (Server)
    void ServerAppiranceValue()
    {
        AppearanceValue = Random.Range(0, Appearance.Count);
        RpcAppiranceValue(AppearanceValue);
    }

    [ClientRpc] //  (Rpc)
    void RpcAppiranceValue(int index)
    {
        Appearance[index].SetActive(true);
    }

    // Sipariş seç (Server)
    void ServerSelectOrder()
    {
        OrderCount = Random.Range(1, RecipeData._MenuDatas.Length + 1);
        for (int i = 0; i < 1; i++)
        {
            int OrderSelectName = Random.Range(0, RecipeData._MenuDatas.Length);
            //int OrderSelectNumber = Random.Range(1, 3);
            int OrderSelectNumber = 1;
            AddOrderItem(RecipeData._MenuDatas[OrderSelectName]._name, RecipeData._MenuDatas[OrderSelectName]._MealName,
                RecipeData._MenuDatas[OrderSelectName]._DrinkName, RecipeData._MenuDatas[OrderSelectName]._SnackName);
        }
    }

    [ClientRpc] // liste ekleme clientler için
    public void AddOrderItem(string itemName, string mealName, string drinkName, string snackName)
    {
        var newItem = new OrderItem(itemName, mealName, drinkName, snackName);

        orderItems.RemoveAll(item => item.Equals(newItem));

        orderItems.Add(newItem);
    }

    void Update()
    {
        if (isServer)
        {
            ServerGoToPos();
        }
    }

    [Server]
    private void ServerGoToPos()
    {
        if (TargetPos != null)
        {
            print(CustomerAgent.isStopped);
            float stoppingDistance = Vector3.Distance(TargetPos, transform.position);
            if (stoppingDistance <= 2f)
            {
                RpcTakeProduct(TargetPos);
                CustomerAgent.isStopped = true;
                anims.SetBool("isWalk", false);
                isStop = true;

                if (isChair)
                {
                    this.transform.position = ChairSelected.transform.position;
                    anims.SetBool("isSit", true);
                    WaitingForOrder = true;
                    //  yeme süresi belirleme
                    if (isEat)
                    {
                        if (eatTime > MaxeatTime)
                        {
                            anims.SetBool("isEating", false); // yeme animasyonu olacak
                            RandomPosSelectto();
                            _TrayInteract.ServerStateChange(false);
                            _TrayInteract.RpcChairAdd(ChairSelected);
                            _Tray._name = null;


                            if (ToiletTime > MaxToiletTime)
                            {
                                isChair = false;
                                isEat = false;
                                RandomPosSelectto();
                                ToiletSelected.GetComponent<Toilet>().PoolEnable();
                            }
                            else
                            {
                                ToiletTime += Time.deltaTime;
                            }
                            // if (isToilet && ToiletSelected != null)
                            // {
                            //     ToiletSelected.GetComponent<Toilet>()._Door.isDoorChange(false);
                            // }
                        }
                        else
                        {
                            eatTime += Time.deltaTime;
                            anims.SetBool("isEating", true); // yeme animasyonu olacak
                        }
                    }
                    else
                    {
                        //  bekleme süresi
                        if (SitTime > MaxSitTime)
                        {
                            RpcName();
                            RandomPosSelectto();

                            // if (isToilet && ToiletSelected != null)
                            // {
                            //     ToiletSelected.GetComponent<Toilet>()._Door.isDoorChange(false);
                            // }

                            if (ToiletTime > MaxToiletTime)
                            {
                                isChair = false;
                                isEat = false;
                                isToilet = false;
                                RandomPosSelectto();
                                ToiletSelected.GetComponent<Toilet>().PoolEnable();
                            }
                            else
                            {
                                ToiletTime += Time.deltaTime;
                            }
                        }
                        else
                        {
                            SitTime += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                WaitingForOrder = false;
                CustomerAgent.isStopped = false;
                CustomerAgent.SetDestination(TargetPos);
                anims.SetBool("isSit", false);
                anims.SetBool("isWalk", true);
                isStop = false;
                RpcTakeProduct(TargetPos);
                // if (isToilet&& ToiletSelected != null)
                // {
                //     if (stoppingDistance < 0.6f)
                //     {
                //         ToiletSelected.GetComponent<Door>().isDoorChange(true);
                //     }
                // }
            }

            // bitiş ve yok oluş
            if (isFinish)
            {
                if (stoppingDistance < 0.3f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    [ClientRpc]
    private void RpcName()
    {
        foreach (var names in orderItems)
        {
            _BurgerScreen.ListRemove(names.MealName);
            _SnackScreen.ListRemove(names.DrinkName);
            _SnackScreen.ListRemove(names.SnackName);
        }
    }

    [ClientRpc]
    private void RpcTakeProduct(Vector3 Pos)
    {
        //Quaternion targetRotation = Quaternion.LookRotation(Pos);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
        //transform.rotation = targetRotation;
    }

    [Server]
    private void ServerSelectGoPos() // start da çalışıyor
    {
        RpcSelectOrderStayGoPos();
    }

    [ClientRpc]
    public void RpcSelectOrderStayGoPos()
    {
        if (orderItems.Count > 0)
        {
            TargetPos = OrderStayPos.position + new Vector3(0, 0, -customerQueue);
        }
    }

    public void PlayerGiveProduct(GameObject PlayerHandObj, GameObject player) // sipariş teslimi
    {
        if (WaitingForOrder)
        {
            Tray packet = PlayerHandObj.GetComponent<Tray>();
            _TrayInteract = PlayerHandObj.GetComponent<TrayInteract>();
            _Tray = packet;
            foreach (var orderItem in orderItems)
            {
                if (packet._name == orderItem.itemName)
                {
                    // teslim sonrası player işlemi
                    player.GetComponent<PlayerInteract>().HandFullTest(false);
                    player.GetComponent<PlayerInteract>().isHandRightTrue = false;
                    player.GetComponent<PlayerInteract>().InteractObj = null;

                    // teslim sonrası player işlemi
                    PlayerHandObj.transform.SetParent(null);
                    PlayerHandObj.transform.localRotation = Quaternion.identity;
                    PlayerHandObj.GetComponent<TrayInteract>().isInteract = false;
                    PlayerHandObj.GetComponent<TrayInteract>().collider.enabled = false;
                    PlayerHandObj.GetComponent<Tray>().Trash();
                    PlayerHandObj.transform.DOMove(Hand.position, 0.6f)
                        .OnComplete(() => ComplitePlayerGiveProduct());
                }
            }
        }
    }

    private void ComplitePlayerGiveProduct()
    {
        _BurgerScreen.ListRemove(_Tray.inParentBurgerName);
        isEat = true;
    }

    // Cash System
    [Server]
    public void CashRegisterProduct(GameObject CashObj, ScrollView BuyCardItemList, Label TotalAmountText)
    {
        RegisterAddToCartManager Cash = CashObj.GetComponent<RegisterAddToCartManager>();

        if (Cash.isFood && Cash.isDrink && Cash.isSnack)
        {
            RegisterAddToCartManager.instance.buycardAmountChange(TotalAmountText);
            // RandomPosSelectto();

            Cash.ItemCardItemList.Clear();
            Cash.BuyCardItemList.Clear();
            Cash.BuyProductItemList.Clear();
            BuyCardItemList.Clear();
            Cash.isFood = false;
            Cash.isDrink = false;
            Cash.isSnack = false;

            switch (HappySystem)
            {
                case 3:
                    dailyStatistics.ServerCustomerSatisfactionCountAdd(0, 0, 1);
                    break;
                case 2:
                    dailyStatistics.ServerCustomerSatisfactionCountAdd(0, 1, 0);
                    break;
                case 1:
                    dailyStatistics.ServerCustomerSatisfactionCountAdd(1, 0, 0);
                    break;
            }


            CustomerManager.instance.OrderStayQueue.Remove(gameObject);
            CustomGoToChair();
            // sipariş onaylanınca kartların silinmesi
            CustomerManager.instance.ServerCashQueueMove();
        }
    }

    [ClientRpc]
    private void CustomGoToChair()
    {
        int randomChair = Random.Range(0, CustomerManager.instance.Chairs.Count);
        CustomerManager.instance.RemoveChairs(ChairSelected);

        ChairSelected = CustomerManager.instance.Chairs[randomChair];

        //ChairSelected = CustomerManager.instance.Chairs[randomChair];

        TargetPos = ChairSelected.transform.position;

        isChair = true;
    }

    [Server]
    private void RandomPosSelectto()
    {
        if (isToilet)
        {
            //TargetPos = ToiletSelected.transform.position;
            TargetPos = ChairSelected.transform.position;
            //CustomerManager.instance.AddToilet(ToiletSelected);
            //GoToToilet = true;
            isToilet = false;
            print("toilet");
        }
        else
        {
            CustomerAgent.isStopped = false;
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            TargetPos = new Vector3(randomX, TargetPos.y, randomZ);
            if (TargetPos != null)
            {
                CustomerAgent.SetDestination(TargetPos);
                isFinish = true;
            }

            print("random");
        }
    }

    [Server]
    public void ServerCustomerTime()
    {
        OrderWaitTimer += Time.deltaTime;
        if (OrderWaitTimer < OrderWaitMin)
        {
            HappySystem = 3;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer < OrderWaitMax)
        {
            HappySystem = 2;
        }
        else if (OrderWaitTimer > OrderWaitMin && OrderWaitTimer > OrderWaitMax)
        {
            HappySystem = 1;
        }
    }
}