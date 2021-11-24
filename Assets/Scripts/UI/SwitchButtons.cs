using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchButtons : MonoBehaviour
{
    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;

    // Update is called once per frame
    void Update()
    {
        if(button2 != null)
        {
            button1.SetActive(true);
            button2.SetActive(false);
        }
        else
        {
            button1.SetActive(false);
            button2.SetActive(true);
        }
    }
}
