using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    private List<TileInfo> info;
    public TileOwner owner;

    private void Awake() {
        info = new List<TileInfo>( FindObjectsOfType<TileInfo>());
        for (int i = 0; i < info.Count; i++)
        {
            info[i].GetComponent<TileInfo>().tileOwnerIndex = owner;
            //if(GetComponent<TileInfo>().tileOwnerIndex != owner)
            //GetComponent<TileInfo>().tileOwnerIndex = owner;
            //info.Add(tile);
        }
    }

    private void Update() {
        
    }
}
