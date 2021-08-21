using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusVisuals : MonoBehaviour
{
    public Bonus bonus;

    private float aliveTime = 5f;

    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > spawnTime + aliveTime)
        {
            var tile = TileManagment.GetTile(transform.position);
            tile.canBuildHere = true;
            Destroy(gameObject);
        }
    }
}
