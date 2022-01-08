using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor.PackageManager.Requests;
using HexFiled;

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
        List<HexCell> cells = new List<HexCell>(FindObjectsOfType<HexCell>());
        // for (int i = 0; i < cells.Count; i++)
        // {

        // }
        foreach (var cell in cells)
        {
            if(cell.Color == UnitColor.GREY)
            {
                var randomCell = UnityEngine.Random.Range(0, cells.Count);
                Vector3 respawnPosition = cells[randomCell].transform.position;
                player = FindObjectOfType<ExtraLife>().gameObject;
                player.transform.position = respawnPosition;
            }
        }
    }



    private void OnDisable() {
        _ad.OnUserEarnedReward -= HandleUser;
    }
}
