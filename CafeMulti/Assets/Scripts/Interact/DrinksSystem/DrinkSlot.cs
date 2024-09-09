using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DrinkSlot : NetworkBehaviour
{
    public string _name;
    public GameObject Cup;
    
    public Transform ParentTransform;
    
    [Header("Tür")]
    [SyncVar] public bool isCola, isPanta, isSoda;

    public void AddCup(GameObject handObj)
    {
        Cup = handObj;
    }
}
