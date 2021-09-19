using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;

    private void Start()
    {
        amountText.text = (GameData.gameMaxPlayers - 1).ToString();
    }

    public void OnIncreaseBtnClick()
    {
        GameData.gameMaxPlayers += 1;
        if (GameData.gameMaxPlayers > GameData.possibleMaxPlayers)
        {
            GameData.gameMaxPlayers = GameData.possibleMaxPlayers;
        }

        amountText.text = (GameData.gameMaxPlayers - 1).ToString();
    }

    public void OnDecreaseBtnClick()
    {
        GameData.gameMaxPlayers -= 1;
        if (GameData.gameMaxPlayers < 1)
        {
            GameData.gameMaxPlayers = 1;
        }

        amountText.text = (GameData.gameMaxPlayers - 1).ToString();
    }
}
