using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ItemInfoCanvas : NetworkBehaviour
{
    [Header("UI")] public UIDocument _document;

    private VisualElement Bg;
    private VisualElement Bg2;
    private Label _ıtemName;
    private VisualElement BreadVisuel;
    private VisualElement RawMeatVisuel;
    private VisualElement BakedMeatVisuel;
    private VisualElement TomatoVisual;
    private VisualElement LettuceVisual;
    private VisualElement CheeseVisual;
    private VisualElement CupVisual;
    private VisualElement ColaVisual;
    private VisualElement PantaVisual;
    private VisualElement SodaVisual;
    private VisualElement PatatoVisual;
    private VisualElement PatatoPacketVisual;

    [Header("Ray")] [SerializeField] [Min(1)]
    private float hitRange = 1;

    int pickupLayerMask;
    int pickupLayerMask1;

    private GameObject lastObj;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        CmdItemAdd();
        pickupLayerMask = LayerMask.GetMask("Item");
        pickupLayerMask1 = LayerMask.GetMask("Tray");
    }

    [Command]
    private void CmdItemAdd()
    {
        RpcItemAdd();
    }

    [ClientRpc]
    private void RpcItemAdd()
    {
        GameObject CanvasObj = GameObject.FindGameObjectWithTag("ItemInfoCanvas");
        _document = CanvasObj.GetComponent<UIDocument>();
        Bg = _document.rootVisualElement.Q<VisualElement>("Bg");
        Bg2 = _document.rootVisualElement.Q<VisualElement>("Bg2");

        BreadVisuel = _document.rootVisualElement.Q<VisualElement>("Bread");
        RawMeatVisuel = _document.rootVisualElement.Q<VisualElement>("RawMeat");
        BakedMeatVisuel = _document.rootVisualElement.Q<VisualElement>("BakedMeat");
        TomatoVisual = _document.rootVisualElement.Q<VisualElement>("Tomato");
        LettuceVisual = _document.rootVisualElement.Q<VisualElement>("Lettuce");
        CheeseVisual = _document.rootVisualElement.Q<VisualElement>("Cheese");        
        CupVisual = _document.rootVisualElement.Q<VisualElement>("Cup");
        ColaVisual = _document.rootVisualElement.Q<VisualElement>("Cola");
        PantaVisual = _document.rootVisualElement.Q<VisualElement>("Panta");
        SodaVisual = _document.rootVisualElement.Q<VisualElement>("Soda");
        PatatoVisual = _document.rootVisualElement.Q<VisualElement>("Patato");
        PatatoPacketVisual = _document.rootVisualElement.Q<VisualElement>("PatatoPacket");

        _ıtemName = _document.rootVisualElement.Q<Label>("ItemName");

        Bg.style.display = DisplayStyle.None;
        BreadVisuel.style.display = DisplayStyle.None;
        RawMeatVisuel.style.display = DisplayStyle.None;
        BakedMeatVisuel.style.display = DisplayStyle.None;
        TomatoVisual.style.display = DisplayStyle.None;
        LettuceVisual.style.display = DisplayStyle.None;
        CheeseVisual.style.display = DisplayStyle.None;        
        CupVisual.style.display = DisplayStyle.None;
        ColaVisual.style.display = DisplayStyle.None;
        PantaVisual.style.display = DisplayStyle.None;
        SodaVisual.style.display = DisplayStyle.None;
        PatatoVisual.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (!isLocalPlayer) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,
                     Camera.main.transform.TransformDirection(Vector3.forward), out hit, hitRange, pickupLayerMask1))
        {
            CmdItemInfoTray(hit.collider.gameObject);
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward),
                out hit, hitRange, pickupLayerMask))
        {
            CmdItemInfo(hit.collider.gameObject);
        }
        else
        {
            CmdItemInfoDelete();
        }
    }

    [Command]
    private void CmdItemInfo(GameObject obj)
    {
        RpcItemInfo(obj);
    }

    [ClientRpc]
    private void RpcItemInfo(GameObject obj)
    {
        if (_document == null) return;
        if (obj.GetComponent<ItemInteract>().itemName == "Hamburger Bread")
        {
            Bg.style.display = DisplayStyle.Flex;
            BreadVisuel.style.display = DisplayStyle.Flex;

            lastObj = obj;
            if (lastObj.GetComponent<CombineProduct>().Meat || lastObj.GetComponent<CombineProduct>().Tomato || lastObj.GetComponent<CombineProduct>().Lettuce || lastObj.GetComponent<CombineProduct>().Cheese)
            {
                for (int i = 0; i < lastObj.GetComponent<CombineProduct>().ItemDatas._foodData.Length; i++)
                {
                    switch (lastObj.GetComponent<CombineProduct>().ComplateItemName)
                    {
                        case "Hamburger - 1":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 2":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 3":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 4":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 5":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 6":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 7":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                        case "Hamburger - 8":
                            BreadVisuel.style.display = DisplayStyle.Flex;
                            BakedMeatVisuel.style.display = DisplayStyle.Flex;
                            TomatoVisual.style.display = DisplayStyle.Flex;
                            LettuceVisual.style.display = DisplayStyle.Flex;
                            CheeseVisual.style.display = DisplayStyle.Flex;
                            break;
                    }
                }

                _ıtemName.text = lastObj.GetComponent<ItemName>()._ItemName;
            }
            else
            {
                _ıtemName.text = obj.GetComponent<ItemName>()._ItemName;
            }
        }
        else
        {
            if (obj.GetComponent<ItemName>()._ItemName != null)
            {
                Bg.style.display = DisplayStyle.Flex;
                _ıtemName.text = obj.GetComponent<ItemName>()._ItemName;
                switch (obj.GetComponent<ItemName>()._ItemName)
                {
                    case "Raw Meat":
                        RawMeatVisuel.style.display = DisplayStyle.Flex;
                        break;
                    case "Baked Meat":
                        BakedMeatVisuel.style.display = DisplayStyle.Flex;
                        break;
                    case "Tomato":
                        TomatoVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Lettuce":
                        LettuceVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Cheese":
                        CheeseVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Cup":
                        CupVisual.style.display = DisplayStyle.Flex;
                        break;       
                    case "Cola":
                        ColaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Panta":
                        PantaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Soda":
                        SodaVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Patato":
                        PatatoVisual.style.display = DisplayStyle.Flex;
                        break;
                    case "Patato Packet":
                        PatatoPacketVisual.style.display = DisplayStyle.Flex;
                        break;
                }
            }
        }
    }

    [Command]
    private void CmdItemInfoTray(GameObject obj)
    {
        RpcItemInfoTray(obj);
    }

    [TargetRpc]
    private void RpcItemInfoTray(GameObject obj)
    {
        if (_document == null) return;

        Bg.style.display = DisplayStyle.Flex;

        lastObj = obj;

        if(lastObj.GetComponent<Tray>()._name == null)return;
        Bg.style.display = DisplayStyle.Flex;
        _ıtemName.text = lastObj.GetComponent<Tray>()._name;
        
    }

    [Command]
    private void CmdItemInfoDelete()
    {
        RpcItemInfoDelete();
    }

    [ClientRpc]
    private void RpcItemInfoDelete()
    {
        if (_document == null) return;
        Bg.style.display = DisplayStyle.None;
        _ıtemName.text = "";
        BreadVisuel.style.display = DisplayStyle.None;
        RawMeatVisuel.style.display = DisplayStyle.None;
        BakedMeatVisuel.style.display = DisplayStyle.None;
        TomatoVisual.style.display = DisplayStyle.None;
        LettuceVisual.style.display = DisplayStyle.None;
        CheeseVisual.style.display = DisplayStyle.None;   
        CupVisual.style.display = DisplayStyle.None;
        ColaVisual.style.display = DisplayStyle.None;
        PantaVisual.style.display = DisplayStyle.None;
        SodaVisual.style.display = DisplayStyle.None;
        PatatoVisual.style.display = DisplayStyle.None;
        PatatoPacketVisual.style.display = DisplayStyle.None;
    }
}