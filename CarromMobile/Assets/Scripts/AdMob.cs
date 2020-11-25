using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class AdMob : MonoBehaviour
{
    public static AdMob adMobInstance;
    string App_ID = "ca-app-pub-4853008376724417~4339986524";
    string Interstitial_Ad_ID = "ca-app-pub-3940256099942544/1033173712";
    private InterstitialAd interstitial;

    private void Awake()
    {
        if (adMobInstance == null)
        {
            adMobInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    public void RequestInterstitial()
    {
        this.interstitial = new InterstitialAd(Interstitial_Ad_ID);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
        Debug.Log("Requesting add");
    }

    public void LoadAdd()
    {
        if (this.interstitial.IsLoaded())
        {
            Debug.Log("Add Load");
            this.interstitial.Show();
        }
        Debug.Log("Showing Add");
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        this.interstitial.Destroy();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

  

}
