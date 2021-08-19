using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public float minBonusSpawnTime = 5f, maxBonusSpwnTime = 20f;

    public List<GameObject> bounsPrefs;

    private IEnumerator SpawnRandomBonus()
    {
        while (true)
        {
            float bonusSpawnRate = Random.Range(minBonusSpawnTime, maxBonusSpwnTime);
            yield return new WaitForSeconds(bonusSpawnRate);

            List<PlayerState> activePlayers = PlayerDeathController.alivePlayers;
            var chosenPlayer = activePlayers[Random.Range(0, activePlayers.Count)];
            TileInfo availableTile = TileManagment.GetTile(chosenPlayer.ownerIndex);

            int bonusIndex = Random.Range(0, bounsPrefs.Count);
            GameObject chosenBonus = bounsPrefs[bonusIndex];
            Instantiate(chosenBonus, availableTile.tilePosition, chosenBonus.transform.rotation);
            Debug.Log("spawned");
        }        

    }

    private void Start()
    {
        StartCoroutine(SpawnRandomBonus());
    }
}
