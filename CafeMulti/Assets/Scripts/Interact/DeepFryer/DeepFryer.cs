using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class DeepFryer : NetworkBehaviour
{
    public ItemBox _ItemBox;
    [SyncVar] public bool Availability;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void avalibleChack()
    {
        if (_ItemBox._itemAmount == 0)
        {
            Availability = false;
        }
    }
}
