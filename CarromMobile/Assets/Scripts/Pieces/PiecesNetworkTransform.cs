using Mirror;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PiecesNetworkTransform :  NetworkBehaviour
{
    [System.Serializable]
    public class PositionPackage
    {
        public float x;
        public float y;
        public float z;
    }

    private Vector3 currentPosition;
    private float networkSendRate = 0.1f;
    bool onceTrigger, executed, collisionWait;
    Rigidbody piece;

    private NetworkPacketManeger<PositionPackage> positionPacketManeger;

    private void OnEnable()
    {
        positionPacketManeger = new NetworkPacketManeger<PositionPackage>();
        Game2PManeger.EventAuthorityChange += AuthorityChanged;
        Game4Maneger.EventAuthorityChange += AuthorityChanged;
        positionPacketManeger.onRequirePackageTransmit += TransmitPositionPackagesToAll;
        DiskMove.OnHit += SetHit;
        
    }

    private void OnDestroy()
    {
        Game2PManeger.EventAuthorityChange -= AuthorityChanged;
        Game4Maneger.EventAuthorityChange -= AuthorityChanged;
        positionPacketManeger.onRequirePackageTransmit -= TransmitPositionPackagesToAll;
        DiskMove.OnHit -= SetHit;
      
    }
    void Start()
    {
        currentPosition = transform.position;
        positionPacketManeger.sendSpeed = networkSendRate;
        onceTrigger = false;
        executed = true;
        collisionWait = false;
        piece = GetComponent<Rigidbody>();
        piece.isKinematic = true;
    }
   
    public virtual void FixedUpdate()
    {
        SendPosition();
        if(executed)
            UpdatePosition();
        positionPacketManeger.Tick();
    }

    private void SendPosition()
    {
        if (hasAuthority)
        {
            if (transform.position != currentPosition)
            {
                currentPosition = transform.position;
                float timeStep = Time.time;
                positionPacketManeger.AddPackages(new PositionPackage
                {
                    x = transform.position.x,
                    y = transform.position.y,
                    z = transform.position.z
                });
            }
        }
    }

    private void UpdatePosition()
    {
        if (hasAuthority)
            return;
        var data = positionPacketManeger.GetNextDataReceived();

        if (data == null)
        {
            return;
        }
      
        Vector3 nextPosition = new Vector3(data.x, data.y, data.z);
        StartCoroutine(InetialLerp(nextPosition));
       

    }
    private IEnumerator InetialLerp(Vector3 position)
    {
        executed = false;
        while (!onceTrigger)
        {
            yield return null;
        }
        transform.position = position;
        executed = true;
    }
   
    private void SetHit()
    {
        piece.isKinematic = false;
    }

  
    private void TransmitPositionPackagesToAll(byte[] physics)
    {
        CmdTransmitPositionPackagesToAll(physics);
    }

    [Command]
    private void CmdTransmitPositionPackagesToAll(byte[] position)
    {
        RpcReceivePositionOnClient(position);
    }

    [ClientRpc]
    void RpcReceivePositionOnClient(byte[] data)
    {
        positionPacketManeger.ReceiveData(data);
    }

   
    private void AuthorityChanged()
    {
       // if (piece != null)
            ClearQueue();
    }

    [Server]
    private void ClearQueue()
    {
        RpcClearQueue();
      
    }

    [ClientRpc]
    private void RpcClearQueue()
    {
        collisionWait = false;
        Invoke("CollisionWait", 1f);
        positionPacketManeger.ClearQueue();
        onceTrigger = false;
        if (piece == null)
            piece = GetComponent<Rigidbody>();
        if(!piece.isKinematic)
            piece.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasAuthority)
        {
            if (collision.collider.CompareTag("Pieces") && collisionWait)
            {
                AudioManeger.audioManegerInstance.Play("PiecePiece", 1);
            }
        }
        else
        {
            if (collision.collider.CompareTag("Pieces") && collisionWait)
            {
                AudioManeger.audioManegerInstance.Play("PiecePiece", 1);
               
            }
            if (collision.collider.CompareTag("Disk") && !onceTrigger)
            {
                if (collisionWait)
                {
                    onceTrigger = true;
                    collisionWait = false;
                }

            }
            else if (collision.collider.CompareTag("Pieces") && !onceTrigger)
            {
                if (collisionWait)
                {
                    onceTrigger = true;
                    collisionWait = false;
                }
            }
        }
       /* if (collision.collider.CompareTag("Disk") && !onceTrigger)
        {
            if (!hasAuthority && collisionWait)
            {
                onceTrigger = true;
                collisionWait = false;
            }

        }
        else if (collision.collider.CompareTag("Pieces") && !onceTrigger)
        {
            if (!hasAuthority && collisionWait)
            {
                onceTrigger = true;
                collisionWait = false;
            }
        }*/


    }

    private void CollisionWait()
    {
        collisionWait = true;

    }
}
