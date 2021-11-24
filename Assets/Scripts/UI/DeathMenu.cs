using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public static bool playerIsDead = false; 
    [SerializeField] private HealthController playerHealth;
    public int extralife = 1;

    public GameObject deathMenuUI;
    
 /*
    void Update()
    {
        if(playerHealth.currentHealth <= 0 )
        {
            if(playerIsDead)
            {
                //GetExtraLife(extralife);
                //Pause();
            }
            else
            {
                //Pause();
            }
        }
    }
    */

    public void GetExtraLife()
    {
        deathMenuUI.SetActive(false);
        Time.timeScale = 1f;
        playerIsDead = false;
    }

    public void Pause()
    {
        deathMenuUI.SetActive(true);
        Time.timeScale = 0f;
        playerIsDead = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Main Menu");
        SceneManager.LoadScene("MainMenu");
    }

    public void Resurect()
    {
        //extralife -= lifes;
        Debug.Log("Comertial");
        playerHealth.currentHealth = 4200;

    }
}
