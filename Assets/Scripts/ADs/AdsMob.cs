using System.Collections.Generic;
using System.Linq;
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
    private UnitInfo _player;
    private UnitFactory _factory;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonExit;
    [SerializeField] private GameObject canvas;

    private void OnEnable()
    {
        buttonContinue.onClick.AddListener(Spawn);
        canvas.SetActive(false);
        buttonExit.onClick.AddListener(() =>
        {
            buttonExit.onClick.RemoveAllListeners();
            SceneManager.LoadScene(0);
            Time.timeScale = 1f;
        });
    }

    private void Spawn()
    {
        var player = _player;
        var spawnPos =
            HexManager.CellByColor[UnitColor.Grey].Where(x => x != null).ToList()[
                    Random.Range(0, HexManager.CellByColor[UnitColor.Grey].Count - 1)]
                ;

        _factory.Spawn(player, spawnPos);

        canvas.SetActive(false);
        Time.timeScale = 1f;
    }


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

        foreach (var cell in cells)
        {
            if (cell.Color == UnitColor.Grey)
            {
                var randomCell = Random.Range(0, cells.Count);
                Vector3 respawnPosition = cells[randomCell].transform.position;

                player = FindObjectOfType<ExtraLife>().gameObject;

                player.transform.position = respawnPosition;
                if (player.transform.position == respawnPosition)
                {
                }
            }
        }
    }
}