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

    private List<int> requiredCharTrees;

    private Queue<GameObject> spawningTrees;

    private void Start()
    {
        spawningTrees = new Queue<GameObject>(TileManagment.levelTiles.Count);
        InvokeRepeating("CheckForPlanting", 0f, updateRate);
    }

    private void PlantTreeOnRandomTile()
    {
        /*var treePrefToSpawn = spawningTrees.Dequeue();

        int randIndex;
        TileInfo tile;
        Vector3 playerPos = FindObjectOfType<PlayerState>().transform.position;
        randIndex = Random.Range(0, TileManagment.ariostTiles.Count);
        tile = TileManagment.ariostTiles[randIndex];
        GameObject tree;
        if (tile.canBuildHere)
        {
            if (Vector3.Distance(tile.tilePosition, playerPos) > Mathf.Epsilon)
            {
                tree = Instantiate(treePrefToSpawn, tile.tilePosition, treePrefToSpawn.transform.rotation);
                tree.transform.parent = treesParent;
                TileManagment.AssignBuildingToTile(tile, tree);
                ariostTrees.Add(tree);
            }
            else
            {
                spawningTrees.Enqueue(treePrefToSpawn);
                PlantTreeOnRandomTile();
                return;
            }
        }
        else
        {
            spawningTrees.Enqueue(treePrefToSpawn);
            PlantTreeOnRandomTile();
            return;
        }        */

        //Debug.Log("player pos " + playerPos);
        //Debug.Log("tile pos " + tile.tilePosition);


        
    }

    private void CheckForPlanting()
    {
        /*foreach()
        requiredAriostTrees = Mathf.FloorToInt(treeCoverKoef * TileManagment.ariostTiles.Count);
        int newTreesForPlanting = requiredAriostTrees - ariostTrees.Count - spawningTrees.Count;
        //Debug.Log("need to spawn " + newTreesForPlanting);
        //Debug.Log("Waiting for spawn " + spawningTrees.Count + " trees");

        for (int i = 0; i < newTreesForPlanting; i++)
        {
            CreatePlantTask();
        }*/
    }

    private void CreatePlantTask()
    {
        int treeRandIndex = Random.Range(0, treePrefabs.Count);
        //int tileRandIndex = Random.Range(0, treePrefabs.Count);
        float spawnTime = Random.Range(minSpawnRate, maxSpawnRate);
        var tree = treePrefabs[treeRandIndex];
        spawningTrees.Enqueue(tree);
        //Debug.Log("another tree waiting for spawn");
        StartCoroutine(WaitTillPlant(spawnTime));
    }

    private IEnumerator WaitTillPlant(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlantTreeOnRandomTile();
    }
}
