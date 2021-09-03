using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SuperJump : MonoBehaviour
{
    public float updateTime = 0.1f;
    [SerializeField]
    private Material capAllow, capForbid;

    [SerializeField]
    private List<GameObject> _allCapTargets = new List<GameObject>();

    public List<GameObject> _actualTargets = new List<GameObject>();

    public void UpdateUI(TileInfo target)
    {
        transform.LookAt(target.tilePosition);
        _actualTargets = GetActiveCapTiles(GameData.playerLevel);
        StartCoroutine(Checker(updateTime));
    }

    private List<GameObject> GetActiveCapTiles(int playerLevel)
    {
        List<GameObject> actualTargets = new List<GameObject>();
        for (int i = 0; i <= playerLevel; i++)
        {
            GameObject curTargetTile = _allCapTargets[i];
            curTargetTile.SetActive(true);
            Debug.Log(curTargetTile.name);
            actualTargets.Add(curTargetTile);
        }

        return actualTargets;
    }

    private IEnumerator Checker(float updateTime)
    {
        yield return new WaitForSeconds(updateTime);
        CheckPlayerOnTiles(GameManager.activePlayers);
    }

    private void CheckPlayerOnTiles(List<PlayerState> activePlayers)
    {
        List<TileInfo> actualTiles = new List<TileInfo>();
        foreach (var target in _actualTargets)
        {
            actualTiles.Add(TileManagment.GetTile(target.transform.position));
        }

        foreach (var player in activePlayers)
        {
            if (actualTiles.Contains(player.currentTile))
            {
                SetmaterialForObects(capForbid, _actualTargets);
                return;
            }
            
        }
        SetmaterialForObects(capAllow, _actualTargets);
    }

    private void SetmaterialForObects(Material mat, List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Renderer>().material = mat;
        }
            
    }

    public void StopUpdateUI()
    {
        StopAllCoroutines();
        if (_actualTargets == null)
        {
            return;
        }
        foreach (var obj in _actualTargets)
        {
            obj.SetActive(false);
        }
        _actualTargets = null;
    }
}
