using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [System.Serializable]
    public class ChatPackage
    {
        public string name;
        public string msg;
    }

    float networkSendRate = 0.1f;
    private NetworkPacketManeger<ChatPackage> chatPacketManeger;
    public static event Action<string,string> onMsgRecv;
    private string senderName;

    public void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        chatPacketManeger = new NetworkPacketManeger<ChatPackage>();
        PluginController.onMsgSend += SendMsg;
        chatPacketManeger.onRequirePackageTransmit += OnMsgSend;
        chatPacketManeger.sendSpeed = networkSendRate;
        NetworkManegerLobby.OnClientDisConnected += DestroyGameObject;
        senderName = PlayerPrefs.GetString("Name");
        Debug.Log("sender Name" + senderName);
    }
    private void OnDisable()
    {
        PluginController.onMsgSend -= SendMsg;
        chatPacketManeger.onRequirePackageTransmit -= OnMsgSend;
        NetworkManegerLobby.OnClientDisConnected -= DestroyGameObject;

    }

    public virtual void FixedUpdate()
    {
        chatPacketManeger.Tick();
        OnMsgRecv();

    }
    private void SendMsg(string msgSend)
    {
       chatPacketManeger.AddPackages(new ChatPackage
        {
            name = senderName,
            msg = msgSend
        }) ;
        
       
    }
    private void OnMsgRecv()
    {
        if (hasAuthority)
            return;
        var data = chatPacketManeger.GetNextDataReceived();

        if (data == null)
        {
            return;
        }
        onMsgRecv?.Invoke(data.name,data.msg);
    }
    private void OnMsgSend(byte[] msg)
    {
        CmdTransmitChatPackagesToAll(msg);

    }

    [Command]
    private void CmdTransmitChatPackagesToAll(byte[] msg)
    {
        RpcReceiveChatOnClient(msg);
    }

    [ClientRpc]
    void RpcReceiveChatOnClient(byte[] data)
    {
        chatPacketManeger.ReceiveData(data);

    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
