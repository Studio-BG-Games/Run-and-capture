using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictor : MonoBehaviour
{
    [SerializeField] private List<HealthController> players = new List<HealthController>();
    [SerializeField] private List<ToweHealthController> towers = new List<ToweHealthController>();
    [SerializeField] private List<AudioClip> phrases = new List<AudioClip>();
    [SerializeField] private AudioSource sourse;
    private int startCount;


    private void Awake()
    {
        //TalkForTheKill();
        var newPlayers = new List<HealthController>(FindObjectsOfType<HealthController>());
        players.AddRange(newPlayers);
        startCount = players.Count;
    }

    private void TalkForTheKill()
    {
        players.AddRange(FindObjectsOfType<HealthController>());
        foreach (HealthController player in players)
        {
            if (player.currentHealth <= 0)
            {
                CatchPhrase();
                players.Remove(player);

            }
        }
    }

    private void CatchPhrase()
    {
        sourse.volume = sourse.volume - 0.5f;

        int indexDictor = Random.Range(0, phrases.Count);
        sourse.clip = phrases[indexDictor];
        sourse.PlayOneShot(sourse.clip);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(players.Count < startCount)
        {
            CatchPhrase();
            var newPlayers = new List<HealthController>(FindObjectsOfType<HealthController>());
            players.AddRange(newPlayers);            
            //AddNewPlayers();
        }
        else if( players.Count >= startCount + 4)
        {


            TalkForTheKill();
            CatchPhrase();
        }
            
    }

    private void AddNewPlayers()
    {
        players.Add(FindObjectOfType<HealthController>());
    }
}
