using DG.Tweening;
using Mirror;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;

public class ItemInteract : NetworkBehaviour
{
    [SyncVar] public bool isReadyProduct;

    public ItemName _ItemName;
    [Header("Item")] 
    [SyncVar] public string itemName;
    [SyncVar] public bool isStateFree;
    [SyncVar] public bool isInteract;

    public Rigidbody rb;
    public BoxCollider collision;

    [Header("El Tutacak Yer")] 
    public Transform RightHandPos;

    public Transform RightPosObj;

    [Header("Child Obj")] public GameObject ChildObj;
    
    void Start()
    {
        if (isServer)
        {
            ServerStateChange();
            ServerName();
        }
    }

    [Server]
    private void ServerName()
    {
        _ItemName.ServerItemName(itemName);
    }

    void LateUpdate()
    {
        if (isServer)
        {
            ServerBoxParentUpdate();
        }
    }

    [Server]
    public void ServerBoxParentUpdate()
    {
        RpcBoxParentUpdate();
    }

    [ClientRpc]
    private void RpcBoxParentUpdate()
    {
        if (ChildObj != null && isInteract)
        {
            this.transform.position = ChildObj.transform.position;
            if (RightPosObj != null)
            {
                RightPosObj.position = RightHandPos.position;
            }
        }
    }
    
    // FONKSİYONLAR STATE ////////////////
    [Server]
    private void ServerStateChange()
    {
        RpcStateChange();
    }

    [ClientRpc]
    public void RpcStateChange()
    {
        if (rb != null && collision != null)
        {
            if (!isStateFree)
            {
                rb.isKinematic = true;
                collision.enabled = false;
            }
            else
            {
                rb.isKinematic = false;
                collision.enabled = true;
            }
        }
    }

    public void ItemNotParentObj(Vector3 offset)
    {
        isInteract = false;
        this.transform.parent = null;
        this.transform.DOMove(offset, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => PutItem());
    }

    private void PutItem()
    {
        ServerStateChange(true);
    }

    [Server]
    public void ServerStateChange(bool value)
    {
        isStateFree = value;
        ServerStateChange();
    }


    // Hand system
    public void ItemParentObj(Transform interactPosRot)
    {
        ChildObj = interactPosRot.gameObject;
        this.transform.SetParent(interactPosRot);
        this.transform.localPosition = Vector3.zero; 
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);
        isInteract = true;
    }

    public void HandSystem(Transform RightPos)
    {
        RightPosObj = RightPos;
        RightPos.position = RightHandPos.position;
        RightPos.rotation = Quaternion.Euler(-77, -42, -94);
    }
    
    public void Trash(Transform trashPos)
    {   
        this.transform.SetParent(null);
        isInteract = false;
        Vector3 newPos = new Vector3(trashPos.position.x, trashPos.position.y + 1.5f, trashPos.position.z);
        this.transform.DOScale(Vector3.zero, 0.8f);
        this.transform.DOMove(newPos, 0.5f)
            .OnComplete(() => onComplateTrash());
    }

    void onComplateTrash()
    {
        Destroy(this);
    }
    
}