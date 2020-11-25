using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
public class Game4Maneger : NetworkBehaviour
{
    private List<NetworkConnection> playerConn = new List<NetworkConnection>();   //[***]receive list pf connections from NetworkManegerLoby because of NetworkManegerLobby is in server this list is only be filled in GameManager copy on server [Look for ***]
    private List<GameObject> piecesInstances = new List<GameObject>();
    private static List<Transform> diskSpawnPoints = new List<Transform>();

    [Header("Prefabs")]
    [SerializeField] private GameObject diskPrefab = null;
    [SerializeField] private GameObject[] pieces = null;


    private NetworkIdentity currentNetId;
    private int index;
    private int connCount;

    public static event Action<String,bool> OnDiskSpawnGamePlayer;              //trigger in SPAWN DISK and subscribed in NetworkGamePlayer
    public static event Action<NetworkIdentity, int> OnDiskSpawn;    //trigger in spawn disk and subscribed in playerspawnSystem to spawn hand with relevant connection
    public static event Action EventAuthorityChange;                //trigger in pieces authority change and subscribed in pieces NetworkTransform
    public static event Action<int> GameEnds;
    public static event Action OnGamePlayerLeft;
    private bool redaPot = false;
    private bool redInHole = false;
    private bool doNotChange = true;
    private int whiteCount = 0;
    private int blackCount = 0;
    private int winner = 2;
    private int piecesReciveCount;
    private bool diskFall = false;

    #region Inetial Setup, Subscribing for actions and theire methods
    public void OnEnable()
    {                                                                               //find previous part above
        NetworkManegerLobby.PassRoomPlayers += GetList;                             //[***]this event only gets triggerd is GameManager copy in server 
        NetworkManegerLobby.OnPlayerLeftinServer += PlayerLeft;
        Pieces.EventFall += PieceFall;
        Pieces.FinishTurn += TurnEnds;
        PlayerSpawSystem.OnPlayerSpawn += ParentDisk;
        Pieces.RespawnDisplacedPieces += ReplacePiece;
        DiskNetworkTransform.DiskFall += OnDiskFall;

    }
    private void OnDisable()
    {
        NetworkManegerLobby.PassRoomPlayers -= GetList;
        NetworkManegerLobby.OnPlayerLeftinServer -= PlayerLeft;
        Pieces.EventFall -= PieceFall;
        Pieces.FinishTurn -= TurnEnds;
        PlayerSpawSystem.OnPlayerSpawn -= ParentDisk;
        Pieces.RespawnDisplacedPieces -= ReplacePiece;
        DiskNetworkTransform.DiskFall -= OnDiskFall;
    }
    public static void AddSpawnPoint(Transform transform)
    {
        diskSpawnPoints.Add(transform);
        diskSpawnPoints = diskSpawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        Debug.Log("Disk Spawn Points count" + diskSpawnPoints.Count);
    }
    public static void RemoveSpawnPoint(Transform transform) => diskSpawnPoints.Remove(transform);

    public void GetList(List<NetworkConnection> playersConn)
    {
        for (int i = 0; i < playersConn.Count; i++)
        {
            playerConn.Add(playersConn[i]);
        }
    }

    private void Start()
    {
        winner = 2;
        connCount = 0;
        if (isServer)
        {
            SpawnPieces();
            Invoke("SpawnDisk", 5f);
        }
    }
    #endregion

    [Server]
    private void SpawnDisk()
    {
        AdjustOption();
        doNotChange = false;
        if (winner == 0)
        {
            RpcGameEnds(0);
            return;
        }
        else if (winner == 1)
        {
            RpcGameEnds(1);
            return;
        }
        index = connCount % playerConn.Count;
        currentNetId = playerConn[index].identity;
        OnDiskSpawnGamePlayer?.Invoke(currentNetId.connectionToClient.ToString().Substring(11, 1),redInHole);
        OnDiskSpawn?.Invoke(currentNetId, index);

    }

    [Server]
    private void ParentDisk(GameObject playerInstance)
    {
        GameObject diskInstance = Instantiate(diskPrefab, diskSpawnPoints[index].position, diskSpawnPoints[index].rotation);
        NetworkServer.Spawn(diskInstance, playerConn[index]);
        RpcParentDisk(playerInstance, diskInstance);
        PiecesAuthorityChange();

    }

    [ClientRpc]
    private void RpcParentDisk(GameObject playerInstance, GameObject diskInstance)
    {
        diskInstance.transform.parent = playerInstance.transform;
    }
    [Server]
    private void SpawnPieces()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            GameObject piecesInstance = Instantiate(pieces[i]);
            piecesInstances.Add(piecesInstance);
            NetworkServer.Spawn(piecesInstance, playerConn[connCount % playerConn.Count]);
        }

    }

    [Server]
    private void PiecesAuthorityChange()
    {
        EventAuthorityChange?.Invoke();
         for (int i = 0; i < piecesInstances.Count; i++)
        {
            piecesInstances[i].GetComponent<NetworkIdentity>().RemoveClientAuthority();

            piecesInstances[i].GetComponent<NetworkIdentity>().AssignClientAuthority(playerConn[index]);

        }
        piecesReciveCount = 0;

    }
    [Server]
    private void OnDiskFall()
    {
        diskFall = true;
        Debug.Log("Disk Fall");
    }

    [Server]
    private void PieceFall(GameObject passedGameObject)
    {
        piecesReciveCount--;
        piecesInstances.Remove(passedGameObject);
        //string color = passedGameObject.GetComponent<NetworkIdentity>().ToString().Substring(0,1);
        int pieceNumber = int.Parse(passedGameObject.GetComponent<NetworkIdentity>().ToString().Substring(1, 2));
        if (pieceNumber == 0)
        {
            if (diskFall)
            {
                Debug.Log("Reds with disk");
                doNotChange = false;
                RespawnPieces(pieceNumber);
                piecesReciveCount++;
                TurnEnds();
            }
            else
            {
                Debug.Log("Reda falls");
                doNotChange = true;
                redInHole = true;
                TurnEnds();
            }

        }
        else if (index%2 == 0 && pieceNumber > 9)
        {
            if (diskFall)
            {
                Debug.Log("Piece Fall from white and disk falls");
                doNotChange = false;
                RespawnPieces(pieceNumber);
            }
            else
            {
                Debug.Log("Piece Fall from white");
                doNotChange = true;
                whiteCount++;
                if (redInHole)
                {
                    Debug.Log("Piece Fall from white and red in hole");
                    redaPot = true;
                    redInHole = false;
                    if (whiteCount == 9)
                        winner = 0;
                    TurnEnds();
                }
                else if (whiteCount == 9 && !redaPot)
                {
                    Debug.Log("Piece Fall from white but last piece");
                    piecesReciveCount--;
                    whiteCount = whiteCount - 2;
                    doNotChange = false;
                    RespawnPieces(10);
                    RespawnPieces(11);
                }
                else if (whiteCount == 9 && redaPot)
                {
                    Debug.Log("Piece Fall from white and win");
                    winner = 0;
                    TurnEnds();
                }
                else
                    TurnEnds();

            }

        }
        else if (index%2 == 1 && pieceNumber < 10)
        {
            if (diskFall)
            {
                Debug.Log("Piece Fall from black and disk falls");
                doNotChange = false;
                RespawnPieces(pieceNumber);
            }
            else
            {
                Debug.Log("Piece Fall from black");
                doNotChange = true;
                blackCount++;
                if (redInHole)
                {
                    Debug.Log("Piece Fall from black and red in hole");
                    redaPot = true;
                    redInHole = false;
                    if (blackCount == 9)
                        winner = 1;
                    TurnEnds();
                }
                if (blackCount == 9 && !redaPot)
                {
                    Debug.Log("Piece Fall from black but last piece");
                    piecesReciveCount--;
                    blackCount = blackCount - 2;
                    doNotChange = false;
                    RespawnPieces(1);
                    RespawnPieces(2);
                }
                else if (blackCount == 9 && redaPot)
                {
                    Debug.Log("Piece Fall from black and win");
                    winner = 1;
                    TurnEnds();
                }
                else
                    TurnEnds();


            }
        }
        else
        {
            if (pieceNumber > 9)
            {
                Debug.Log("Piece white fall");
                whiteCount++;
                if (whiteCount == 9 && redInHole)
                {
                    Debug.Log("Piece white fall win when reda in hole");
                    winner = 1;
                    TurnEnds();
                }
                else if (whiteCount == 9 && !redaPot)
                {
                    Debug.Log("Piece white last fall");
                    RespawnPieces(pieceNumber);
                    RespawnPieces(1);
                }
                else if (whiteCount == 9 && redaPot)
                {
                    Debug.Log("Piece white fall wint");
                    winner = 0;
                    TurnEnds();
                }
                else
                    TurnEnds();

            }
            else if (pieceNumber < 10)
            {
                Debug.Log("Piece black fall");
                blackCount++;
                if (blackCount == 9 && redInHole)
                {
                    Debug.Log("Piece black fall win when reda in hole");
                    winner = 1;
                    TurnEnds();
                }
                else if (blackCount == 9 && !redaPot)
                {
                    Debug.Log("Piece black last fall");
                    RespawnPieces(pieceNumber);
                    RespawnPieces(10);
                }
                else if (blackCount == 9 && redaPot)
                {
                    Debug.Log("Piece black fall win");
                    winner = 1;
                    TurnEnds();
                }
                else
                    TurnEnds();
            }
        }
    }

    private void AdjustOption()
    {
        Debug.Log("Adjust options fonNotChange" + doNotChange);
        if (!doNotChange)
            connCount++;
        if (!doNotChange && redInHole)
        {
            RespawnPieces(0);
            redInHole = false;
        }
    }
    private void TurnEnds()
    {
        piecesReciveCount++;
        if (piecesReciveCount >= piecesInstances.Count)
        {
            piecesReciveCount = 0;
            SpawnDisk();
        }
    }

    [Server]
    private void ReplacePiece(GameObject passedObject)
    {
        int pieceNumber = int.Parse(passedObject.GetComponent<NetworkIdentity>().ToString().Substring(1, 2));
        piecesInstances.Remove(passedObject);
        RespawnPieces(pieceNumber);
    }

    [Server]
    private void RespawnPieces(int pieceNumber)
    {
        Debug.Log("Respawning Piece");
        GameObject piecesInstance = Instantiate(pieces[pieceNumber]);
        piecesInstances.Add(piecesInstance);
        NetworkServer.Spawn(piecesInstance, playerConn[(connCount) % playerConn.Count]);
        if (pieceNumber != 0)
        {
            piecesReciveCount++;
            TurnEnds();
        }
    }

    [Server]
    private void PlayerLeft()
    {
        RpcPlayerLeft();
    }
    [ClientRpc]
    private void RpcPlayerLeft()
    {
        Time.timeScale = 0f;
        OnGamePlayerLeft?.Invoke();
    }
    [ClientRpc]
    private void RpcGameEnds(int winner)
    {
        GameEnds?.Invoke(winner);
        Time.timeScale = 0f;
    }
}
