using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Patato : NetworkBehaviour
{
    public GameObject Patatos;
    public ItemName _ItemName;
    public ItemInteract _ItemInteract;
    
   // [Server]
   public void ServerPatatoView()
   {
       Patatos.SetActive(true);
       _ItemName.ServerItemName("Patato");
       _ItemInteract.isReadyProduct = true;
   }
    // [ClientRpc]
    // public void RpcPatatoView()
    // {
    //     
    // }
}
