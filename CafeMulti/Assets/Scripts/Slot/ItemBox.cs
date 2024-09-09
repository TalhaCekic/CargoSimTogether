using System;
using Mirror;
using TMPro;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{
    public string _itemName;
    [SyncVar] public int _maxItemAmount;
    [SyncVar] public int _itemAmount;

    public GameObject Prefab;

    [Header("UI")]
    public TMP_Text Amount;
    
    private void Start()
    {
        ServerStart();
    }

    [Server]
    private void ServerStart()
    {
        RpcStart();
    }
    [ClientRpc]
    private void RpcStart()
    {
        Amount.text = _itemAmount.ToString();
    }

    public void GiveItem(int _amount, GameObject box)
    {
        if (box.GetComponent<ObjInteract>().itemName == _itemName)
        {
            int newAmount = _maxItemAmount - _itemAmount;
            if (newAmount > _amount)
            {
                box.GetComponent<ObjInteract>().GiveBox(_amount);
                _itemAmount += _amount;
            }
            else
            {
                box.GetComponent<ObjInteract>().GiveBox(newAmount);
                _itemAmount += newAmount;
            }
            Amount.text = _itemAmount.ToString();
        }
    }
    public void ServerAmountChange(int value)
    {
        _itemAmount -= value;
        Amount.text = _itemAmount.ToString();
    }
}