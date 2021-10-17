using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Static Fields

    public static List<PlayerState> players = new List<PlayerState>();
    public static List<PlayerState> activePlayers = new List<PlayerState>();
    public static List<PlayerState> tempDeadPlayers = new List<PlayerState>();
    public static List<TileOwner> deadOwners = new List<TileOwner>();
    public static int coinsPerTree = 50;

    #endregion

    [SerializeField] private TextMeshProUGUI _coinText;

    private void Awake()
    {
        activePlayers.Clear();
        tempDeadPlayers.Clear();
        players.Clear();
        DeathChecker.OnPlayerDeath += KillPlayer;
        DeathChecker.OnPlayerRes += ResPlayer;
        DeathChecker.OnPlayerDeathPermanent += DestroyPermanent;
        GameData.OnCoinsCollected += UpdateCoinUI;
        players = FindPlayers();
        SetupActivePlayers(players);
    }

    private void UpdateCoinUI()
    {
        _coinText.text = GameData.coins.ToString();
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
        players.Remove(deadPlayer);
        foreach (var player in activePlayers)
        {
            player.ResetEnemies();
        }

        deadOwners.Add(deadPlayer.ownerIndex);
    }

    private List<PlayerState> FindPlayers()
    {
        List<PlayerState> resultPlayerList = new List<PlayerState>();
        var players = FindObjectsOfType<PlayerState>();
        foreach (var player in players)
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