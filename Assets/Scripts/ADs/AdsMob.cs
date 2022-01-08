using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEditor.PackageManager.Requests;
using HexFiled;
using Random = UnityEngine.Random;
using Units;
using UnityEngine.UI;

public class AdsMob : MonoBehaviour
{
    private string _revardUnitId = "ca-app-pub-3940256099942544/5224354917";
    private RewardedAd _ad;
    private AdRequest _request;
    private Unit _player;
    [SerializeField] private Button button;
    [SerializeField] private GameObject canvas;

    private void OnEnable()
    {
        _ad = new RewardedAd(_revardUnitId);
        _request = new AdRequest.Builder().Build();
        _ad.LoadAd(_request);
        _ad.OnUserEarnedReward += HandleUser;
        button.onClick.AddListener(() => ShowAd()) ;
        canvas.SetActive(false);
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

        //Respawn(life.gameObject);
        _player.Spawn();

    }

    public void ShowAd()
    {
        //_player = player;
        if (_ad.IsLoaded())
        {
            _ad.Show();
        }
    }

    public void ShowCanvas(Unit player)
    {
        _player = player;
        canvas.SetActive(true);
    }

    public void Respawn(GameObject player)
    {
        List<HexCell> cells = new List<HexCell>();
        cells.AddRange(HexManager.CellByColor[UnitColor.GREY]);
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
