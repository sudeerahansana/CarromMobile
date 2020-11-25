using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class Notification : MessageBase
{
    public string content;
}
public class Messages : MonoBehaviour
{
    /*public static event Action OnPlayerLeft;
    public static event Action OnPlayerJoined;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (!NetworkClient.active)
            NetworkServer.RegisterHandler<Notification>(OnNotificationServer);
        else
            NetworkClient.RegisterHandler<Notification>(OnNotificationClient);

    }

    private void OnNotificationServer(NetworkConnection conn, Notification msg)
    {
        Debug.Log("Message Receive in server");
      //  NetworkServer.SendToAll(new Notification { content = msg.content });
        notificationTxt.gameObject.SetActive(true);
        Invoke("CloseText", 1f);
        notificationTxt.text = msg.content.Substring(1);
        if (msg.content.Substring(0, 1) == "0")
            OnPlayerJoined?.Invoke();
        else
            OnPlayerLeft?.Invoke();

        Invoke("CloseText", 1f);
    }
    private void OnNotificationClient(NetworkConnection conn, Notification msg)
    {
        Debug.Log("Message Receive in client");
        notificationTxt.gameObject.SetActive(true);
        Invoke("CloseText", 1f);
        notificationTxt.text = msg.content.Substring(1);
        if (msg.content.Substring(0, 1) == "0")
            OnPlayerJoined?.Invoke();
        else
            OnPlayerLeft?.Invoke();

        
    }
    private void CloseText()
    {
        notificationTxt.gameObject.SetActive(false);
    }*/
}
