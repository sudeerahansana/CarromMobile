using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;
using Mirror.Websocket;

/// <summary>
/// site on every pieces
/// trigger an event when fall in to hole passing its modified networkIdentity to game manager when this piesce comes still after a collietion
/// </summary>

public class Pieces : NetworkBehaviour
{
    public static event Action<GameObject> EventFall;
    public static event Action<GameObject> RespawnDisplacedPieces;
    public static event Action FinishTurn;
    private Rigidbody piece;

   
    private void OnEnable()
    {
        DiskNetworkTransform.EventHitEnd += CheckForFallenPieces;
        piece=gameObject.GetComponent<Rigidbody>();
    }
    private void OnDisable()
    {
        DiskNetworkTransform.EventHitEnd -= CheckForFallenPieces;
    }

    private void CheckForFallenPieces()
    {
      //  bool destroy = false;
        if (piece.position.y < -0.53f)
        {
            RespawnDisplacedPieces?.Invoke(gameObject);
            Destroy(gameObject);
            //destroy = true;
        }
        else if (piece.position.y < -0.03f)
        {
            Debug.Log("Piece fall");
            EventFall?.Invoke(gameObject);
            Destroy(gameObject);
            //destroy = true;
        }
        else
            FinishTurn?.Invoke();

       /* if (destroy)
            Destroy(gameObject);*/

    }
   
}
