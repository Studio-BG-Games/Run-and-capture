using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    //public Vector2 position = Vector3.zero;
    public float gCost = 0f;
    public float hCost = 0f;
    public float fCost = 0f;

    public PathNode parent = null;
}
