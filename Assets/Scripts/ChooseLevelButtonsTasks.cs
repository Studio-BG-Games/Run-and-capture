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

    [SerializeField] private GameObject btnNext, btnPrev;

    private List<string> menuText = new List<string> { "MATS", "EMIR" };

    private int  levelsAmount = 2;

    private void Start()
    {
        btnNext.SetActive(true);
        btnPrev.SetActive(false);
    }

    public void OnPrevBtnClick()
    {
        GameData.currentChosenLevel--;
        if (GameData.currentChosenLevel < 1)
        {
            GameData.currentChosenLevel = levelsAmount;
        }

        menuLevelImg.sprite = menuSprites[GameData.currentChosenLevel - 1];
        gameText.text = menuText[GameData.currentChosenLevel - 1];

        btnNext.SetActive(true);
        btnPrev.SetActive(false);
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

        btnNext.SetActive(false);
        btnPrev.SetActive(true);
    }

    public void OnTestBtnClick()
    {
        SceneLoader.LoadScene(3);
    }

}
