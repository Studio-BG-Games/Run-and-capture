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
    public List<TileOwner> easyCaptureFor = new List<TileOwner>();
    public List<TileOwner> checkedFor = new List<TileOwner>();


    public bool isBorderTile = false;
    public bool isLocked = false;

    #region Pathfinding values
    [Header("Pathfinding Settings")]
    public float gCost = 0f;
    public float hCost = 0f;
    public float fCost = 0f;

    public TileInfo parent = null;

    #endregion

}


