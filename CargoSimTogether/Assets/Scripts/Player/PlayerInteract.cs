using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Animations.Rigging;

public class PlayerInteract : NetworkBehaviour
{
    public Transform HitPointInteract;
    public Animator anims;

    public GameObject cam;

    // private RaycastHit hit;
    public Vector3 offset;
    public float yOffset = 10f;
    [SerializeField] [Min(1)] private float hitRange = 1;

    int pickupLayerMask;
    int pickupLayerMask2;
    int pickupLayerMask3;
    int pickupLayerMask4;
    int pickupLayerMask5;
    int pickupLayerMask6;
    int pickupLayerMask7;
    int pickupLayerMask8;
    int pickupLayerMask9;
    int pickupLayerMask10;
    int pickupLayerMask11;
    int pickupLayerMask12;
    int pickupLayerMask13;
    int pickupLayerMask14;
    int pickupLayerMask15;
    int pickupLayerMask16;
    int pickupLayerMask17;
    int pickupLayerMask18;
    int pickupLayerMask19;

    [SyncVar] public bool handFull = false;
    [SyncVar] public bool mouseActivity = false;
    public GameObject InteractObj;
    public GameObject HandObj;
    public Transform InteractPos;
    public Transform InteractPosBox; // Kutu
    public Transform InteractPosItem; // item
    public Transform InteractPosBrush; // brush

    [Header("El Sistemi")] [SyncVar] public bool isHandRightTrue;
    public RigBuilder rigBuilder;
    [SerializeField] Transform HandTargetRight, HandLookRight;
    [SerializeField] Transform HandTargetLeft, HandLookLeft;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    void Start()
    {
        if (!isLocalPlayer) return;
        LayerAdd();
        mouseActivity = false;
    }

    private void LayerAdd()
    {
        pickupLayerMask = LayerMask.GetMask("Laptop");
        pickupLayerMask2 = LayerMask.GetMask("BoxObj");
        pickupLayerMask3 = LayerMask.GetMask("ItemSlot");
        pickupLayerMask4 = LayerMask.GetMask("CashRegister");
        pickupLayerMask5 = LayerMask.GetMask("Counter");
        pickupLayerMask6 = LayerMask.GetMask("Grill");
        pickupLayerMask7 = LayerMask.GetMask("Item");
        pickupLayerMask9 = LayerMask.GetMask("Customer");
        pickupLayerMask10 = LayerMask.GetMask("Tray");
        pickupLayerMask11 = LayerMask.GetMask("TrashBin");
        pickupLayerMask12 = LayerMask.GetMask("GrillButton");
        pickupLayerMask13 = LayerMask.GetMask("DrinkMachine");
        pickupLayerMask14 = LayerMask.GetMask("DrinkButton");
        pickupLayerMask15 = LayerMask.GetMask("FryerSlot");
        pickupLayerMask16 = LayerMask.GetMask("FryerButton");
        pickupLayerMask17 = LayerMask.GetMask("Door");
        pickupLayerMask18 = LayerMask.GetMask("Pool");
        pickupLayerMask19 = LayerMask.GetMask("Brush");
    }

    public void ServerInteract() // E ile etkileşim
    {
        if (mouseActivity)
        {
            if (InteractObj != null)
            {
                mouseActivity = false;
                CmdInteract(InteractObj);
                InteractObj = null;
            }
        }
        else
        {
            RaycastHit hit;
            // Laptop
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                    hitRange, pickupLayerMask))
            {
                CmdServerInteract(hit.collider.gameObject);
            }
            // Kasa Etkileşimi
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask4))
            {
                CmdServerInteract(hit.collider.gameObject);
            }

            // box Objeler
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask2))
            {
                CmdBoxInteract(hit.collider.gameObject);
            }
            // direkt slottan item alma
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask3))
            {
                CmdSlotInteractSpawn(hit.collider.gameObject);
            }
            // Fritöz item patatese çevirir
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask15))
            {
                CmdFryerPacketPatato(hit.collider.gameObject);
            }
            // Fritöz Button indir kaldır
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask16))
            {
                CmdFryerButton(hit.collider.gameObject);
            }

            // Item
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask7))
            {
                CmdTakeItem(hit.collider.gameObject); // item ele alma
                CmdCombineItem(hit.collider.gameObject); // item combine
                //CmdItemPacketing(hit.collider.gameObject);
            }
            // Customer
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask9))
            {
                CmdTakeCustomer(hit.collider.gameObject); // Musteri Teslim
            }
            // Tepsi
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask10))
            {
                CmdTrayPutItem(hit.collider.gameObject); // tepsiye ürün yerleştir
            }
            // grill Button
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask12))
            {
                CmdGrillButton(hit.collider.gameObject); // 
            }
            // Drink put
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask13))
            {
                CmdDrinkPut(hit.collider.gameObject); // 
            }
            // Drink başlangıç
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask14))
            {
                CmdDrinkButton(hit.collider.gameObject); // 
            }

            // Çöpe boşalt
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask11))
            {
                CmdTrashSystem(hit.collider.gameObject); // Musteri Teslim
            }
            // Kapı
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask17))
            {
                CmdDoor(hit.collider.gameObject); // Musteri Teslim
            }
            // Boku temizle
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask18))
            {
                CmdPool(hit.collider.gameObject); // Musteri Teslim
            }
            // fırçayı al
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask19))
            {
                CmdBrush(hit.collider.gameObject); // Musteri Teslim
            }
        }
    }

    // boku temizle
    [Command]
    private void CmdBrush(GameObject obj)
    {
        if (obj != null)
        {
            RpcBrush(obj);
        }
    }

    [ClientRpc]
    private void RpcBrush(GameObject obj)
    {
        if (handFull && HandObj != null) return;
        HandObj = obj;
        if (HandObj.GetComponent<BrushInteract>())
        {
            HandObj.GetComponent<BrushInteract>().ServerStateChange(false);
            HandObj.GetComponent<BrushInteract>().BoxParentObj(InteractPosBrush);
            HandObj.GetComponent<BrushInteract>().HandSystem(HandTargetRight, HandTargetLeft);
            // isHandRightTrue = true;
            HandFullTest(true);
        }
    }

    // boku temizle
    [Command]
    private void CmdPool(GameObject obj)
    {
        if (obj != null)
        {
            RpcPool(obj);
        }
    }

    [ClientRpc]
    private void RpcPool(GameObject obj)
    {
        //if (!handFull && HandObj == null) return;
        if (HandObj.CompareTag("Brush"))
        {
            print("b");
            obj.GetComponent<Toilet>().PoolDisable();
        }
    }

    // kapı sistemi
    [Command]
    private void CmdDoor(GameObject obj)
    {
        if (obj != null)
        {
            RpcDoor(obj);
        }
    }

    [ClientRpc]
    private void RpcDoor(GameObject obj)
    {
        obj.GetComponent<Door>().ServerDoorOpen();
    }

    // Fryer objeyi pakete yerleştir
    [Command]
    private void CmdFryerButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcFryerButton(obj);
        }
    }

    [ClientRpc]
    private void RpcFryerButton(GameObject obj)
    {
        obj.GetComponent<FryerButton>().ServerStartChange();
    }

    // Fryer objeyi pakete yerleştir
    [Command]
    private void CmdFryerPacketPatato(GameObject obj)
    {
        if (obj != null)
        {
            if (!handFull) return;
            RpcFryerPacketPatato(obj);
        }
    }

    [ClientRpc]
    private void RpcFryerPacketPatato(GameObject obj)
    {
        if (!HandObj.CompareTag("Patato")) return;
        if (HandObj == null) return;
        if (!obj.GetComponent<DeepFryer>().Availability) return;
        if (obj.GetComponent<ItemBox>()._itemAmount < 1) return;
        HandObj.GetComponent<Patato>().ServerPatatoView();
        obj.GetComponent<DeepFryer>().avalibleChack();
        obj.GetComponent<ItemBox>().ServerAmountChange(1);
    }

    // Bardak Doldur
    [Command]
    private void CmdDrinkButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcDrinkButton(obj);
        }
    }

    [ClientRpc]
    private void RpcDrinkButton(GameObject obj)
    {
        if (obj == null) return;

        obj.GetComponent<DrinkButton>().DrinkStart();
    }

    // Bardak yerleştir
    [Command]
    private void CmdDrinkPut(GameObject obj)
    {
        if (obj != null)
        {
            RpcDrinkPut(obj);
        }
    }

    [ClientRpc]
    private void RpcDrinkPut(GameObject obj)
    {
        //if (!HandObj.CompareTag("Cup")) return;
        if (obj != null)
        {
            if (obj.GetComponent<DrinkSlot>().Cup == null)
            {
                HandObj.GetComponent<ItemInteract>().ItemParentObj(obj.GetComponent<DrinkSlot>().ParentTransform);
                // HandObj.GetComponent<Rigidbody>().isKinematic = true;
                // HandObj.GetComponent<ItemInteract>().collision.enabled = false;
                obj.GetComponent<DrinkSlot>().AddCup(HandObj);
                isHandRightTrue = false;
                InteractObj = null;
                HandObj = null;
                HandFullTest(false);
            }
            else
            {
                if (!obj.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>().isStartFull)
                {
                    CmdTakeItem(obj.GetComponent<DrinkSlot>().Cup);
                    obj.GetComponent<DrinkSlot>().Cup = null;
                }
            }
        }
    }

    // ocak butonu aktifleştir
    [Command]
    private void CmdGrillButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcGrillButton(obj);
        }
    }

    [ClientRpc]
    private void RpcGrillButton(GameObject obj)
    {
        if (obj == null) return;
        obj.GetComponent<GrillMachineButton>().GrillStartAndStop();
    }

    // tepsiye ürün yerleştir
    [Command]
    private void CmdTrayPutItem(GameObject obj)
    {
        if (obj != null)
        {
            if (handFull)
            {
                RpcTrayPutItem(obj);
            }
            else
            {
                RpcTrayInteract(obj);
            }
        }
    }

    [ClientRpc]
    private void RpcTrayPutItem(GameObject obj)
    {
        if (obj == null || HandObj == null) return;
        ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
        itemInteract.ServerStateChange(true);
        if (HandObj.GetComponent<ItemName>().isDrink)
        {
            obj.GetComponent<Tray>().ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 2, HandObj);
        }

        if (HandObj.GetComponent<ItemName>().isFood)
        {
            obj.GetComponent<Tray>().ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 1, HandObj);
        }

        if (HandObj.GetComponent<ItemName>().isSnack)
        {
            obj.GetComponent<Tray>().ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 3, HandObj);
        }

        isHandRightTrue = false;
        InteractObj = null;
        HandObj = null;
        HandFullTest(false);
    }

    [ClientRpc]
    private void RpcTrayInteract(GameObject obj)
    {
        if (obj == null || InteractObj != null) return;
        InteractObj = obj;
        InteractObj.GetComponent<TrayInteract>().BoxParentObj(InteractPosBox);
        InteractObj.GetComponent<TrayInteract>()
            .HandSystem(HandTargetRight, HandTargetLeft);
        InteractObj.GetComponent<TrayInteract>().CmdSetInteractBox();

        HandFullTest(true);
    }

    // slot etkileşimi
    [Command]
    private void CmdSlotInteractSpawn(GameObject obj)
    {
        if (obj != null)
        {
            if (handFull) return;
            GameObject spawnObj = Instantiate(obj.GetComponent<ItemBox>().Prefab);
            NetworkServer.Spawn(spawnObj);
            RpcSlotInteractSpawn(spawnObj, obj);
        }
    }

    [ClientRpc]
    private void RpcSlotInteractSpawn(GameObject spawnObj, GameObject slot)
    {
        if (spawnObj == null || InteractPosItem == null) return;
        if (spawnObj.CompareTag("Tray"))
        {
            if (spawnObj.GetComponent<TrayInteract>())
            {
                if (slot.GetComponent<ItemBox>()._itemAmount < 1) return;
                InteractObj = spawnObj;
                spawnObj.GetComponent<TrayInteract>().ServerStateChange(false);
                spawnObj.GetComponent<TrayInteract>().BoxParentObj(InteractPosBox);
                spawnObj.GetComponent<TrayInteract>().HandSystem(HandTargetRight, HandTargetLeft);
                slot.GetComponent<ItemBox>().ServerAmountChange(1);
                isHandRightTrue = true;
                HandFullTest(true);
            }
        }
        else
        {
            if (spawnObj.GetComponent<ItemInteract>())
            {
                if (slot.GetComponent<ItemBox>()._itemAmount < 1) return;
                HandObj = spawnObj;
                spawnObj.GetComponent<ItemInteract>().ServerStateChange(false);
                spawnObj.GetComponent<ItemInteract>().ItemParentObj(InteractPosItem);
                spawnObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight);
                slot.GetComponent<ItemBox>().ServerAmountChange(1);
                isHandRightTrue = true;
                HandFullTest(true);
            }
        }
    }

    // pc/cash Register etkileşimi 
    [Command]
    private void CmdServerInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcServerInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcServerInteract(GameObject obj)
    {
        if (handFull) return;
        InteractObj = obj;
        if (InteractObj.GetComponent<LaptopInteract>())
        {
            if (!InteractObj.GetComponent<LaptopInteract>().isInteractPC)
            {
                InteractObj.GetComponent<LaptopInteract>().SetCameraPosition();
                mouseActivity = true;
                CmdInteract(InteractObj);
            }
        }

        if (InteractObj.GetComponent<CashRegister>())
        {
            if (!InteractObj.GetComponent<CashRegister>().isInteractCR)
            {
                InteractObj.GetComponent<CashRegister>().SetPlayerPosition();
                mouseActivity = true;
                CmdInteract(InteractObj);
            }
        }
    }

    // kutu etkileşimi başlangıcı - ele alma eylemi.
    [Command]
    private void CmdBoxInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcBoxInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcBoxInteract(GameObject obj)
    {
        if (handFull) return;
        InteractObj = obj;
        if (InteractObj.GetComponent<ObjInteract>())
        {
            if (!InteractObj.GetComponent<ObjInteract>().isInteract)
            {
                InteractObj.GetComponent<ObjInteract>().BoxParentObj(InteractPosBox);
                InteractObj.GetComponent<ObjInteract>()
                    .HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
                InteractObj.GetComponent<ObjInteract>().CmdSetInteractBox();
                HandFullTest(true);
            }
        }
    }

    // Itemi masadan geri alma / E
    [Command]
    private void CmdTakeItem(GameObject obj)
    {
        if (obj != null)
        {
            RpcTakeItem(obj);
        }
    }

    [ClientRpc]
    private void RpcTakeItem(GameObject obj)
    {
        if (handFull) return;
        HandObj = obj;
        if (HandObj.GetComponent<ItemInteract>())
        {
            HandObj.GetComponent<ItemInteract>().ServerStateChange(false);
            HandObj.GetComponent<ItemInteract>().ItemParentObj(InteractPosItem);
            HandObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight);
            isHandRightTrue = true;
            HandFullTest(true);
        }
    }

    // item Combine yapma / E
    [Command]
    private void CmdCombineItem(GameObject obj)
    {
        if (obj != null)
        {
            RpcCombineItem(obj);
        }
    }

    [ClientRpc]
    private void RpcCombineItem(GameObject obj)
    {
        if (!obj.CompareTag("Burger")) return;
        if (HandObj.CompareTag("Meat") || HandObj.CompareTag("Tomato") || HandObj.CompareTag("Lettuce") ||
            HandObj.CompareTag("Cheese"))
        {
            if (!handFull && HandObj == null) return;
            obj.GetComponent<CombineProduct>()
                .Combine(HandObj.GetComponent<ItemName>()._ItemName, HandObj, this.gameObject);
        }
    }

    public void CombineInteract()
    {
        HandObj = null;
        InteractObj = null;
        isHandRightTrue = false;
        HandFullTest(false);
    }

    // item paketleme yapma / E
    [Command]
    private void CmdItemPacketing(GameObject obj)
    {
        if (obj != null)
        {
            RpcItemPacketing(obj);
        }
    }

    [ClientRpc]
    private void RpcItemPacketing(GameObject obj)
    {
        if (obj.CompareTag("Packet"))
        {
            if (!handFull && HandObj == null) return;
            if (HandObj.CompareTag("Item"))
            {
                ItemPacket Obj = obj.GetComponent<ItemPacket>();
                HandObj.GetComponent<ItemInteract>().isInteract = false;
                Obj.PossessionItem(HandObj);
                isHandRightTrue = false;
                HandFullTest(false);
                HandObj = null;
            }
        }
    }

    // Musteri Teslim / E
    [Command]
    private void CmdTakeCustomer(GameObject obj)
    {
        if (obj != null)
        {
            RpcTakeCustomer(obj);
        }
    }

    [ClientRpc]
    private void RpcTakeCustomer(GameObject obj)
    {
        if (!handFull || obj == null || InteractObj == null) return;
        Customer Obj = obj.GetComponent<Customer>();
        if (Obj.isStop && InteractObj.CompareTag("Tray"))
        {
            Obj.PlayerGiveProduct(InteractObj, gameObject);
        }
    }

    // Çöpe item boşatma / E
    [Command]
    private void CmdTrashSystem(GameObject obj)
    {
        if (obj != null)
        {
            RpcTrashSystem(obj);
        }
    }

    [ClientRpc]
    private void RpcTrashSystem(GameObject obj)
    {
        if (InteractObj != null)
        {
            if (InteractObj.CompareTag("Box"))
            {
                InteractObj.GetComponent<ObjInteract>().Trash(obj.transform);
                HandFullTest(false);
                InteractObj = null;
                HandObj = null;
            }
            else
            {
                InteractObj.GetComponent<Tray>().Trash();
            }

            // if (InteractObj.CompareTag("Tray"))
            // {
            //     InteractObj.GetComponent<Tray>().Trash();
            // }
        }

        if (HandObj != null)
        {
            if (HandObj.CompareTag("Item"))
            {
                HandObj.GetComponent<ItemInteract>().Trash(obj.transform);
                HandFullTest(false);
                InteractObj = null;
                HandObj = null;
            }
        }
        else
        {
            print("boş");
        }
    }

    // duraklatılma yapılınca burası ekranı bırakmayı yarar
    public void Interact() // input sistemi içerisinde
    {
        if (InteractObj != null)
        {
            if (InteractObj.GetComponent<LaptopInteract>())
            {
                if (InteractObj.GetComponent<LaptopInteract>().isInteractPC)
                {
                    CmdInteract(InteractObj);
                }
            }

            if (InteractObj.GetComponent<CashRegister>())
            {
                if (InteractObj.GetComponent<CashRegister>().isInteractCR)
                {
                    CmdInteract(InteractObj);
                }
            }
        }
    }

    [Command]
    private void CmdInteract(GameObject obj)
    {
        if (obj.GetComponent<CashRegister>())
        {
            obj.GetComponent<CashRegister>().CmdSetInteractCR();
        }

        if (obj.GetComponent<LaptopInteract>())
        {
            obj.GetComponent<LaptopInteract>().CmdSetInteractPC();
        }
    }

    // sol Click
    public void BoxInteractShelftoBoxHold() // input sisteminde
    {
        RaycastHit hit;
        // Grill
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask6))
        {
            CmdDropGrill(hit.point); // ızgaraya yere bırakma eylemi 
        }
        // Slota yerleştir
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask3))
        {
            CmdSlotInteract(hit.collider.gameObject);
        }
        // Tezgah * itemi ve bıçağı bırakma
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask5))
        {
            CmdDropItem(hit.point); // item tezgaha bırakma eylemi 
        }
        // fryer slota yerleştir
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask15))
        {
            CmdSlotInteract(hit.collider.gameObject);
        }
    }

    // kutudaki itemleri boşaltma
    [Command]
    private void CmdSlotInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcSlotInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcSlotInteract(GameObject obj)
    {
        if (InteractObj == null) return;
        if (InteractObj.CompareTag("Box"))
        {
            obj.GetComponent<ItemBox>().GiveItem(InteractObj.GetComponent<ObjInteract>()._objAmount, InteractObj);
            if (obj.CompareTag("Fryer"))
            {
                obj.GetComponent<DeepFryer>().Availability = false;
            }
        }
    }

    // item bırakma eylemi (tezgaha)
    [Command]
    private void CmdDropItem(Vector3 hitPoint)
    {
        RpcDropItem(hitPoint);
    }

    [ClientRpc]
    private void RpcDropItem(Vector3 hitPoint)
    {
        if (HandObj != null)
        {
            if (HandObj.CompareTag("Brush"))
            {
                if (HandObj.GetComponent<BrushInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    BrushInteract itemInteract = HandObj.GetComponent<BrushInteract>();
                    itemInteract.ItemNotParentObj(HitPointInteract.position);
                    itemInteract.ServerStateChange(true);
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
            else
            {
                if (HandObj.GetComponent<ItemInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
                    itemInteract.ItemNotParentObj(HitPointInteract.position);
                    itemInteract.ServerStateChange(true);
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
        }

        if (InteractObj != null)
        {
            if (InteractObj.CompareTag("Box"))
            {
                if (InteractObj.GetComponent<ObjInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    ObjInteract boxObj = InteractObj.GetComponent<ObjInteract>();
                    boxObj.ItemNotParentObj(HitPointInteract.position);
                    boxObj.ServerStateChange(false);
                    boxObj.CmdSetDropBox();
                    boxObj.BoxNotParentObj();
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
            else
            {
                if (InteractObj.GetComponent<TrayInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    TrayInteract boxObj = InteractObj.GetComponent<TrayInteract>();
                    boxObj.ItemNotParentObj(HitPointInteract.position);
                    boxObj.GetComponent<Rigidbody>().isKinematic = false;
                    //boxObj.ServerStateChange(false);
                    // boxObj.CmdSetDropBox();
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
        }
    }

    // item bırakma eylemi (ızgaraya)
    [Command]
    private void CmdDropGrill(Vector3 hitPoint)
    {
        RpcDropGrill(hitPoint);
    }

    [ClientRpc]
    private void RpcDropGrill(Vector3 hitPoint)
    {
        if (HandObj == null) return;
        if (HandObj.CompareTag("Meat"))
        {
            offset = hitPoint;
            HitPointInteract.position = offset + new Vector3(0, 0.1f, 0);
            ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
            itemInteract.ItemNotParentObj(HitPointInteract.position);
            itemInteract.ServerStateChange(true);
            HandObj.GetComponent<GrillProduct>().ServerPosChangeBool(true);
            InteractObj = null;
            HandObj = null;
            HandFullTest(false);
        }
    }


    // Kutu Droplama
    public void DropedInteract() // input sisteminde
    {
        CmdDrop(InteractObj);
    }

    // sade kutu ayarları sunucuya gönderimi (kutuyu yere bırakma)
    [Command]
    private void CmdDrop(GameObject obj)
    {
        RpcDrop(obj);
    }

    [ClientRpc]
    private void RpcDrop(GameObject obj)
    {
        if (obj == null) return;
        if (obj.GetComponent<ObjInteract>())
        {
            HandFullTest(false);
            obj.GetComponent<ObjInteract>().CmdSetDropBox();
            obj.GetComponent<ObjInteract>().BoxNotParentObj();
            InteractObj = null;
        }
    }

    [Command]
    public void HandFullTest(bool value)
    {
        handFull = value;
        RpcRigHandSystem(value);
    }

    [ClientRpc]
    private void RpcRigHandSystem(bool value)
    {
        if (value)
        {
            if (isHandRightTrue)
            {
                rigBuilder.layers[1].active = value;
                rigBuilder.layers[2].active = !value;
            }
            else
            {
                rigBuilder.layers[2].active = value;
                rigBuilder.layers[1].active = value;
            }
        }
        else
        {
            rigBuilder.layers[2].active = value;
            rigBuilder.layers[1].active = value;
        }

        //CmdAnimation();
    }

    [Command]
    private void MouseActivi()
    {
        if (DailyStatistics.instance.isDayPanel)
        {
            mouseActivity = true;
        }
    }
}