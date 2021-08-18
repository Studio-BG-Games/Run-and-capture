using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public Vector3 tilePosition;

    public bool canMove = true;
    public bool canBeAttacked = true;
    public bool canBuildHere = true;
    public bool isLocked = false;

    public GameObject buildingOnTile;

    public TileOwner tileOwnerIndex = TileOwner.Neutral; //recieved by TileManager on game start
    public TileOwner whoCanEasyGetTile = TileOwner.Neutral;
    public List<TileOwner> easyCaptureFor = new List<TileOwner>();
    public List<TileOwner> checkedFor = new List<TileOwner>();


    public bool isBorderTile = false;
    //public bool isChecked = false;
    
}


