using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeathChecker : MonoBehaviour
{

    public GameObject deathParticles, resParticles;

    public float resurrectTime = 7f;

    public int tilesLeftForDeath = 1;

    private List<float> lastDeathTime = new List<float>();

    private float updateTime = 1f;
    private int spawnSafezone = 1;
    

    public static Action<PlayerState> OnPlayerDeath;
    public static Action<PlayerState> OnPlayerDeathPermanent;
    public static Action<PlayerState> OnPlayerRes;

    private void Awake()
    {
        //TileManagment.OnAnyTileCaptured += CheckPlayersDeath;
    }
    private void Start()
    {       

        SetupLastDeathTimes(GameManager.activePlayers);

        TileManagment.OnAnyTileCaptured += CheckPlayersDeath;

        InvokeRepeating("Checker", 1f, updateTime);
    }

    private void SetupLastDeathTimes(List<PlayerState> players)
    {
        foreach (var player in players)
        {
            lastDeathTime.Add(0f);
        }
    }

    private void CheckPlayersDeath()
    {
        List<PlayerState> thisIterationDeadPlayers = new List<PlayerState>();
        foreach (var player in GameManager.activePlayers)
        {
            var playerTile = player.currentTile;
            var myAdjacentTiles = TileManagment.GetOwnerAdjacentTiles(playerTile, player.ownerIndex);
            int cantStandTilesCounter = 0;
            int tileCounter = 0;
            foreach (var tile in myAdjacentTiles)
            {
                tileCounter++;
                if (!tile.canMove)
                {
                    cantStandTilesCounter++;
                }

            }
            if (cantStandTilesCounter >= myAdjacentTiles.Count)
            {
                thisIterationDeadPlayers.Add(player);
            }
            /*if (playerTile.tileOwnerIndex != player.ownerIndex)
            {

                int cantStandTilesCounter = 0;
                int tileCounter = 0;
                foreach (var tile in myAdjacentTiles)
                {
                    tileCounter++;
                    if (!tile.canMove)
                    {
                        cantStandTilesCounter++;
                    }

                }
                if (cantStandTilesCounter >= myAdjacentTiles.Count)
                {
                    thisIterationDeadPlayers.Add(player);
                }
            }*/
        }

        foreach (var player in thisIterationDeadPlayers)
        {
            MakeDead(player);
        }
    }

    public void MakeDead(PlayerState player)
    {
        int playerIndex = GameManager.players.IndexOf(player);
        lastDeathTime[playerIndex] = Time.time;
        //GameManager.UpdatePlayers(player);

        OnPlayerDeath?.Invoke(player);
        PlayerDeadActions(player);
    }

    public void MakeDeadPermanent(PlayerState player)
    {
        int playerIndex = GameManager.players.IndexOf(player);
        lastDeathTime[playerIndex] = Time.time;
        //GameManager.UpdatePlayers(player);

        OnPlayerDeathPermanent?.Invoke(player);
        PlayerDeadActions(player);
    }

    private void Checker()
    {
        CheckFinalDeath();
        CheckIfNeedRessurection();        
    }

    private void CheckFinalDeath()
    {
        List<PlayerState> thisIterationDeadPlayers = new List<PlayerState>();
        foreach (var player in GameManager.activePlayers)
        {
            int tileIndex = (int)player.ownerIndex;
            var playerTiles = TileManagment.charTiles[tileIndex];
            if (playerTiles.Count <= tilesLeftForDeath)
            {
                thisIterationDeadPlayers.Add(player);
            }
        }

        foreach (var player in thisIterationDeadPlayers)
        {
            MakeDeadPermanent(player);
        }
    }

    private void CheckIfNeedRessurection()
    {
        List<PlayerState> needResPlayers = new List<PlayerState>();

        foreach (PlayerState player in GameManager.tempDeadPlayers)
        {
            int playerIndex = GameManager.players.IndexOf(player);
            if (Time.time > resurrectTime + lastDeathTime[playerIndex])
            {
                needResPlayers.Add(player);
            }
        }
        foreach (PlayerState player in needResPlayers)
        {
            ResPlayer(player);
        }

    }

    private void ResPlayer(PlayerState player)
    {
        OnPlayerRes?.Invoke(player);
        PlayerResActions(player);
    }

    private void PlayerDeadActions(PlayerState player)
    {
        List<TileInfo> playerTiles = TileManagment.GetCharacterTiles(player);
        TileManagment.SetEasyCaptureForAll(playerTiles);

        player.SetDead();
        //Debug.Log("player " + player.name + " dead");

        if (deathParticles)
        {
            Instantiate(deathParticles, player.transform.position, deathParticles.transform.rotation);
        }
    }

    private void PlayerResActions(PlayerState player)
    {

        List<TileInfo> playerTiles = TileManagment.GetCharacterTiles(player);
        TileInfo resTile = GetAvailableResTile(player, playerTiles);
        if (resTile)
        {
            TileManagment.RemoveLockState(playerTiles);
            player.SetAlive(resTile.tilePosition);
            TileManagment.SetCharTilesState(player);
            if (resParticles)
            {
                Instantiate(resParticles, player.transform.position, deathParticles.transform.rotation);
            }
        }
        else 
        {
            OnPlayerDeathPermanent?.Invoke(player);
        }

        //Debug.Log("player " + player.name + " res");

        
    }

    private TileInfo GetAvailableResTile(PlayerState player, List<TileInfo> playerTiles)
    {
        foreach (TileInfo tile in playerTiles)
        {
            if (tile.canMove && tile.tileOwnerIndex == player.ownerIndex)
            {
                var myNeighbourTiles = TileManagment.GetOwnerAdjacentTiles(tile, player.ownerIndex);
                if (myNeighbourTiles.Count >= spawnSafezone)
                {
                    return tile;
                }
            }
        }
        Debug.Log("nowhere to spawn");
        return null;
    }

    public void OnKillBtnClick(int playerIndex)
    {
        PlayerState player = GameManager.players[playerIndex];
        MakeDead(player);
    }
}
