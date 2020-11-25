
using UnityEngine;
using Mirror;
using System;


//[RequireComponent(typeof(PlayerManeger))]
public class CameraSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToEnableCam=null;
    public static event Action OnCamEnable;
    
    public override void OnStartAuthority()
    {
        EnableComponenetsCam();
    }
    public void EnableComponenetsCam()
    {
        for (int i = 0; i < componentsToEnableCam.Length; i++)
        {
          componentsToEnableCam[i].enabled = true;
               
        }
        OnCamEnable?.Invoke();
    }
}
   