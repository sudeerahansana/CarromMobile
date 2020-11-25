using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this scripts sits on each individual spawnPoints.Therefore transform=transform of parent object where script is a child
public class PlayerSpawnPoint : MonoBehaviour
{
    private void Awake() => PlayerSpawSystem.AddSpawnPoint(transform);
    private void OnDestroy() => PlayerSpawSystem.RemoveSpawnPoint(transform);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
