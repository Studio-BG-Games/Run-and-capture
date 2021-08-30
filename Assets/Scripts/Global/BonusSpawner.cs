using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public float minBonusSpawnTime = 5f, maxBonusSpwnTime = 20f;

    public List<GameObject> bounsPrefs;

    private int _currentTries = 0, _maxTries = 15;

    private IEnumerator SpawnRandomBonus()
    {
        while (true)
        {
            float bonusSpawnRate = Random.Range(minBonusSpawnTime, maxBonusSpwnTime);
            yield return new WaitForSeconds(bonusSpawnRate);

            List<PlayerState> activePlayers = GameManager.activePlayers;
            if (activePlayers.Count < 1)
            {
                continue;
            }
            var chosenPlayer = activePlayers[Random.Range(0, activePlayers.Count)];
            TileInfo availableTile = GetAvailableTile(chosenPlayer.ownerIndex);

            if (availableTile)
            {
                availableTile.canBuildHere = false;
                int bonusIndex = Random.Range(0, bounsPrefs.Count);
                GameObject chosenBonus = bounsPrefs[bonusIndex];
                Instantiate(chosenBonus, availableTile.tilePosition, chosenBonus.transform.rotation);
            }            
            //Debug.Log("spawned");
        }        

    }

    private void Start()
    {
        StartCoroutine(SpawnRandomBonus());
    }

    private TileInfo GetAvailableTile(TileOwner owner)
    {
        TileInfo availableTile = TileManagment.GetTile(owner);
        if (availableTile.canMove && availableTile.canBuildHere)
        {            
            return availableTile;
        }
        else
        {
            _currentTries++;
            if (_currentTries < _maxTries)
            {
                return GetAvailableTile(owner);
            }
            else
            {
                _currentTries = 0;
                return null;
            }
            
        }
    }
}
