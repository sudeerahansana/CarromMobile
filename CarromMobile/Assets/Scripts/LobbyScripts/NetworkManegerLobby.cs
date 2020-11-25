
using Mirror;
using Mirror.Websocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



//inherited therfor execute automaticaly when hosted or joined
public class NetworkManegerLobby : NetworkManager
{
    private bool wrongIp = false;
    [Scene] [SerializeField] private string menuScene = string.Empty;     //[Scene] letting add scene name jus by fragging

    [Header("Scripts")]
    [SerializeField] private Room2PlayerLobby room2PlayerPrefab=null;
    [SerializeField] private Room4playerLobby room4PlayerPrefab = null;
    [SerializeField] private Game2Player game2PlayerPrefabs = null;
    [SerializeField] private Game4player game4PlayerPrefabs = null;


    [Header("Prefabs")]
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject cameraSpawnSystem = null;
    [SerializeField] private GameObject game2pManager = null;
    [SerializeField] private GameObject chatManegerPrefab= null;
    [SerializeField] private GameObject game4pManager = null;
  
    public static event Action OnClientConnected;      //can listen from any where(static)
    public static event Action OnClientDisConnected;     //listen in joinLobby
    public static event Action<NetworkConnection> OnServerReadied;  //when someOne Readied to trigger player Spawn System and camera spawnSystem
    public static event Action<List<NetworkConnection>> PassRoomPlayers;      //to pass creted list with RoomPlayers.Connections invoked in OnServerChangeScene
    public static event Action OnShuttingDownNgrok;
    public static event Action OnPlayerLeftinServer;
  
    public List<NetworkConnection> PlayersConn = new List<NetworkConnection>();     //to pass conns to gameManeger.Items adds in ServerChangeScene
    public List<Room2PlayerLobby> Room2Players { get; set; } = new List<Room2PlayerLobby>();     //in the room
    public List<Room4playerLobby> Room4Players { get; set; } = new List<Room4playerLobby>();     //in the room

    public List<Game2Player> Game2Players { get; } = new List<Game2Player>();    //removed from room and added to while in the game

    public List<Game4player> Game4Players { get; } = new List<Game4player>();    //removed from room and added to while in the game

    private void OnEnable()
    {
        Telepathy.Client.OnWrongIpError += WrongIp;
        JoinLobbyMenu.OnInvalidIp += WrongIp;
    }
    #region Initial setup
    public override void OnStartServer()
    {
        Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();  // for        whith out draging spawnable prefabs
    }                                                                                                   //both        created a directory to store them
    public override void OnStartClient()                                                                  //instance   and load them from there
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefabs in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefabs);
        }
    }
    private void OnDisable()
    {
        Telepathy.Client.OnWrongIpError -= WrongIp;
        JoinLobbyMenu.OnInvalidIp -= WrongIp;

    }
    #endregion


    #region Handle Client Connection and Disconnections
    private void WrongIp()
    {
        wrongIp = true;
    }
    public override void OnClientConnect(NetworkConnection conn)     //when client connect to the server
    {
        base.OnClientConnect(conn);         //to the base logic provided by mirror[base refer method overidn]
        OnClientConnected?.Invoke();//similar to   this-----if(OnClientConnected!= null)OnClientConnected();
                                    //Invokes the event only if OnClientConnected is not null

    }

    public override void OnClientDisconnect(NetworkConnection conn)     // called only on clients when 
    {                                                                   //1)wrong ip
        base.OnClientDisconnect(conn);
        if (wrongIp)
        {
            pluginContoller.transform.Find("Canvas/Panel_InvalidRoomId").gameObject.SetActive(true);
            wrongIp = false;
        }
        else
        {
            pluginContoller.transform.Find("Canvas/Panel_HostLeaving").gameObject.SetActive(true);
        }

    }
  
    public override void OnStopClient()       //called on client when (wrong ip / client disconnects / server leave)
    {                                         //called on server when (server stop)
        OnClientDisConnected?.Invoke();
        //NetworkClient.Send(new Notification { content = "0" + PlayerPrefs.GetString("Name") + "left the game" });
        base.OnStopClient();
        AdMob.adMobInstance.RequestInterstitial();
        if (mainUi == null)
        {
            mainUi = GameObject.FindWithTag("MainUi");
        }
        Debug.Log("MainUi"+(mainUi==null));
        if (mainUi != null)
        {
            mainUi.GetComponent<Animator>().SetInteger("AnimeInt", 0);
        }
    }
  
    #endregion


    #region Handle server connect and disconnects 
    public override void OnServerConnect(NetworkConnection conn)        //execute on server when client connects
    {

        if (numPlayers >= maxConnections)     //check for number of players
        {
            conn.Disconnect();     //disconnect extera player
            return;

        }

        if (SceneManager.GetActiveScene().path != menuScene)   //if someone joins after game started 
        {
            conn.Disconnect();       //disconnect him
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)   //when client on the server adds a new player not himself to scene
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (maxConnections == 2)
            {
                bool isLeader = Room2Players.Count == 0;        //when it comes to exucution of method,check currnet length of the list

                Room2PlayerLobby roomPlayerInstance = Instantiate(room2PlayerPrefab);   //spawn in roomplayer prefab
                GameObject chatManegerInstance = Instantiate(chatManegerPrefab);

                roomPlayerInstance.IsLeader = isLeader;
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);   //tying playerd connction with this game object
                NetworkServer.Spawn(chatManegerInstance, conn);
            }
            else
            {
      
                bool isLeader = Room4Players.Count == 0;        //when it comes to exucution of method,check currnet length of the list

                Room4playerLobby roomPlayerInstance = Instantiate(room4PlayerPrefab);   //spawn in roomplayer prefab
                GameObject chatManegerInstance = Instantiate(chatManegerPrefab);

                roomPlayerInstance.IsLeader = isLeader;
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);   //tying playerd connction with this game object
                NetworkServer.Spawn(chatManegerInstance, conn);
                   
                //then server knows this game objects represent this connction
                // OnRoomPlayerConnections?.Invoke(maxConnections);
                
            }
        }
        
    }



    
    public override void OnServerDisconnect(NetworkConnection conn)     //called onServer whe client disconnect as well as when server or host disconnects
    {
       
        if (maxConnections == 2)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<Room2PlayerLobby>();   //because list is in tyle NetworkRoomPlayerLobby
                if (player != null)
                {
                    Room2Players.Remove(player);              //remove that player from the list
                    NotifyPlayersOfReadyState();

                }
                else
                {
                    var gamePlayer = conn.identity.GetComponent<Game2Player>();   //because list is in tyle NetworkRoomPlayerLobby
                    if (conn.ToString().Substring(11, 1) != "0")
                    {
                        OnPlayerLeftinServer?.Invoke();
                    }

                }
                TelepathyTransport.server.AlterConnIdWhen2PClientDisconnects(int.Parse(conn.ToString().Substring(11, 1)));

            }

        }
        else
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<Room4playerLobby>();   //because list is in tyle NetworkRoomPlayerLobby
                if (player != null)
                {
                    Room4Players.Remove(player);              //remove that player from the list
                    NotifyPlayersOfReadyState();

                }
                else
                {
                    var gamePlayer = conn.identity.GetComponent<Game4player>();   //because list is in tyle NetworkRoomPlayerLobby
                    if (conn.ToString().Substring(11, 1) != "0")
                    {
                        OnPlayerLeftinServer?.Invoke();
                    }

                }
                TelepathyTransport.server.AlterConnIdWhen4PClientDisconnects(int.Parse(conn.ToString().Substring(11, 1)));
            }
        }
        
        base.OnServerDisconnect(conn);
       
    }
    public override void OnStopServer()         //called on everyOne when serverStops
    {
        if (NetworkServer.active)
        {
            OnShuttingDownNgrok?.Invoke();
        }
        if(maxConnections==2)
            Room2Players.Clear();
        else 
            Room4Players.Clear();

    }
    #endregion


    #region Game Starting Process and scene change process
    public void StartGame()        //when start BUtton Pressed
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) { return; }
            if(maxConnections==2)
                ServerChangeScene("Carrom_2Boad");
            else 
                ServerChangeScene("Carrom_4Boad");

        }

    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (maxConnections ==4)
        {
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Carrom_4Boad"))
            {
                for (int i = 0; i < Room4Players.Count; i++)
                {
                    var conn = Room4Players[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                    PlayersConn.Add(conn);
                }
                Room4playerLobby[] player = new Room4playerLobby[Room4Players.Count];
                for (int i = Room4Players.Count - 1; i >= 0; i--)
                {
                    NetworkConnection conn = Room4Players[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                    player[int.Parse(conn.ToString().Substring(11, 1))] = Room4Players[i];
                    /*var gamePlayerIstance = Instantiate(game4PlayerPrefabs);
                    gamePlayerIstance.SetDisplayName(Room4Players[i].DisplayName);
                    //  NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);*/

                }
                for(int i = player.Length - 1; i >= 0; i--)
                {
                    var conn = player[i].connectionToClient;
                    var gamePlayerIstance = Instantiate(game4PlayerPrefabs);
                    gamePlayerIstance.SetDisplayName(player[i].DisplayName);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);
                }
                /*  for (int i =0; i < RoomPlayers.Count; i++)
                  {
                      var conn = RoomPlayers[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                      var gamePlayerIstance = Instantiate(gamePlayerPrefabs);
                      gamePlayerIstance.SetDisplayName(RoomPlayers[i].DisplayName);
                      //  NetworkServer.Destroy(conn.identity.gameObject);
                      NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);

                  }
      */
                }
            }
        else
        {
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Carrom_2Boad"))
            {
                for (int i = 0; i < Room2Players.Count; i++)
                {
                    var conn = Room2Players[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                    PlayersConn.Add(conn);
                }
                for (int i = Room2Players.Count - 1; i >= 0; i--)
                {
                    var conn = Room2Players[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                    var gamePlayerIstance = Instantiate(game2PlayerPrefabs);
                    gamePlayerIstance.SetDisplayName(Room2Players[i].DisplayName);
                    //  NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);

                }
                /*  for (int i =0; i < RoomPlayers.Count; i++)
                  {
                      var conn = RoomPlayers[i].connectionToClient;                          //gameObject.connectionToClient give the networkConnectionOf the the object which have networkIdentity
                      var gamePlayerIstance = Instantiate(gamePlayerPrefabs);
                      gamePlayerIstance.SetDisplayName(RoomPlayers[i].DisplayName);
                      //  NetworkServer.Destroy(conn.identity.gameObject);
                      NetworkServer.ReplacePlayerForConnection(conn, gamePlayerIstance.gameObject);

                  }
      */
            }
        }
        //Form menu TO game
       
        base.ServerChangeScene(newSceneName);

    }


    public override void OnServerSceneChanged(string newSceneName)
    {

        if (maxConnections == 2)
        {
            if (newSceneName.StartsWith("Carrom_2Boad"))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);   //can pass another parameter,if not this spawn system will be owned by server
                GameObject cameraSpawnSystemInstance = Instantiate(cameraSpawnSystem);
                NetworkServer.Spawn(cameraSpawnSystemInstance);
                GameObject gameManagerInstance = Instantiate(game2pManager);
                NetworkServer.Spawn(gameManagerInstance);
            }
        }
        else
        {
            if (newSceneName.StartsWith("Carrom_4Boad"))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);   //can pass another parameter,if not this spawn system will be owned by server
                GameObject cameraSpawnSystemInstance = Instantiate(cameraSpawnSystem);
                NetworkServer.Spawn(cameraSpawnSystemInstance);
                GameObject gameManagerInstance = Instantiate(game4pManager);
                NetworkServer.Spawn(gameManagerInstance);
            }
        }

        PassRoomPlayers?.Invoke(PlayersConn);

        
    }
    #endregion

    #region Others
    public void NotifyPlayersOfReadyState()
    {
        if (maxConnections == 2)
        {
            foreach (var player in Room2Players)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }
        else
        {
            foreach (var player in Room4Players)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }
        
    }

    public bool IsReadyToStart()
    {
        if (maxConnections == 2)
        {
            if (numPlayers < maxConnections) { return false; }
            foreach (var player in Room2Players)           //need everyBody in the room to start the game
            {
                if (!player.IsReady) { return false; }

            }
        }
        else
        {
            if (numPlayers < 3) { return false; }
            foreach (var player in Room4Players)           //need everyBody in the room to start the game
            {
                if (!player.IsReady) { return false; }

            }
        }
       

        return true;

    }
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);

    }

    public override void OnSceneReload()
    {
        base.OnSceneReload();
        Debug.Log("Destroy");
        Destroy(gameObject);
    }
    #endregion

}
