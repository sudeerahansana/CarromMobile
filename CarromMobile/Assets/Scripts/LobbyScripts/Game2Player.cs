using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game2Player : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

    [SerializeField] private TextMeshProUGUI[] playerNameTexts = new TextMeshProUGUI[2];
    [SerializeField] private GameObject gameUI=null;
    [SerializeField] private GameObject[] players=null;
    [SerializeField] private GameObject dialogePanel = null;
    [SerializeField] private GameObject gameEndImage = null;
    [SerializeField] private TMP_Text gameEndWinner = null;
    [SerializeField] private GameObject playerLeftImage = null;

    [SerializeField] private Canvas canvas = null;

    [SerializeField] private RectTransform chatButton = null;
    [SerializeField] private RectTransform containerPanel = null;

    public static event Action<int,int,int> onEnableGamePlayer;
   
    public string myName;
  

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

    public override void OnStartAuthority()
    {
        myName = PlayerPrefs.GetString("Name");
        Game2PManeger.OnDiskSpawnGamePlayer += AdjustNonPlayerDisplay;
        Game2PManeger.GameEnds += OnGameEnds;
        Game2PManeger.OnGamePlayerLeft += PlayerLeft;
        gameUI.SetActive(true);
        SetOnEnableGamePlayer(); 
    }
   
    private void SetOnEnableGamePlayer()
    {
        float scaleFactor = (float)gameObject.GetComponentInChildren<Canvas>().scaleFactor;
        float panelScale = (float)containerPanel.localScale.x;
        int btnSize = (int)(chatButton.rect.width*panelScale*scaleFactor);
        int btnLeftMargine = (int)((Math.Abs(chatButton.anchoredPosition.y) - chatButton.rect.width / 2)*panelScale  * scaleFactor);
        int btnTopMargine = (int)((Math.Abs(chatButton.anchoredPosition.x) - chatButton.rect.height / 2) *panelScale * scaleFactor);
     
        onEnableGamePlayer?.Invoke(btnSize,btnLeftMargine,btnTopMargine);

    }
    private void OnDestroy()
    {
        Game2PManeger.OnDiskSpawnGamePlayer -= AdjustNonPlayerDisplay;
        NetworkManegerLobby.OnClientDisConnected -= OnClientOnlyDisconnects;
        Game2PManeger.GameEnds -= OnGameEnds;
        Game2PManeger.OnGamePlayerLeft -= PlayerLeft;

    }

    private void Update()
    {
        if (dialogePanel.activeSelf && canvas.sortingOrder!=3)
        {
            canvas.sortingOrder = 3;
        }
        else if(!dialogePanel.activeSelf && canvas.sortingOrder !=1)
        {
            canvas.sortingOrder = 1;
        }
    }
    private void OnClientOnlyDisconnects()
    {
        Room.Game2Players.Remove(this);
        Destroy(gameObject);

    }
    private void AdjustNonPlayerDisplay(string passedNetId,bool redInHole)
    {
        CmdAdjustNonPlayerDisplay(passedNetId,redInHole);
    }
    [Command]
    private void CmdAdjustNonPlayerDisplay(string passedNetId, bool redInHole)
    {
        RpcAdjustNonPlayerDisplay(passedNetId,redInHole);
    }

    [ClientRpc]
    private void RpcAdjustNonPlayerDisplay(string passedNetId, bool redInHole)
    {
         OwnNonPlayerAdjustment(passedNetId,redInHole); 
       
    }

    private void OwnNonPlayerAdjustment(string passedId, bool redInHole)             //this is not working because could not get networkConnection of a client
                                                                     //cause it returns null
    {
        if (!hasAuthority) //if its server hasAuthority becomes true
        {

            foreach (var player in Room.Game2Players)    
            {
                if (player.hasAuthority)
                {
                    player.OwnNonPlayerAdjustment(passedId,redInHole);
                    break;
                }
            }
            return;
        }
        for(int i = 0; i < room.Game2Players.Count; i++)
        {
            players[i].GetComponentInChildren<Image>().color = Color.white;
        }
        players[int.Parse(passedId)].GetComponentsInChildren<Image>()[0].color = Color.red;
        players[int.Parse(passedId)].transform.Find("Image_Red").gameObject.SetActive(redInHole);

    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.Game2Players.Add(this);
        UpdateNonPlayerUI();
      
    }
    private void OnGameEnds(int conn)
    {
        for(int i=0;i<Room.Game2Players.Count;i++)
        {
            if (myName == Room.Game2Players[(Room.Game2Players.Count-1)-i].myName)
            {
                if (i == conn)
                {
                    dialogePanel.SetActive(true);
                    gameEndImage.SetActive(true);
                    gameEndWinner.text = "You Won";
                }
                else
                {
                    dialogePanel.SetActive(true);
                    gameEndImage.SetActive(true);
                    gameEndWinner.text = "You Lost";
                }
                return;
            }
        }
    }
  
    private void PlayerLeft()
    {
        dialogePanel.SetActive(true);
        playerLeftImage.SetActive(true);
    }
     public void Yes()
    {
        Time.timeScale = 1f;
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
   
    [Obsolete]
    public override void OnNetworkDestroy()
    {

        Room.Game2Players.Remove(this);
       
    }
    [Server]
    public void SetDisplayName(string displayName )
    {
        this.displayName = displayName;
    }

    private void UpdateNonPlayerUI()
    {

        if (!hasAuthority) //if the player who updated is not us
        {

            foreach (var player in Room.Game2Players)    //find us
            {
                if (player.hasAuthority)
                {
                    player.UpdateNonPlayerUI();
                    break;
                }
            }
            return;
        }
        for (int i = 0; i < Room.Game2Players.Count; i++)
        {
            playerNameTexts[i].text = Room.Game2Players[Room.Game2Players.Count-1-i].displayName;

        }
       
    }
}
