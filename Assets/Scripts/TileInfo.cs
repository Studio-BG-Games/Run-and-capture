using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public Vector3 tilePosition;

    public bool canMove = true;
    public bool canBeAttacked = true;
    public bool canBuildHere = true;

    public GameObject buildingOnTile;

    public TileOwner tileOwnerIndex = TileOwner.Neutral; //recieved by TileManager on game start
    public TileOwner whoCanEasyGetTile = TileOwner.Neutral;


    public bool isBorderTile = false;
    //public bool isChecked = false;
    
}


