using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class RewardExtraLife : MonoBehaviour
{
    private string RewardUnitId = "ca-app-pub-3940256099942544/5224354917";
    private RewardedAd rewardedAd;
    [SerializeField] private Extralife _extraLife;
    private int extralife;
    

    private void OnEnable() {
        this.rewardedAd = new RewardedAd(RewardUnitId);
        AdRequest adRequest = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(adRequest);
        this.rewardedAd.OnUserEarnedReward += HandleEarnedReward;
    }

    private void Update() {
        //extralife =_extraLife.life;
    }

    private void HandleEarnedReward(object sender, Reward e)
    {
        extralife = _extraLife.life ;
        //extralife--;
        _extraLife.life--;
    }

    public void ShowAd()
    {
        if(rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
    }

}
