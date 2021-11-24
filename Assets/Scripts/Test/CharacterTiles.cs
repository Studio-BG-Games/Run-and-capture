using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTiles : MonoBehaviour
{
    //[SerializeField] private 
    public List<GameObject> myTile ;
    public List<string> neutralList ;
    [SerializeField] private int nCount;
    public List<GameObject> enemyTiles;

    private void Update() {
        //enemyTiles = new List<GameObject>(FindObjectsOfType<GameObject>());
        foreach(GameObject tile in enemyTiles)
        {
            if(gameObject.GetComponent<TileInfo>().tileOwnerIndex == TileOwner.Neutral)
            {
                enemyTiles.Add(FindObjectOfType<GameObject>());
            }
        }
        
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.GetComponent<TileInfo>() != null)
        {
            if((other.gameObject.GetComponent<TileInfo>().tileOwnerIndex == GetComponent<PlayerState>().ownerIndex  
                || other.gameObject.GetComponent<TileInfo>().tileOwnerIndex == TileOwner.Neutral))
            {
                myTile.Add(other.gameObject);
            }
        }
    }
}
