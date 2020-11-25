
using UnityEngine;

public class Piviot : MonoBehaviour
{
    public float gizmozSize = .75f;
    public Color gizmoColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color=gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmozSize);
    }
}
