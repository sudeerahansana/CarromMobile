using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskSpwnPoints : MonoBehaviour
{
    private void Awake()
    {
        Game2PManeger.AddSpawnPoint(transform);
        Game4Maneger.AddSpawnPoint(transform);

    }
    private void OnDestroy()
    {
        Game2PManeger.RemoveSpawnPoint(transform);
        Game4Maneger.RemoveSpawnPoint(transform);


    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.02f);
     }
}

