using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusObject : MonoBehaviour
{
    public Bonus bonus;

    public float aliveTime = 5f;

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

    private void OnTriggerEnter(Collider other)
    {
        var playerBonusController = other.gameObject.GetComponent<PlayerBonusController>();
        if (playerBonusController)
        {
            bool bonusPickedUp = playerBonusController.AddBonusToPlayer(bonus);
            if (bonusPickedUp)
            {
                //Debug.Log("picked up " + gameObject.name);
                var tile = TileManagment.GetTile(transform.position);
                TileManagment.SetTileAvailable(tile);
                Destroy(gameObject);
            }
        }
    }
}
