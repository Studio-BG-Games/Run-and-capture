using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class RewardExtraLife : MonoBehaviour
{ 
    //ca-app-pub-3940256099942544/5224354917
    private string RewardUnitId = "ca-app-pub-3940256099942544/5224354917"; //ca-app-pub-6397060103571541~3496155288
    private RewardedAd rewardedAd;
    [SerializeField] private Extralife _extraLife;
    private int lifeCount;
    

    private void OnEnable() {
        lifeCount = _extraLife.life;
        this.rewardedAd = new RewardedAd(RewardUnitId);
        AdRequest adRequest = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(adRequest);
        this.rewardedAd.OnUserEarnedReward += HandleEarnedReward;
    }
    private void OnTriggerEnter(Collider other) {

    }

    private void Update() {

        _extraLife.life = lifeCount - 1;
    }

    private void HandleEarnedReward(object sender, Reward e)
    {
        //_extraLife.life ;
        //extralife--;
        //_extraLife.life--;
        lifeCount -= 1;
        _extraLife.life = lifeCount;
    }

    public void ShowAd()
    {
        if(rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
    }

}
