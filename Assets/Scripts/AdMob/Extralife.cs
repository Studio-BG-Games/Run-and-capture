using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extralife : MonoBehaviour
{
    public int life = 2;
    [SerializeField] private DeathMenu menu;
    void Update()
    {
        if(life == 0)
        {
            menu.LoadMenu();
        }
    }
}
