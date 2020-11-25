
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Types;
using Mirror.Examples.Basic;


//[RequireComponent(typeof(PlayerManeger))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToEnable=null;

   /* private void OnEnable()
    {
        Physics.defaultContactOffset = 0.0001f;
    }
   
    private void OnDestroy()
    {
        Physics.defaultContactOffset = 0.0001f;

    }*/
    public override void OnStartAuthority()
    {
        EnableComponenets();
        enabled = true;
       
    }

    public void EnableComponenets()
    {
        if (!hasAuthority)                                //if this condition haven't checked thire vrssion of player in our self will be activated
            return;
        for (int i = 0; i < componentsToEnable.Length; i++)
        {
            if (i > componentsToEnable.Length - 2)
            {
                if (!componentsToEnable[i].gameObject.activeSelf)
                    componentsToEnable[i].gameObject.SetActive(true);
            }
            else
            {
                if (!componentsToEnable[i].isActiveAndEnabled)
                    componentsToEnable[i].enabled = true;
            }

        }
    }

}