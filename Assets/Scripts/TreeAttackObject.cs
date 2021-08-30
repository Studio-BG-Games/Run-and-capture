using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAttackObject : MonoBehaviour
{
    // Start is called before the first frame update
    private float liveTime = 2f;
    void Start()
    {
        Destroy(gameObject, liveTime);
    }    
}
