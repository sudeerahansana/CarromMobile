using Mirror;
using Mirror.Examples.Basic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//spawn in the player with this script
public class CameraSpawSystem : NetworkBehaviour
{
    [SerializeField] private GameObject cameraPrefabs = null;
    private static List<Transform> camSpawnPoints = new List<Transform>();
    private int nextCamIndex = 0;

    public static void AddSpawnPoint(Transform transform)   //called by cameraSpawnPoint scricpt awake
    {
        camSpawnPoints.Add(transform);
        camSpawnPoints = camSpawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => camSpawnPoints.Remove(transform);

    public override void OnStartServer()
    {
        NetworkManegerLobby.OnServerReadied += SpawnCamera;
    }



    private void OnDestroy()
    {
        NetworkManegerLobby.OnServerReadied -= SpawnCamera;
        nextCamIndex = 0;
    }

    [Server]
    public void SpawnCamera(NetworkConnection conn)
    {
        nextCamIndex = int.Parse(conn.ToString().Substring(11, 1));
        Transform spawnPoint = camSpawnPoints.ElementAt(nextCamIndex);
        if (spawnPoint == null)
        {
            Debug.LogError($"Missinf spawn points for player {nextCamIndex}");
            return;
        }
        GameObject playerInstance = Instantiate(cameraPrefabs, camSpawnPoints[nextCamIndex].position, camSpawnPoints[nextCamIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);
        
    }

   

}
