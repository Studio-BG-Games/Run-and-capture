using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCollisionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var playerBonusController = other.gameObject.GetComponent<PlayerBonusController>();
        if (playerBonusController)
        {
            var currentBonus = GetComponent<BonusVisuals>().bonus;
            bool bonusPickedUp = playerBonusController.AddBonusToPlayer(currentBonus);
            if (bonusPickedUp)
            {
                Debug.Log("picked up " + gameObject.name);
                Destroy(gameObject);
            }            
        }        
    }
}
