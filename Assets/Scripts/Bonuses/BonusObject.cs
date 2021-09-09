using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusObject : MonoBehaviour
{
    public Bonus bonus;
    public AudioClip collect_SFX;

    public float aliveTime = 8f;

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
                playerBonusController.GetComponent<AudioController>().PlayCollectSound(collect_SFX);
                Destroy(gameObject);
            }
        }
    }
}
