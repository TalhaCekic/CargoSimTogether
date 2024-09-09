using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // TextMeshPro sınıfları için gerekli

public class UIOrder : NetworkBehaviour
{
    [Header("Ürün bilgisi")]
    [SyncVar] public string OrderImageName;
    public Image OrderImage;
    public int value;
    [Header("Ürün Adedi")] [SyncVar] public int Number;
    public TMP_Text NumberText;

    public Datas RecipeOrderDataList;
    
    void Start()
    {
        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
    }

    [ClientRpc]
    private void RpcStart()
    {
        NumberText.text = Number.ToString();
        AddValues();
    }

    public void AddValues()
    {
        for (int i = 0; i < RecipeOrderDataList._MenuDatas.Length; i++)
        {
            if (OrderImage != null) // Null kontrolü
            {
                switch (value)
                {
                    case 1:
                        for (int a = 0; a < RecipeOrderDataList._foodData.Length; a++)
                        {
                            if (OrderImageName == RecipeOrderDataList._foodData[a]._name)
                            {
                                OrderImage.sprite = RecipeOrderDataList._foodData[a]._sprite; // Sprite atanıyor
                            }
                        }

                        break;
                    case 2:
                        for (int a = 0; a < RecipeOrderDataList._drinkData.Length; a++)
                        {
                            if (OrderImageName == RecipeOrderDataList._drinkData[a]._name)
                            {
                                OrderImage.sprite = RecipeOrderDataList._drinkData[a]._sprite; // Sprite atanıyor
                            }
                        }

                        break;
                    case 3:
                        for (int a = 0; a < RecipeOrderDataList._snackData.Length; a++)
                        {
                            if (OrderImageName == RecipeOrderDataList._snackData[a]._name)
                            {
                                OrderImage.sprite = RecipeOrderDataList._snackData[a]._sprite; // Sprite atanıyor
                            }
                        }

                        break;
                }
            }
            else
            {
                Debug.LogError("OrderImage bileşeni atanmamış!");
            }
        }
    }
}