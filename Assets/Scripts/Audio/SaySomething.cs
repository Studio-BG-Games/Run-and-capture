using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using UnityEngine.Audio;


public class SaySomething : MonoBehaviour
{
    //[SerializeField] private HealthController health;
    [SerializeField] MainWeapon player;
    [SerializeField] List<SwitchWeapon> enemies;

    //[SerializeField] private List<PlayerState> players;
    private int startPlayersCount;
    public static float healthCount;
    public static MainWeapon mainWeapon;
    public static SwitchWeapon staticswitch;

    void Start()
    {
        mainWeapon = player;
        //healthCount = health.currentHealth;

        startPlayersCount = new List<SwitchWeapon>( FindObjectsOfType<SwitchWeapon>()).Count;
    }



    private void Update() {
        player = FindObjectOfType<MainWeapon>();
        if(player == null)
        {
            //VoiceEnable.isDisable = true;
            //playerSource.Play();
        }
        else
            //VoiceEnable.isDisable = false;
        
        enemies = new List<SwitchWeapon>( FindObjectsOfType<SwitchWeapon>());

        if(enemies.Count < startPlayersCount)
        {
            VoiceEnable.isAiDisabled = true;
                    //playerSource.Play();
        }
        else
            VoiceEnable.isAiDisabled = false;                

    }

    public void Say()
    {

        //playerSource.Play();
        Debug.Log("Play");
    }
}
