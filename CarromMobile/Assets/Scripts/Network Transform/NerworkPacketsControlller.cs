using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NerworkPacketsControlller : NetworkBehaviour
{
   
    [System.Serializable]
    public class TransformPackage
    {
        public float posX;
        public float posY;
        public float posZ;
        public float rotY;
        public float TimeStep;
    }

    public NetworkPacketManeger<TransformPackage> m_TransformPacketManeger;
    public NetworkPacketManeger<TransformPackage> transformPacketManeger
    {
        get
        {
            if (m_TransformPacketManeger == null)
            {
                m_TransformPacketManeger = new NetworkPacketManeger<TransformPackage>();
                if (hasAuthority)
                    m_TransformPacketManeger.onRequirePackageTransmit += TransmitTransformPackagesToAll;

            }

            return m_TransformPacketManeger;
        }
    }
    public virtual void FixedUpdate()
    {
        transformPacketManeger.Tick();
    }

    /// <summary>
    ///          Transform
    /// </summary>
    /// <param name="transform"></param>
    private void TransmitTransformPackagesToAll(byte[] positions)
    {
        CmdTransmitTransformPackagesToAll(positions);
    }
    [Command]
    private void CmdTransmitTransformPackagesToAll(byte[] physics)
    {
        RpcTransmitTransformPackagesToAll(physics);
    }
    [ClientRpc]
    void RpcTransmitTransformPackagesToAll(byte[] data)
    {
        transformPacketManeger.ReceiveData(data);
    }
}
