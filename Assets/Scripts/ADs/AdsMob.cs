using System.Collections.Generic;
using Chars;
using Data;
using UnityEngine;
// using GoogleMobileAds.Api;
using HexFiled;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdsMob : MonoBehaviour
{
    // private string _revardUnitId = "ca-app-pub-3940256099942544/5224354917";
    // private RewardedAd _ad;
    // private AdRequest _request;
    private UnitInfo _player;
    private UnitFactory _factory;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonExit;
    [SerializeField] private GameObject canvas;

    private void OnEnable()
    {
        // _ad = new RewardedAd(_revardUnitId);
        // _request = new AdRequest.Builder().Build();
        // _ad.LoadAd(_request);
        // _ad.OnUserEarnedReward += HandleUser;
        buttonContinue.onClick.AddListener(Spawn);
        canvas.SetActive(false);
        buttonExit.onClick.AddListener(() =>
        {
            buttonExit.onClick.RemoveAllListeners(); 
            SceneManager.LoadScene(0);
            Time.timeScale = 1f;
        });
        //
    }
    // private void Start() {
    //     ShowAd();
    // }

    // private void HandleUser(object sender, Reward reward)
    // {
    //     
    //     _player.Spawn(HexManager.CellByColor[UnitColor.GREY][Random.Range(0, HexManager.CellByColor[UnitColor.GREY].Count - 1)].coordinates);
    //     canvas.SetActive(false);
    //
    // }

    private void Spawn()
    {
        var player = _player;
        player.spawnPos =
            HexManager.CellByColor[UnitColor.Grey][Random.Range(0, HexManager.CellByColor[UnitColor.Grey].Count - 1)]
                .coordinates;

        _factory.Spawn(player);

        canvas.SetActive(false);
        Time.timeScale = 1f;
        buttonContinue.onClick.RemoveAllListeners();
    }

    // public void ShowAd()
    // {
    //     //_player = player;
    //     if (_ad.IsLoaded())
    //     {
    //         _ad.Show();
    //     }
    // }

    public void ShowCanvas(UnitInfo player, UnitFactory factory)
    {
        _factory = factory;
        _player = player;
        canvas.SetActive(true);
        Time.timeScale = 0f;
        
    }

    public void Respawn(GameObject player)
    {
        List<HexCell> cells = new List<HexCell>();
        cells.AddRange(HexManager.CellByColor[UnitColor.Grey]);
        // for (int i = 0; i < cells.Count; i++)
        // {

        // }
        foreach (var cell in cells)
        {
            if (cell.Color == UnitColor.Grey)
            {
                var randomCell = Random.Range(0, cells.Count);
                Vector3 respawnPosition = cells[randomCell].transform.position;
                //cells[randomCell].Color = UnitColor.YELLOW;
                player = FindObjectOfType<ExtraLife>().gameObject;

                player.transform.position = respawnPosition;
                if (player.transform.position == respawnPosition)
                {
                    //cell.Color = UnitColor.YELLOW;
                }
            }
        }
    }


    // private void OnDisable() {
    //     _ad.OnUserEarnedReward -= HandleUser;
    // }
}