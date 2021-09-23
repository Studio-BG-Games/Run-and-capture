using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseLevelButtonsTasks : MonoBehaviour
{
    [SerializeField] private Image menuLevelImg;
    [SerializeField] private List<Sprite> menuSprites;
    [SerializeField] private TextMeshProUGUI gameText;

    private List<string> menuText = new List<string> { "MATS", "EMIR" };

    private int  levelsAmount = 2;

    public void OnPrevBtnClick()
    {
        GameData.currentChosenLevel--;
        if (GameData.currentChosenLevel < 1)
        {
            GameData.currentChosenLevel = levelsAmount;
        }

        menuLevelImg.sprite = menuSprites[GameData.currentChosenLevel - 1];
        gameText.text = menuText[GameData.currentChosenLevel - 1];
    }

    public void OnNextBtnClick()
    {
        GameData.currentChosenLevel++;
        if (GameData.currentChosenLevel > levelsAmount)
        {
            GameData.currentChosenLevel = 1;
        }

        menuLevelImg.sprite = menuSprites[GameData.currentChosenLevel - 1];
        gameText.text = menuText[GameData.currentChosenLevel - 1];
    }

    public void OnTestBtnClick()
    {
        SceneLoader.LoadScene(3);
    }

}
