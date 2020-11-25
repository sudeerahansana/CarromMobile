
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System;

public class Room4playerLobby : NetworkBehaviour
{

    [SerializeField] private Animator dialogeButtonAnime = null;
    [SerializeField] private GameObject dialogPanel = null;


    [Header("UI")]
    [SerializeField] private GameObject lobbyUi = null;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI roomIdText = null;
    [SerializeField] private GameObject[] ReadyImages = new GameObject[4];
    [SerializeField] private GameObject[] NotReadyImages = new GameObject[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private GameObject roomIdTextObj = null;
    [SerializeField] private RectTransform chatButton = null;
 //   [SerializeField] private RectTransform panel = null;


    public static event Action<int, int, int> onEnableRoomPlayer;


    [SyncVar(hook = nameof(HandDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;



    private bool isLeader;

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }


    private NetworkManegerLobby room;

    private NetworkManegerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManegerLobby;
        }
    }
    private void OnEnable()
    {
        NetworkManegerLobby.OnClientDisConnected += OnClientOnlyDisconnects;

    }
    private void OnDisable()
    {
        NetworkManegerLobby.OnClientDisConnected -= OnClientOnlyDisconnects;
    }
    public override void OnStartAuthority()
    {
        name = PlayerPrefs.GetString("Name");
        CmdSetDisplayName(name);
        lobbyUi.SetActive(true);
        if (NetworkServer.active)
        {
            roomIdTextObj.SetActive(true);
            roomIdText.text = "ROOM ID:-" + PluginController.roomId;
        }
       
        SetOnEnableRoomPlayer();
    }

    private void SetOnEnableRoomPlayer()
    {
        float scaleFactor = (float)gameObject.GetComponentInChildren<Canvas>().scaleFactor;
        int btnSize = (int)(chatButton.rect.width * scaleFactor);
        int btnLeftMargine = (int)((Math.Abs(chatButton.anchoredPosition.x) - chatButton.rect.width / 2) * scaleFactor);
        int btnTopMargine = (int)((Math.Abs(chatButton.anchoredPosition.y) - chatButton.rect.height / 2) * scaleFactor);


        onEnableRoomPlayer?.Invoke(btnSize, btnLeftMargine, btnTopMargine);
    }

    public override void OnStartClient()
    {
        Room.Room4Players.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Room.Room4Players.Remove(this);
        UpdateDisplay();
        Destroy(gameObject);
    }

    private void OnClientOnlyDisconnects()
    {
        Room.Room4Players.Remove(this);
        UpdateDisplay();
        Destroy(gameObject);

    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    public void HandDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {

        if (!hasAuthority) //if the player who updated is not us
        {

            foreach (var player in Room.Room4Players)    //find us
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        //if we updated

        for (int i = 0; i < playerNameTexts.Length; i++)   //clear everyOns display
        {
            playerNameTexts[i].text = "Waiting for player...";
            ReadyImages[i].SetActive(false);
            NotReadyImages[i].SetActive(false);
        }


        for (int i = 0; i < Room.Room4Players.Count; i++)                //and the setting all back
        {
            playerNameTexts[i].text = Room.Room4Players[i].DisplayName;

            if (Room.Room4Players[i].IsReady)
                ReadyImages[i].SetActive(true);
            else
                NotReadyImages[i].SetActive(true);

        }
    }


    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    public void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;

    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }
    [Command]
    public void CmdStartGame()
    {
        if (Room.Room4Players[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    private void Quit()
    {
        if (dialogPanel.activeSelf)
            dialogPanel.SetActive(false);

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            room.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            room.StopClient();
        }
        else if (NetworkServer.active)
        {
            room.StopServer();
        }

    }

    public void QuitButton()
    {
        dialogeButtonAnime.SetInteger("DialogeButton", 1);
        Invoke("Quit", 1f);
    }

    public void NoButton()
    {
        dialogeButtonAnime.SetInteger("DialogeButton", 2);
        Invoke("DissapearDialog", 1f);
    }

    private void DissapearDialog()
    {
        dialogPanel.SetActive(false);
    }


}
