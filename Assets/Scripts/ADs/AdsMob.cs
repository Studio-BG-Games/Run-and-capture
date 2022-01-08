using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor.PackageManager.Requests;
using HexFiled;
using Random = UnityEngine.Random;

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
        //ShowAd();
        lifeCount += 1;
        ExtraLife.lifeCount = lifeCount;
        ExtraLife life = FindObjectOfType<ExtraLife>();
        life.health += 1;

        Respawn(life.gameObject);

    }

    public void ShowAd()
    {
        if (_ad.IsLoaded())
        {
            _ad.Show();
        }
    }

    public void Respawn(GameObject player)
    {
        List<HexCell> cells = new List<HexCell>();
        //cells.AddRange();
        // for (int i = 0; i < cells.Count; i++)
        // {

        // }
        foreach (var cell in cells)
        {
            if(cell.Color == UnitColor.GREY)
            {
                var randomCell = Random.Range(0, cells.Count);
                Vector3 respawnPosition = cells[randomCell].transform.position;
                //cells[randomCell].Color = UnitColor.YELLOW;
                player = FindObjectOfType<ExtraLife>().gameObject;
                player.transform.position = respawnPosition;
                if(player.transform.position == respawnPosition)
                {
                    //cell.Color = UnitColor.YELLOW;
                }
            }
        }
    }



    private void OnDisable() {
        _ad.OnUserEarnedReward -= HandleUser;
    }
}
