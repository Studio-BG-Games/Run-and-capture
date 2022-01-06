using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider manaBar;

    private int maxMana = 100;
    private int currentMana;
    private WaitForSeconds regenTick = new WaitForSeconds(0.5f);
    private Coroutine regen;


    public static ManaBar instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        currentMana = maxMana;
        manaBar.maxValue = maxMana;
        manaBar.value = maxMana;
    }

    public void UseMana(int amount)
    {
        if(currentMana - amount >= 0 )
        {
            currentMana -= amount;
            manaBar.value = currentMana;

            if(regen != null)
                StopCoroutine(regen);

             regen = StartCoroutine(RegenMana());
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    private IEnumerator RegenMana()
    {
        yield return new WaitForSeconds(0.1f);

        while(currentMana < maxMana)
        {
            currentMana += maxMana / 10;
            manaBar.value = currentMana; 
            yield return regenTick;
        }

        regen = null;
    }

}
