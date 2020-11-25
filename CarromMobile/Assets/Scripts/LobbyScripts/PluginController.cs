using Mono.Nat;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading;
using Mirror.Examples.Basic;

//[System.Serializable]
public class PluginController : MonoBehaviour
{
     private string[] tokens = { };
    [SerializeField] private GameObject noConnDialogePanel = null;
    [SerializeField] private GameObject roomIdDialogPanel = null;
    [SerializeField] private TextMeshProUGUI dialogTextField=null;
    [SerializeField] private GameObject netwrokManager = null;
    [SerializeField] private GameObject[] dialogeImages = null;
    [SerializeField] private RectTransform[] settings = null;
    [SerializeField] private GameObject mainUi=null;
    [SerializeField] private GameObject signInWaitPanel = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private GameObject waitPanel = null;


    private int buttonSize, leftMargine, topMargine;
    private static string playerName, email, password,token,message;
    private static bool msgSend;
   // private static int browserResult;
    public static string roomId = null;

    public static event Action<string> onMsgSend;
   
    const string pluginName = "com.example.ngrokprolibrary.NgrokPluging";//Android studio project NAME.CLASSNAME

    static AndroidJavaClass _pluginClass;//static public getters-to access classs object
    static AndroidJavaObject _pluginInstance;//static public getters-to access instance of the object
    public static PluginController pluginControllerInstance;



    class BrowseCallback : AndroidJavaProxy
    {
        public BrowseCallback() : base(pluginName + "$BrowserCallback")   //class in plugin
        {
        }
     
        public void onBrowseComplete(int result,string pName,string pEmail,string pPassword,string pToken)
        {
            if (result == 1)
            {
               
                PluginController.playerName = pName;
                PluginController.email = pEmail;
                PluginController.password = pPassword;
                PluginController.token = pToken;
                //PluginController.browserResult = 1;  //WHEN EXECUTED IN UPDATE
                PluginController.pluginControllerInstance.saveData(); //fix for execution in update;
              
            }
            else if(result==2)
            {
               // PluginController.browserResult = 2;
                PluginController.pluginControllerInstance.passToken();
            }

        }
    }

    class ChatCallback : AndroidJavaProxy
    {
         public ChatCallback() : base(pluginName + "$ChatCallback")   //class in plugin
        {
           
        }
       
        public void onChatCallback(string msgRecv)
        {
            PluginController.msgSend = true;
            PluginController.message = msgRecv;
        }
    }
    public static AndroidJavaClass PluginClass//with this we con only modify PluginClass only by executing *(look down) but can aceess anyWhere
    {                                                                                                      
        get                                                                                               
        {
            if (_pluginClass == null)                                                         
            {                                                       
                _pluginClass = new AndroidJavaClass(pluginName);//*(here)
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");//to pass the activity to gain context
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity"); //currentactivity is method in unity
                _pluginClass.SetStatic<AndroidJavaObject>("mainActivity", activity);
            }
            return _pluginClass;
        }
    }
    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");//static method(CallStatic) getinstance() in an.studio called
            }
            return _pluginInstance;
        }
    }

    
    private void OnEnable()
    {
        Room2PlayerLobby.onEnableRoomPlayer += SetUpChatUiForRoomPlayer;
        Game2Player.onEnableGamePlayer += SetChatUiForGamePlayer;
        Room4playerLobby.onEnableRoomPlayer += SetUpChatUiForRoomPlayer;
        Game4player.onEnableGamePlayer += SetChatUiForGamePlayer;
        NetworkManegerLobby.OnClientDisConnected += CloseChatUi;
        CameraSetup.OnCamEnable += DisplayChatUiForGamePlayer;
        NetworkManegerLobby.OnShuttingDownNgrok += ShutDownNgrok;
        ChatManager.onMsgRecv += MsgRecv;
        NetworkManager.OnSceneChange += CloseChatOnSceneChange;
        
    }
    private void OnDisable()
    {
        Room2PlayerLobby.onEnableRoomPlayer -= SetUpChatUiForRoomPlayer;
        Game2Player.onEnableGamePlayer -= SetChatUiForGamePlayer;
        Room4playerLobby.onEnableRoomPlayer -= SetUpChatUiForRoomPlayer;
        Game4player.onEnableGamePlayer -= SetChatUiForGamePlayer;
        NetworkManegerLobby.OnClientDisConnected -= CloseChatUi;
        CameraSetup.OnCamEnable -= DisplayChatUiForGamePlayer;
        NetworkManegerLobby.OnShuttingDownNgrok -= ShutDownNgrok;
        ChatManager.onMsgRecv -= MsgRecv;
        NetworkManager.OnSceneChange -= CloseChatOnSceneChange;
    }
    void Awake()
    {
        Application.targetFrameRate = 60;
        if (pluginControllerInstance == null)
        {
            pluginControllerInstance = this;
        }
        else
        {
            if(pluginControllerInstance.netwrokManager ==null || pluginControllerInstance.mainUi == null)
            {
                pluginControllerInstance.netwrokManager = GameObject.FindWithTag("NetworkManeger");
                pluginControllerInstance.mainUi = GameObject.FindWithTag("MainUi");
               
            }
            pluginControllerInstance.settings[0].anchorMin = new Vector2(1, 0);
            pluginControllerInstance.settings[0].anchorMax = new Vector2(1, 0);
            pluginControllerInstance.settings[0].pivot = new Vector2(0.5f, 0.5f);
            pluginControllerInstance.settings[0].anchoredPosition = new Vector2(-50, 50);
            pluginControllerInstance.settings[0].gameObject.SetActive(true);
            pluginControllerInstance.settings[1].anchorMin = new Vector2(1, 0);
            pluginControllerInstance.settings[1].anchorMax = new Vector2(1, 0);
            pluginControllerInstance.settings[1].pivot = new Vector2(0.5f, 0.5f);
            pluginControllerInstance.settings[1].anchoredPosition = new Vector2(-50, 50);
            pluginControllerInstance.settings[2].Rotate(new Vector3(0, 0, 90));

            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
        private void Start()
    {
       // browserResult = 0;
        msgSend = false;
    }

    private void Update()
    {
        if (msgSend)
        {
            msgSend = false;
            onMsgSend?.Invoke(message);
        }
       /* if (browserResult == 1)
        {
            browserResult = 0;
            saveData();
        }
        else if (browserResult == 2)
        {
            browserResult = 0;
            passToken();
        }*/

    }
  
    public void noConnection()
    {
        noConnDialogePanel.SetActive(true);
    
    }

    public void Host2Players()  //when host lobby button pressed for 2 players
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            roomId = "Room Id";
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 2;
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().StartHost();
           // ipAdressJoinPanel.SetActive(false);
            /* roomIdDialogPanel.SetActive(true);
             netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 2;
             Invoke("HostTillLoadingScreenLoads", 1f);*/

        }
        else
        {
            Debug.LogWarning("AlertView not Supported on this platform");
           
            // roomIdDialogPanel.SetActive(true);
            roomId = "Room Id";
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 2;
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().StartHost();
           // ipAdressJoinPanel.SetActive(false);
            // onRoomIdGet?.Invoke("roomId");
        }

    }
   
    public void Host4Players()  //when host lobby button pressed for 2 players
    {
        if (Application.platform == RuntimePlatform.Android)
        {
           
            roomId = "Room Id";
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 4;
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().StartHost();
            /*oomIdDialogPanel.SetActive(true);
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 4;
            Invoke("HostTillLoadingScreenLoads", 1f);*/
        }
        else
        {
            Debug.LogWarning("AlertView not Supported on this platform");
            roomId = "Room Id";
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().maxConnections = 4;
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().StartHost();
        }


    }

    private void HostTillLoadingScreenLoads()
    {
        try
        {
            PluginInstance.Call("getRoomId");
            roomId = PluginInstance.GetStatic<string>("roomId");
            roomId = roomId.Substring(0, 1) + roomId.Substring(15);
            dialogeImages[0].SetActive(false);
            dialogeImages[1].SetActive(true);
            dialogTextField.text = "Share this Room Id with Your Friends\n" + roomId;
            netwrokManager.GetComponentInChildren<NetworkManegerLobby>().StartHost();
        }
        catch (AndroidJavaException e)
        {
            Debug.Log(e);
            dialogeImages[0].SetActive(false);
            dialogeImages[2].SetActive(true);
            mainUi.GetComponent<Animator>().SetInteger("AnimeInt", 0);
        }

    }

    public void OpenWebViewTapped()                  //when signin button pressed
    {
      
        mainUi.GetComponent<Animator>().SetInteger("AnimeInt", 9);
        signInWaitPanel.SetActive(true);
        OpenWebView("https://dashboard.ngrok.com/signup", 0);
    }

    public void OpenWebView(string URL,int pixelShift)  
    {
     
        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("showWebView", new object[] { URL, pixelShift, new BrowseCallback() });
    }

    private void SetUpChatUiForRoomPlayer(int btnSize,int btnLeft, int btnTop)
    {
        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("showChatView", new object[] {  btnSize, btnLeft,  btnTop,
                             new ChatCallback()});

    }
    private void CloseChatOnSceneChange()
    {
        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("closeOnSceneChange");
    }
    private void SetChatUiForGamePlayer(int btnSize,int btnLeft, int btnTop)
    {
        buttonSize = btnSize;
        leftMargine = btnLeft;
        topMargine = btnTop;
       
    }
    private void DisplayChatUiForGamePlayer()
    {
        loadingScreen.SetActive(false);
        waitPanel.SetActive(true);
        Invoke("CloseWaitPanel", 3f);
        settings[0].anchorMin = new Vector2(1, 0.5f);
        settings[0].anchorMax = new Vector2(1, 0.5f);
        settings[0].pivot = new Vector2(0.5f, 0.5f);
        settings[0].anchoredPosition = new Vector2(-25f, -100f);
        settings[0].gameObject.SetActive(true);
        settings[1].anchorMin = new Vector2(1, 0.5f);
        settings[1].anchorMax = new Vector2(1, 0.5f);
        settings[1].pivot = new Vector2(0.5f, 0.5f);
        settings[1].anchoredPosition = new Vector2(-25f, -100f);
        settings[2].Rotate(new Vector3(0, 0, -90));



        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("orientationChanged", new object[] {  buttonSize,  leftMargine,  topMargine,
                             new ChatCallback()});
    }
    private void CloseWaitPanel()
    {
        waitPanel.SetActive(false);
    }
    private void MsgRecv(string name,string msgRecv)
    {
         PluginInstance.Call("msgRecv",new object[] { name,msgRecv });
    }
    
    private void CloseChatUi()
    {
        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("closeChatView");
    }
    private void ShutDownNgrok()
    {
        if (Application.platform == RuntimePlatform.Android)
            PluginInstance.Call("closeServer");
    }
   

    private void saveData()         //called in finishedSignUp()
    {
        PlayerPrefs.SetInt("setUp", 1);

        PlayerPrefs.SetString("Name",playerName);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/data.bin";    //storage/emulated/0/Android/data/com.sudeera.unity/files.data.bin
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, "name:" + playerName + " email:" + email + " password:" + password + " Token:" + token);
        stream.Close();

        signInWaitPanel.SetActive(false);
        mainUi.GetComponent<Animator>().SetBool("MainEnter", true);
        Debug.Log("Data Saved in path" + path);
    }
    private void passToken()
    {
        PlayerPrefs.SetInt("setUp", 1);

        System.Random rd = new System.Random();
        int randomNumber = rd.Next(1, 5);
        PluginInstance.Call("configNgrokWithGivenToken", new object[] {tokens[randomNumber]});

        PlayerPrefs.SetString("Name", playerName);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/data.bin";    //storage/emulated/0/Android/data/com.sudeera.unity/files.data.bin
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, "name:" + playerName + " email:" + email + " password:" + password + " Token:" + tokens[randomNumber]);
        stream.Close();

        signInWaitPanel.SetActive(false);
        mainUi.GetComponent<Animator>().SetBool("MainEnter", true);
        Debug.Log("Token Passed");
    }
    
}
