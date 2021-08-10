using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesSpawner : MonoBehaviour
{
    public float minSpawnRate = 5f, maxSpawnRate = 10f;

    public float updateRate = 1f;

    [Range(0f,0.8f)]
    public float treeCoverKoef = 0.2f;

    public Transform treesParent;

    public List<GameObject> treePrefabs;

    private List<List<GameObject>> charTrees = new List<List<GameObject>>();

    //private List<int> requiredCharTrees;

    private List<Queue<GameObject>> spawningTrees = new List<Queue<GameObject>>();

    private void Start()
    {
        //spawningTrees = new Queue<GameObject>(TileManagment.levelTiles.Count);
        InitCharTrees();
        InvokeRepeating("CheckForPlanting", 0f, updateRate);
    }

    private void InitCharTrees()
    {
        for (int i = 0; i < TileManagment.charTiles.Count; i++)
        {
            List<GameObject> charTreeList = new List<GameObject>();
            charTrees.Add(charTreeList);  //init empty lists for character trees

            Queue<GameObject> charTreeQueue = new Queue<GameObject>(TileManagment.levelTiles.Count);
            spawningTrees.Add(charTreeQueue);
        }
    }

    private void PlantTreeOnRandomTile(PlayerState player)
    {
        int charIndex = (int)player.ownerIndex;
        var treePrefToSpawn = spawningTrees[charIndex].Dequeue();

        int randIndex;
        TileInfo tile;
        Vector3 playerPos = player.transform.position;
        randIndex = Random.Range(0, TileManagment.charTiles[charIndex].Count);
        tile = TileManagment.charTiles[charIndex][randIndex];
        GameObject tree;
        if (tile.canBuildHere)
        {
            if (Vector3.Distance(tile.tilePosition, playerPos) > Mathf.Epsilon)
            {
                tree = Instantiate(treePrefToSpawn, tile.tilePosition, treePrefToSpawn.transform.rotation);
                tree.transform.parent = treesParent;
                TileManagment.AssignBuildingToTile(tile, tree);
                charTrees[charIndex].Add(tree);
            }
            else
            {
                spawningTrees[charIndex].Enqueue(treePrefToSpawn);
                PlantTreeOnRandomTile(player);
                return;
            }
        }
        else
        {
            spawningTrees[charIndex].Enqueue(treePrefToSpawn);
            PlantTreeOnRandomTile(player);
            return;
        }

        //Debug.Log("player pos " + playerPos);
        //Debug.Log("tile pos " + tile.tilePosition);



    }

    private void CheckForPlanting()
    {
        foreach (var player in TileManagment.players)
        {
            int numberOfPlayerTrees = charTrees[(int)player.ownerIndex].Count;
            int requirePlayerTrees = Mathf.FloorToInt(treeCoverKoef * TileManagment.charTiles[(int)player.ownerIndex].Count);
            int newTreesForPlanting = requirePlayerTrees - numberOfPlayerTrees - spawningTrees[(int)player.ownerIndex].Count;
            //Debug.Log("req trees " + requirePlayerTrees);
            //Debug.Log("need to spawn " + newTreesForPlanting);
            for (int i = 0; i < newTreesForPlanting; i++)
            {
                CreatePlantTask(player);
            }
        }
        
    }

    private void CreatePlantTask(PlayerState player)
    {
        int treeRandIndex = Random.Range(0, treePrefabs.Count);        
        float spawnTime = Random.Range(minSpawnRate, maxSpawnRate);
        var tree = treePrefabs[treeRandIndex];
        spawningTrees[(int)player.ownerIndex].Enqueue(tree);
        //Debug.Log("another tree waiting for spawn");
        StartCoroutine(WaitTillPlant(spawnTime, player));
    }

    private IEnumerator WaitTillPlant(float delay, PlayerState player)
    {
        yield return new WaitForSeconds(delay);
        PlantTreeOnRandomTile(player);
    }
}
