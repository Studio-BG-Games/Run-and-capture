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

    private static List<List<GameObject>> charTrees = new List<List<GameObject>>();    

    private static List<Queue<GameObject>> spawningTrees = new List<Queue<GameObject>>();

    private void Start()
    {        
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
        if (tile.canMove && tile.canBuildHere)
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

    }

    private void CheckForPlanting()
    {
        foreach (var player in GameManager.players)
        {
            int numberOfPlayerTrees = charTrees[(int)player.ownerIndex].Count;
            int requirePlayerTrees = Mathf.FloorToInt(treeCoverKoef * TileManagment.charTiles[(int)player.ownerIndex].Count);
            int newTreesForPlanting = requirePlayerTrees - numberOfPlayerTrees - spawningTrees[(int)player.ownerIndex].Count;            
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
        
        StartCoroutine(WaitTillPlant(spawnTime, player));
    }

    private IEnumerator WaitTillPlant(float delay, PlayerState player)
    {
        yield return new WaitForSeconds(delay);
        PlantTreeOnRandomTile(player);
    }

    public static void RemoveTree(GameObject currentTree)
    {
        TileInfo tileWithTree = TileManagment.GetTile(currentTree.transform.position);
        TileManagment.ReleaseTile(tileWithTree);
        Destroy(currentTree);
        GameData.AddCoin(GameManager.coinsPerTree);

        foreach (var list in charTrees)
        {
            if (list.Contains(currentTree))
            {
                list.Remove(currentTree);
                break;
            }
        }
    }
}
