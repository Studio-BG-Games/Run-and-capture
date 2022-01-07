using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    [SerializeField] private GameObject canvasPref;
    public static int lifeCount = 1;
    public int health = 1;

    private void Update() {
        if(health <= 0)
        {
            lifeCount -= 1;   
            Instantiate(canvasPref);//.GetComponent<add>().ShowAd();      
            if(lifeCount < 0)
            {
                lifeCount = 0;
            }   
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
