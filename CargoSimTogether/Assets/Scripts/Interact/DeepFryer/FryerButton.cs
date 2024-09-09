using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class FryerButton : NetworkBehaviour
{
    public ItemBox _ItemBox;
    public GameObject FryerObj;
    public DeepFryer _DeepFryer;

    public bool isStart;
    
    [SyncVar] public float isCookDelay;
    [SyncVar] public float isMaxCookDelay;

    public Transform Pos1;
    public Transform Pos2;

    void Update()
    {
        if (isServer)
        {
            ServerCookPatato();
        }
    }

    [Server]
    private void ServerCookPatato()
    {
        if (isStart)
        {
            if (isCookDelay > isMaxCookDelay)
            {
                ServerCook();
                _DeepFryer.Availability = true;
            }
            else
            {
                isCookDelay += Time.deltaTime;
                _DeepFryer.Availability = false;
            }
        }
    }
    [Server]
    private void ServerCook()
    {
        RpcCook();
    }
    [ClientRpc]
    private void RpcCook()
    {
        ServerObjPos();
        isCookDelay = 0;
        isStart = false;
    }
    public void ServerStartChange()
    {
        isStart = !isStart;
        ServerObjPos();
    } 
    [ClientRpc]
    private void ServerObjPos()
    {
        if (isStart)
        {
            //FryerObj.transform.position = new Vector3(FryerObj.transform.position.x, +1.3f, FryerObj.transform.position.z);
            FryerObj.transform.DOMove(Pos1.position,1);
        }
        else
        {
            FryerObj.transform.DOMove(Pos2.position,1);
            //FryerObj.transform.position = new Vector3(FryerObj.transform.position.x, +1.1f, FryerObj.transform.position.z);
        }
 
    }
}
