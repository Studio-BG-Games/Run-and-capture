using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using UnityEngine.Audio;


public class SaySomething : MonoBehaviour
{
    //[SerializeField] private List<HealthController> playerHealths;
    //[SerializeField] private List<Sounds> phrases;
    [SerializeField] private Extralife life;
    //[SerializeField] private GameManager manager;
    [SerializeField] private HealthController health;
    [SerializeField] MainWeapon player;
    [SerializeField] List<SwitchWeapon> Ragnar;
    //[SerializeField] SwitchWeapon Ragnar;

    //[SerializeField] List<AI_BotController> bot;
    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioSource botSource;
    [SerializeField] private List<PlayerState> players;
    private string nameing = "Ragnar(Clone)";
    private int startlives;
    public static float healthCount;
    public static MainWeapon mainWeapon;
    public static SwitchWeapon staticswitch;
    //public static List<AI_BotController> bostatic;

    void Start()
    {
        mainWeapon = player;
        healthCount = health.currentHealth;
        //staticswitch = Ragnar;
        //bostatic = bot;
        //players = new List<PlayerState>(FindObjectsOfType<PlayerState>());

        //startPlayersCount = players.Count;
        startlives = new List<SwitchWeapon>( FindObjectsOfType<SwitchWeapon>()).Count;
        //startlives = Extralife.staticLives;



    }

    // Update is called once per frame

    private void Update() {
        player = FindObjectOfType<MainWeapon>();
        if(player == null)
        {
            //VoiceEnable.isDisable = true;
            //playerSource.Play();
        }
        else
            //VoiceEnable.isDisable = false;
        
        Ragnar = new List<SwitchWeapon>( FindObjectsOfType<SwitchWeapon>());
        //Ragnar = FindObjectOfType<SwitchWeapon>();
        //if(Ragnar.name == nameing)
        //{
            //foreach(SwitchWeapon sw in Ragnar)
            //{
                if(Ragnar.Count < startlives)
                {
                    VoiceEnable.isAiDisabled = true;
                    //playerSource.Play();
                }
                else
                    VoiceEnable.isAiDisabled = false;                
            //}
        //}

            /*
        bot = new List<AI_BotController>( FindObjectsOfType<AI_BotController>());
        foreach(AI_BotController ai in bot)
        {
            if(bot == null)
            {
                VoiceEnable.isAiDisabled = true;
                //playerSource.Play();
            }
            else
                VoiceEnable.isAiDisabled = false;
        }*/
    }

    public void Say()
    {

        playerSource.Play();
        Debug.Log("Play");
    }
}
