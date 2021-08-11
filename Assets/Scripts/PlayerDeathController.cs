using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public static List<PlayerState> players = new List<PlayerState>();

    public GameObject deathParticles, resParticles;

    public float resurrectTime = 7f;

    private List<PlayerState> alivePlayers = new List<PlayerState>();
    private List<PlayerState> deadPlayers = new List<PlayerState>();
    private List<float> lastDeadTime = new List<float>();

    private float updateTime = 1f;
    private int spawnSafezone = 1;

    //public static Action<PlayerState> OnPlayerDeath;

    private void Awake()
    {
        var tmpPlayers = FindObjectsOfType<PlayerState>();
        foreach (var player in tmpPlayers)
        {
            players.Add(player);
        }
        TileManagment.players = players;        
    }

    private void Start()
    {
        foreach (var player in players)
        {
            alivePlayers.Add(player);
            lastDeadTime.Add(0f);
        }
        TileManagment.OnAnyTileCaptured += CheckPlayersDeath;

        InvokeRepeating("CheckIfNeedRessurection", 1f, updateTime);
    }

    private void CheckPlayersDeath()
    {
        List<PlayerState> thisIterationDeadPlayers = new List<PlayerState>();
        foreach (var player in alivePlayers)
        {
            var playerTile = TileManagment.GetTile(player.transform.position);
            var myAdjacentTiles = TileManagment.GetAllAdjacentTiles(playerTile, player.ownerIndex);
            if (playerTile.tileOwnerIndex != player.ownerIndex)
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
            }
        }

        foreach (var player in thisIterationDeadPlayers)
        {
            MakeDead(player);
        }
    }

    private void MakeDead(PlayerState player)
    {        
        int playerIndex = players.IndexOf(player);
        lastDeadTime[playerIndex] = Time.time;
        alivePlayers.Remove(player);
        deadPlayers.Add(player);

        //OnPlayerDeath?.Invoke(player);
        PlayerDeadActions(player);
    }

    private void CheckIfNeedRessurection()
    {
        List<PlayerState> needResPlayers = new List<PlayerState>();

        foreach (PlayerState player in deadPlayers)
        {
            int playerIndex = players.IndexOf(player);
            if (Time.time > resurrectTime + lastDeadTime[playerIndex])
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
        alivePlayers.Add(player);
        deadPlayers.Remove(player);
        PlayerResActions(player);
        //player.gameObject.SetActive(true);        
    }

    private void PlayerDeadActions(PlayerState player)
    {
        if (deathParticles)
        {
            Instantiate(deathParticles, player.transform.position, deathParticles.transform.rotation);
        }

        //player.SetNewState(ActionType.Attack, CharacterState.Dead);
        player.OnDeathActions();
        List<TileInfo> playerTiles = TileManagment.charTiles[(int)player.ownerIndex];
        TileInfo currentTile = TileManagment.GetTile(player.transform.position);
        currentTile.canMove = true;
        foreach (TileInfo tile in playerTiles)
        {
            foreach (var enemy in player.enemies)
            {
                tile.easyCaptureFor.Add(enemy.ownerIndex);
            }
        }
        player.gameObject.SetActive(false);
        Debug.Log("player " + player.name + " dead");
    }

    private void PlayerResActions(PlayerState player)
    {

        List<TileInfo> playerTiles = TileManagment.charTiles[(int)player.ownerIndex];

        foreach (TileInfo tile in playerTiles)
        {
            tile.easyCaptureFor.Clear();
        }

        player.gameObject.SetActive(true);
        //player.SetNewState(ActionType.Attack, CharacterState.Idle);
        player.currentTile = GetAvailableResPos(player, playerTiles);
        player.transform.position = player.currentTile.tilePosition;
        player.currentTile.canMove = false;
        //player.transform.position = TileManagment.GetRandomOtherTile(player.ownerIndex).tilePosition;

        //player.SetStartParams();
        Debug.Log("player " + player.name + " res");

        if (resParticles)
        {
            Instantiate(resParticles, player.transform.position, deathParticles.transform.rotation);
        }
    }

    private TileInfo GetAvailableResPos(PlayerState player, List<TileInfo> playerTiles)
    {
        foreach (TileInfo tile in playerTiles)
        {
            if (tile.canMove && tile.tileOwnerIndex == player.ownerIndex)
            {
                var myNeighbourTiles = TileManagment.GetAllAdjacentTiles(tile, player.ownerIndex);
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
        PlayerState player = players[playerIndex];
        MakeDead(player);
    }
}
