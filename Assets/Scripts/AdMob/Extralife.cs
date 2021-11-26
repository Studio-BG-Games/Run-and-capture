using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extralife : MonoBehaviour
{
    public int life = 1;
    [SerializeField] private DeathMenu menu;
    
/*    void Update()
    {
        if(life == 0)
        {
            //Count.lifesValue -= 1;
            menu.LoadMenu();
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        if(life < 0)
        {
            //Count.lifesValue -= 1;
            menu.LoadMenu();
        }
    }
}
