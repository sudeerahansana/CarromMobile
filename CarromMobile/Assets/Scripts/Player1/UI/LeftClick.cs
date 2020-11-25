using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftClick : NetworkBehaviour
{
   
    [SerializeField] private TouchManeger touchManeger=null;

   
   
    public void leftPressed()
    {
        touchManeger.leftPressed();
       
       
    }
}
