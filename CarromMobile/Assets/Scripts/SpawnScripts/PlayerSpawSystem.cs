using Mirror;

using Mirror.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

//every player having a spawn system prefab
public class PlayerSpawSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefabs = null;
    private static List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> playerInstances = new List<GameObject>();
  
    public static event Action<GameObject> OnPlayerSpawn;
    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
            spawnPoints.Add(transform);
            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer()
    {
        NetworkManegerLobby.OnServerReadied += SpawnPlayer;
        Game2PManeger.OnDiskSpawn += SpawnHand;
        Game4Maneger.OnDiskSpawn += SpawnHand;
        NetworkManegerLobby.PassRoomPlayers += GetPlayerList;
    }
   
    private void OnDestroy()
    { 
        NetworkManegerLobby.OnServerReadied -= SpawnPlayer;
        Game2PManeger.OnDiskSpawn -= SpawnHand;
        Game4Maneger.OnDiskSpawn -= SpawnHand;
        NetworkManegerLobby.PassRoomPlayers -= GetPlayerList;
    }

    public void GetPlayerList(List<NetworkConnection> playersConn)
    {
        Invoke("DestroyPlayers", 4.5f);
    }

    private void DestroyPlayers()
    {
        for(int i = 0; i < playerInstances.Count; i++)
        {
            Destroy(playerInstances[i]);
        }
    }
    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        nextIndex = int.Parse(conn.ToString().Substring(11, 1));
        Transform spawnPoint = spawnPoints.ElementAt(nextIndex);
       
        if (spawnPoint == null)
        {
            return;
        }
        GameObject playerInstance = Instantiate(playerPrefabs, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);
        playerInstances.Add(playerInstance);
    }

    [Server]
    public void SpawnHand(NetworkIdentity passedNetId,int index)
    { 
        GameObject playerInstance = Instantiate(playerPrefabs, spawnPoints[index].position, spawnPoints[index].rotation);
        NetworkServer.Spawn(playerInstance, passedNetId.connectionToClient);
        OnPlayerSpawn?.Invoke(playerInstance);
      
    }


}
 