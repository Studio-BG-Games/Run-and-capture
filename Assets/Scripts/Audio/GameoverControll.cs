using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverControll : MonoBehaviour
{
    public int count = 0;
    public AudioSource source;
    private void Update() {
        count = Extralife.staticLives;
        if(Extralife.staticLives < 0)
        {
            Gameover.disable = false;
            source.playOnAwake = source.clip;
            
        }
        else
        {
            Gameover.disable = true;
        }

    }
}
