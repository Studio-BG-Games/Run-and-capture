using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static List<PlayerState> players = new List<PlayerState>();

    public static List<PlayerState> activePlayers = new List<PlayerState>();
    public static List<PlayerState> tempDeadPlayers = new List<PlayerState>();
    public static List<PlayerState> deadPlayers = new List<PlayerState>();

    //public List<PlayerState>playersNOnStatic = new List<PlayerState>();

    public static int coinsPerTree = 50;

    [SerializeField]
    private TextMeshProUGUI coinText;

    private void Awake()
    {
        activePlayers.Clear();
        tempDeadPlayers.Clear();
        deadPlayers.Clear();
        players.Clear();
        DeathChecker.OnPlayerDeath += KillPlayer;
        DeathChecker.OnPlayerRes += ResPlayer;
        DeathChecker.OnPlayerDeathPermanent += DestroyPermanent;
        GameData.OnCoinsCollected += UpdateCoinUI;
        players = FindPlayers();
        SetupActivePlayers(players);        
        //playersNOnStatic = players;
    }

    private void UpdateCoinUI()
    {
        coinText.text = GameData.coins.ToString();
    }

    private void SetupActivePlayers(List<PlayerState> allPlayers)
    {
        foreach (var player in allPlayers)
        {
            activePlayers.Add(player);
        }
    }

    private void KillPlayer(PlayerState deadPlayer)
    {
        activePlayers.Remove(deadPlayer);
        tempDeadPlayers.Add(deadPlayer);
    }

    private void ResPlayer(PlayerState resPlayer)
    {
        activePlayers.Add(resPlayer);
        tempDeadPlayers.Remove(resPlayer);
    }

    private void DestroyPermanent(PlayerState deadPlayer)
    {
        Debug.Log("destroyed perm " + deadPlayer.name);
        activePlayers.Remove(deadPlayer);
        deadPlayers.Add(deadPlayer);
    }

    private List<PlayerState> FindPlayers()
    {
        List<PlayerState> resultPlayerList = new List<PlayerState>();
        var players = FindObjectsOfType<PlayerState>();
        foreach(var player in players)
        {
            resultPlayerList.Add(player);
        }
        return resultPlayerList;
    }
}

public enum TileOwner
{
    Neutral = 0,
    Ariost = 1,
    Ragnar = 2,
    Emir = 3,
    Asvald = 4,
}
