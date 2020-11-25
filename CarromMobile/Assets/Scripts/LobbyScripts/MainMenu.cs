using Mono.Nat;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mirror;
using System.Collections.Generic;
using Mirror.Examples.Basic;
using System.Collections;

//[System.Serializable]
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerCountPanel=null;
    [SerializeField] private GameObject mainPanel=null;
    [SerializeField] private GameObject ipGameObject=null;
  //  [SerializeField] private GameObject backGroundImage=null;
    [SerializeField] private GameObject logo=null;
    [SerializeField] private PluginController pluginController=null;
    [SerializeField] private GameObject settingsBtn = null;
 
    [Header("UI")]
   // [SerializeField]private Button hostButton=null;
    [SerializeField] private Animator mainUiAnimator=null;
   /* public RectTransform webPanel;
    public RectTransform textCarom;    */ 
    private void OnEnable()
    {
        Debug.Log("On Enable");
        Screen.orientation = ScreenOrientation.Portrait;//setting orientation when start
        PlayerPrefs.SetInt("setUp", 1);
        if (PlayerPrefs.GetInt("setUp") == 1)   //if game was setup previously open hostLobby pannel
        {
            mainUiAnimator.SetBool("MainEnter", true);
            settingsBtn.SetActive(true);
            AdMob.adMobInstance.RequestInterstitial();
        }
        else
        {
            mainUiAnimator.SetBool("MainEnter", false);
        }
    }

    private void Update()
    {    
        if (Input.GetKeyDown(KeyCode.Escape))        //can quit app till with connction error mesege on
        {
          
            if (mainPanel.activeSelf)               //quit from landing page pannel
                Application.Quit();
            else if (playerCountPanel.activeSelf)
            {
                mainUiAnimator.SetInteger("AnimeInt", 1);
            }
            else if (ipGameObject.activeSelf)
            {
                mainUiAnimator.SetInteger("AnimeInt", 3);
               
            }

        }
     
    }
  
    public void StartPanelIdle()
    {
        mainUiAnimator.SetInteger("AnimeInt", 8);
    }
    public void HostLobby()                 //when Host Loby button pressed
    {
          mainUiAnimator.SetInteger("AnimeInt", 2);
    }

    public void twoPButtonAnimePlay()
    {
        mainUiAnimator.SetInteger("AnimeInt", 5);
    }
    public void fourPButtonAnimePlay()
    {
        mainUiAnimator.SetInteger("AnimeInt", 6);
    }
    public void Pressed2p()  //when host lobby button pressed for 2 players
    {
        AdMob.adMobInstance.LoadAdd();
        logo.SetActive(false);
        pluginController.Host2Players();
    
    }
    public void Pressed4p()  //when host lobby button pressed
    {
        AdMob.adMobInstance.LoadAdd();
        logo.SetActive(false);
        pluginController.Host4Players();
      
    }

    public void JoinLobby()
    {
       mainUiAnimator.SetInteger("AnimeInt", 4);
    }

}
