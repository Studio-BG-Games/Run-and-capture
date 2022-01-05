using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamelogic : MonoBehaviour
{
    Mana manaSystem;

    private void Start() {
        manaSystem = new Mana();
    }

    private void Update() {
        Debug.Log("HP: " + manaSystem.Current_MP);

        manaSystem.Update();
    }
}
public class Mana 
{
    public float Max_MP = 100;
    public float Current_MP = 1;
    List<ManaRegen> regenList;
    public Mana()
    {
        regenList = new List<ManaRegen>();
    }
    public Mana(float max, float current)
    {
        Max_MP = max;
        Current_MP = current;
        regenList = new List<ManaRegen>();
    }
    public void Update() {
        foreach (var regen in regenList.ToArray())
        {
            if(Current_MP < Max_MP && regen.fDuraton > 0)
            {
                Current_MP += regen.fManaPerSecond * Time.deltaTime;
            }
            regen.fDuraton -= 1f *Time.deltaTime; 

            if(regen.fDuraton < 0)
            {
                regenList.RemoveAt(regenList.IndexOf(regen));
            }
        }
    }
}

public class ManaRegen
{
    public float fDuraton;
    public float fManaPerSecond;

    public ManaRegen()
    {

    }
    public ManaRegen(float duration, float manaPerSecond)
    {
        fDuraton = duration;
        fManaPerSecond = manaPerSecond;
    }
}
