using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DrinkButton : NetworkBehaviour
{   
    [SyncVar] public bool isStart;
    public string _name;
    public DrinkProduct _DrinkProduct;
    public DrinkSlot _DrinkSlot;

    private void Update()
    {
        if (isServer)
        {
            ServerSlotStart(); 
        }
    }

    [Server]
    private void ServerSlotStart()
    {
        RpcSlotStart();
    }
    [ClientRpc]
    private void RpcSlotStart()
    {
        if (isStart)
        {
            _DrinkProduct = _DrinkSlot.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>();
            _DrinkProduct.ServerFilling(gameObject);
            _DrinkProduct.GetComponent<ItemInteract>().rb.isKinematic = true;
            _DrinkProduct.GetComponent<ItemInteract>().collision.enabled = false;
        }
    }

    public void DrinkStart()
    {
        isStart = true;
    }
}
