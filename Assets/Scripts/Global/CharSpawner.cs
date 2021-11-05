using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharSpawner : MonoBehaviour
{
    //public float minSpawnRate = 5f, maxSpawnRate = 10f;

    public float updateRate = 1f;

    public List<GameObject> charPrefs;

    public static Action OnPlayerSpawned;

    private void Awake()
    {
        //DeathChecker.OnPlayerDeathPermanent += SetupNewPlayerSpawn;
    }

    private void Start()
    {
        SpawStartEnemies();
        InvokeRepeating("CheckForSpawn", 1f, updateRate);
    }

    private void SetupNewPlayerSpawn(/*PlayerState deadPlayer*/)
    {
        TileOwner lastDeadOwnerIndex = GameManager.deadOwners[GameManager.deadOwners.Count-1];
        foreach (var pref in charPrefs)
        {
            var prefIndex = pref.GetComponent<PlayerState>().ownerIndex;
            bool canSpawnThisPref = true;
            if (prefIndex == lastDeadOwnerIndex)
            {
                continue;
            }
            foreach (var enemy in GameManager.activePlayers)
            {
                if (prefIndex == enemy.ownerIndex && 
                gameObject.GetComponent<Crystall>())
                {
                    canSpawnThisPref = false;
                }
            }
            if (canSpawnThisPref)
            {
                Debug.Log("spawn normal");
                //StartCoroutine(SpawnTask(minSpawnRate, maxSpawnRate, pref));
                SpawnEnemy(pref);
                return;
            }

        }
        int oldPlayerIndex = (int)lastDeadOwnerIndex - 2;

        Debug.Log("spawn rest");
        //StartCoroutine(SpawnTask(minSpawnRate, maxSpawnRate, charPrefs[oldPlayerIndex]));
        SpawnEnemy(charPrefs[oldPlayerIndex]);

    }

    private IEnumerator SpawnTask(float minRate, float maxRate, GameObject prefToSpawn)
    {
        float delay = UnityEngine.Random.Range(minRate, maxRate);
        yield return new WaitForSeconds(delay);
        SpawnEnemy(prefToSpawn);
    }

    

    private void SpawnEnemy(GameObject pref)
    {
        int enemyIndex = charPrefs.IndexOf(pref);
        SpawnEnemy(enemyIndex);
    }

    private void SpawnEnemy(int enemyIndex)
    {
        //Debug.Log("try");
        int maxSpawnTries = TileManagment.levelTiles.Count;
        TileInfo targetPos = TileManagment.GetRandomOwnerTile(TileOwner.Neutral);
        List<TileInfo> adjNeutralTiles = TileManagment.GetOwnerAdjacentTiles(targetPos, TileOwner.Neutral); //try random Tile

        if (adjNeutralTiles.Count < 6)
        {
            int startTileIndex = TileManagment.charTiles[(int)TileOwner.Neutral].IndexOf(targetPos);
            for (int i = startTileIndex + 1; i < TileManagment.charTiles[(int)TileOwner.Neutral].Count; i++)
            {
                targetPos = TileManagment.levelTiles[i];
                adjNeutralTiles = TileManagment.GetOwnerAdjacentTiles(targetPos, TileOwner.Neutral);
                if (adjNeutralTiles.Count == 6)
                {
                    break;
                }
            }
        }

        if (adjNeutralTiles.Count < 6)
        {
            int startTileIndex = TileManagment.charTiles[(int)TileOwner.Neutral].IndexOf(targetPos);

            for (int i = startTileIndex - 1; i >0; i--)
            {
                targetPos = TileManagment.levelTiles[i];
                adjNeutralTiles = TileManagment.GetOwnerAdjacentTiles(targetPos, TileOwner.Neutral);
                if (adjNeutralTiles.Count == 6)
                {
                    break;
                }
            }
        }

        if (adjNeutralTiles.Count < 6)
        {
            return;
        }       

        //Debug.Log("inst");
        var enemy = Instantiate(charPrefs[enemyIndex], targetPos.tilePosition, charPrefs[enemyIndex].transform.rotation).GetComponent<PlayerState>();
        TileManagment.ChangeTileOwnerSilent(targetPos, enemy);
        foreach (var tile in adjNeutralTiles)
        {
            TileManagment.ChangeTileOwnerSilent(tile, enemy);
        }
        GameManager.activePlayers.Add(enemy);
        GameManager.players.Add(enemy);
        OnPlayerSpawned?.Invoke();
    }

    private void SpawStartEnemies()
    {
        int numberOfEnemies = GameData.gameMaxPlayers - 1;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy(i);
        }
        /*for (int i = 0; i < TileManagment.charTiles.Count; i++)
        {
            List<GameObject> charTreeList = new List<GameObject>();
            charTrees.Add(charTreeList);  //init empty lists for character trees

            Queue<GameObject> charTreeQueue = new Queue<GameObject>(TileManagment.levelTiles.Count);
            spawningTrees.Add(charTreeQueue);
        }*/
    }    

    private void CheckForSpawn()
    {
        Debug.Log("active players "+ GameManager.activePlayers.Count);
        if (GameManager.activePlayers.Count < GameData.gameMaxPlayers)
        {
            SetupNewPlayerSpawn();
        }        
    }



    
    
}
