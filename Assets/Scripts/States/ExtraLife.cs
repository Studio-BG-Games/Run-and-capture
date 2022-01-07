using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    [SerializeField] private GameObject canvasgroupe;
    [SerializeField] private AdsMob canvasPref;
    public static int lifeCount = 1;
    public int health = 1;

    private void Start() {
        
        canvasPref = FindObjectOfType<AdsMob>();
        canvasPref.gameObject.SetActive(false);
    }

    private void Update() {
        if(health <= 0)
        {
            lifeCount -= 1;   
            canvasPref.gameObject.SetActive(true);
            Time.timeScale = 0f;
            //canvasPref.ShowAd();
            //Instantiate(canvasPref);//.GetComponent<add>().ShowAd();      
            if(lifeCount < 0)
            {
                lifeCount = 0;
            }   
        }
        if(lifeCount > 0)
        {
            canvasPref.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

    }

    // public void AddLIfe(int count)
    // {
        
    //     lifeCount = count;
    //     if(count < 1)
    //     {
    //         //add reward = new add();
    //         count++;            
    //     }

    // }

}
