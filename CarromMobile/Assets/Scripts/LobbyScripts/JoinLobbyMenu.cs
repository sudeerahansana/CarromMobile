
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManegerLobby networkManeger = null;
    [SerializeField] private TelepathyTransport telepathy=null;   //instance of TelepathyScript

    [Header("UI")]
 //   [SerializeField] private GameObject dialogePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Animator mainUiAnimator = null;
    public static event Action OnInvalidIp;
 

    private void OnEnable()    
    {
        NetworkManegerLobby.OnClientConnected += HandleClientConnected;    
       // NetworkManegerLobby.OnClientDisConnected += HandleClientDisConnected;
    }
    private void OnDisable()   
    {
        NetworkManegerLobby.OnClientConnected -= HandleClientConnected;     
       // NetworkManegerLobby.OnClientDisConnected -= HandleClientDisConnected;
    }
    
   
    public void JoinLobby()
    {
        mainUiAnimator.SetInteger("AnimeInt", 7);

        Invoke("JoinAnimeEvent", 0.5f);
    }
    public void JoinAnimeEvent()
    {
        try
        {
            if (ipAddressInputField.text.Substring(0, 1) == "c")
            {
                string ipAddress = ipAddressInputField.text.Substring(1);
                networkManeger.networkAddress = ipAddress;
                networkManeger.StartClient();
                AdMob.adMobInstance.LoadAdd();

            }
            else
            {
                string ipAddress = ipAddressInputField.text.Substring(0, 1) + ".tcp.ngrok.io";  //slipt ip addr from roomId(input)
                string portNumber = ipAddressInputField.text.Substring(1);     //split portNumber
                                                                               //string ipAddress = ipAddressInputField.text;
                networkManeger.networkAddress = ipAddress;
                telepathy.port = System.Convert.ToUInt16(portNumber);      //port number is ushort in Telepathy
                networkManeger.StartClient();
            }
        }
        catch (FormatException e)
        {
            OnInvalidIp?.Invoke();
        }
        catch(OverflowException e)
        {
            OnInvalidIp?.Invoke();
        }
       // AdMob.adMobInstance.RequestInterstitial();
    }
   private void HandleClientConnected()      //listen when clientConnected through event
    {
      
        Debug.Log("Handle Client Connect");
      
    }


  /*  private void HandleClientDisConnected(string message)
    {
        joinButton.interactable = true;
        Debug.Log("Handle Client Disconnect");
        if (message != "")
        {
            Debug.Log("Message" + message);
            dialogePanel.SetActive(true);
        }
        else
        {
            mainPanel.SetActive(true);
        }
    }*/

}
