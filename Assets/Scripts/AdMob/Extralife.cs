using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extralife : MonoBehaviour
{
    public int life = 1;
    [SerializeField] private DeathMenu menu;
    public static int staticLives;
    public  int staticNoLife;

    private void Awake() {
        staticLives = life;
    }
    
    void Update()
    {
        life = staticLives;
        
    }

    private void OnTriggerEnter(Collider other) {
        if(life < 0)
        {
            //Count.lifesValue -= 1;
            menu.LoadMenu();
        }
    }
}
