using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class SnackScreen : NetworkBehaviour
{
    public UIDocument Doc;
    public VisualElement Cart;
    public VisualTreeAsset _template;
    public ScrollView CartList;

    void OnEnable()
    {
        Doc.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("Screen")))
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= Doc.panelSettings.targetTexture.width;
            pixelUV.y *= Doc.panelSettings.targetTexture.height;

            // var cursor = _document.rootVisualElement.Q<VisualElement>("Cursor");
            // if (cursor != null)
            // {
            //     cursor.style.left = pixelUV.x;
            //     cursor.style.top = pixelUV.y;
            // }
            return pixelUV;
        });
    }

    public void AddCart(string name)
    {
        CartList = Doc.rootVisualElement.Q<ScrollView>("SnackView");
        
        Cart = _template.CloneTree();

        Cart.Q<VisualElement>("Card"); // Sepetteki kartı bul
        Label CardName = Cart.Q<Label>("Name"); // isim  text bul
        VisualElement ColaImage = Cart.Q<VisualElement>("ColaImage"); // avatar bul
        VisualElement PantaImage = Cart.Q<VisualElement>("PantaImage"); // avatar bul
        VisualElement SodaImage = Cart.Q<VisualElement>("SodaImage"); // avatar bul
        VisualElement PotatoImage = Cart.Q<VisualElement>("PotatoImage"); // avatar bul

        Cart.name = name;
        CardName.text = name;

        
        if (Cart != null)
        {
            if (CartList != null)
            {
                CartList.Add(Cart);
            }
        }


        ColaImage.style.display = DisplayStyle.None;
        PantaImage.style.display = DisplayStyle.None;
        SodaImage.style.display = DisplayStyle.None;
        PotatoImage.style.display = DisplayStyle.None;

        if (name == "Cola")
        {
            ColaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Panta")
        {
            PantaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Soda")
        {
            SodaImage.style.display = DisplayStyle.Flex;
        }

        if (name == "Patato")
        {
            PotatoImage.style.display = DisplayStyle.Flex;
        }
    }
    public void ListRemove(string name)
    {
        for (int i = 0; i < CartList.childCount; i++)
        {
            if (CartList[i].name == name)
            {
                Destroy(CartList[i].visualTreeAssetSource); // İlk olarak GameObject'i yok et
                CartList.RemoveAt(i); // Sonra listedeki referansını kaldır
                print("remove");
                break;
            }
        }
    }
    public void ListClear()
    {
        CartList = Doc.rootVisualElement.Q<ScrollView>("SnackView");
        
        for (int i = 0; i < CartList.childCount; i++)
        {
            Destroy(CartList[i].visualTreeAssetSource);
            CartList.Clear(); 
        }
        
    }
}
