using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
//using UnityEditor.PackageManager.Requests;

public class add : MonoBehaviour
{
    private string _revardUnitId = "ca-app-pub-3940256099942544/5224354917";
    private RewardedAd _ad;
    private AdRequest _request;

    private void OnEnable()
    {
        _ad = new RewardedAd(_revardUnitId);
        _request = new AdRequest.Builder().Build();
        _ad.LoadAd(_request);
        _ad.OnUserEarnedReward += HandleUser;
        ShowAd();
    }

    private void HandleUser(object sender, Reward reward)
    {
        
    }

    private void ShowAd()
    {
        if (_ad.IsLoaded())
        {
            _ad.Show();
        }
    }
}
