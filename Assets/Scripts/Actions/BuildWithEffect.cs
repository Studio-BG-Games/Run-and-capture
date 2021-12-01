using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWithEffect : MonoBehaviour
{
    public MainWeapon player;
    public ToweHealthController tower;
    public GameObject prefVFX;
    void Start()
    {
        player = FindObjectOfType<MainWeapon>();
        //Instantiate(prefVFX);
    }
    private void OnEnable() {
        player = FindObjectOfType<MainWeapon>();
        if(player != null && player.GetComponent<PlayerState>().ownerIndex == tower.owner)
        Instantiate(prefVFX, player.GetComponentInParent<Transform>());
    }

    private void Update() {
        
    }
}
