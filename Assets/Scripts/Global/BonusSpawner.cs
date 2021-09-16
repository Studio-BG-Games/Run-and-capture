using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public float minBonusSpawnTime = 5f, maxBonusSpwnTime = 20f;

    public Transform bonusParent;

    public List<GameObject> bounsPrefs;

    public List<BonusObject> activeBonuses = new List<BonusObject>();

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
                BonusObject spawnedBonus = Instantiate(chosenBonus, availableTile.tilePosition, chosenBonus.transform.rotation, bonusParent).GetComponent<BonusObject>();
                spawnedBonus.OnDestroy += RemoveBonus;
                activeBonuses.Add(spawnedBonus);
            }            
            //Debug.Log("spawned");
        }        

    }

    private void RemoveBonus(BonusObject sender)
    {
        sender.OnDestroy -= RemoveBonus;
        activeBonuses.Remove(sender);
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
