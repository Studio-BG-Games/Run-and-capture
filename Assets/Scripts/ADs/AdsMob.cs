using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor.PackageManager.Requests;

public class AdsMob : MonoBehaviour
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
        //
    }
    // private void Start() {
    //     ShowAd();
    // }

    private void HandleUser(object sender, Reward reward)
    {
        //ExtraLife life; 
        int lifeCount = ExtraLife.lifeCount;
        ShowAd();
        lifeCount += 1;

    }

    public void ShowAd()
    {
        if (_ad.IsLoaded())
        {
            _ad.Show();
        }
    }

    private void OnDisable() {
        //_ad.OnUserEarnedReward -= HandleUser;
    }
}
