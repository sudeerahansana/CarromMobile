using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DiskNetworkTransform : NetworkBehaviour
{
    [System.Serializable]
    public class PositionPackage
    {
        public float x;
        public float y;
        public float z;
    }
    private Vector3 currentPosition;
    float networkSendRate = 0.1f;
    private bool onceTrigger,onceHit,executed;
    public static event Action EventHitEnd;
    public static event Action EventHitEndOnReda;
    public static event Action DiskFall;
    private NetworkPacketManeger<PositionPackage> positionPacketManeger;

    private void OnEnable()
    {
        positionPacketManeger = new NetworkPacketManeger<PositionPackage>();
        positionPacketManeger.onRequirePackageTransmit += TransmitPositionPackagesToAll;
        DiskMove.OnHit += SetHit;
    }
    private void OnDestroy()
    {
        positionPacketManeger.onRequirePackageTransmit -= TransmitPositionPackagesToAll;
        DiskMove.OnHit -= SetHit;

    }
    void Start()
    {
        currentPosition = transform.position;
        positionPacketManeger.sendSpeed = networkSendRate;
        onceTrigger = true;
        onceHit = false;
        executed = true;
    }

    public virtual void FixedUpdate()
    {
        if (onceHit)
        {
            SendPosition();
            if(onceTrigger && NetworkServer.active)
            {
                onceTrigger = false;
                Invoke("DiskHitEnd", 6f);
            }
        }
        if (executed)
        {
            UpdatePosition();

        }
        positionPacketManeger.Tick();
    }

   
    private void SendPosition()
    {
        if (hasAuthority)
        {
            if (transform.position != currentPosition)
            {

                float timeStep = Time.time;
                currentPosition = transform.position;

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
        if(onceTrigger)
        {
            onceTrigger = false;
            executed = false;
            Physics.defaultContactOffset = 0.01f;
            StartCoroutine(InetialLerp(nextPosition));
        }
        else
            transform.position = nextPosition;

    }
    private IEnumerator InetialLerp(Vector3 position)
    {
        yield return new WaitForSeconds(1f);
        transform.position = position;
        if(NetworkServer.active)
            Invoke("DiskHitEnd", 5f);
        executed = true;

    }

    private void DiskHitEnd()
    {
        if (transform.position.y < -0.53f)
        {
            
        }
        else if (transform.position.y < -0.03f)
        {
            Debug.Log("Disk fall");
            DiskFall?.Invoke();
        }
        EventHitEndOnReda?.Invoke();
        EventHitEnd?.Invoke();
        GameObject parentHand = transform.parent.gameObject;
        Destroy(parentHand);
    }
    private void SetHit()
    {
        onceHit = true;
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

   

  
}
