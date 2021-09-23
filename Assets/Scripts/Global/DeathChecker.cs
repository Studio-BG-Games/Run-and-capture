using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeathChecker : MonoBehaviour
{

    public GameObject deathParticles, resParticles;
    public GameObject deathBlue_VFX, deathRed_VFX, deathGreen_VFX, deathYellow_VFX;

    public AudioSource deathSrc;

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

    private void CheckPlayersDeath(PlayerState agressor)
    {
        List<PlayerState> thisIterationDeadPlayers = new List<PlayerState>();
        foreach (var player in agressor.enemies)
        {
            if (!GameManager.activePlayers.Contains(player)
                || Vector3.Distance(agressor.transform.position, player.transform.position) > 1.5f * TileManagment.tileOffset)
            {
                continue;
            }
            TileInfo playerTile = player.currentTile;
            if (player.currentTile.canMove)
            {
                playerTile = player.targetMoveTile;
            }
            var myAdjacentTiles = TileManagment.GetOwnerAdjacentTiles(playerTile, player.ownerIndex);
            int canStandTiles = 0;
            foreach (var tile in myAdjacentTiles)
            {
                if (tile.canMove)
                {
                    canStandTiles++;
                }
            }
            if (canStandTiles > 0)
            {
                continue;
            }
            thisIterationDeadPlayers.Add(player);
            Debug.Log("Found " + canStandTiles + " canStand tiles in " + myAdjacentTiles.Count + " adjacent");
        }

        foreach (var player in thisIterationDeadPlayers)
        {
            //MakeDead(player);
            MakeDeadPermanent(player);
        }
    }

    private void SetupLastDeathTimes(List<PlayerState> players)
    {
        foreach (var player in players)
        {
            lastDeathTime.Add(0f);
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
        //lastDeathTime[playerIndex] = Time.time;
        //GameManager.UpdatePlayers(player);

        OnPlayerDeathPermanent?.Invoke(player);
        PlayerDeadActions(player);
    }

    private void Checker()
    {
        CheckFinalDeath();
        //CheckIfNeedRessurection();
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

    private void SpawnPlayerDeathParticles(PlayerState player)
    {
        GameObject deathVFX;
        switch (player.ownerIndex)
        {
            case TileOwner.Ariost:
                deathVFX = deathRed_VFX;
                break;
            case TileOwner.Ragnar:
                deathVFX = deathBlue_VFX;
                break;
            case TileOwner.Asvald:
                deathVFX = deathYellow_VFX;
                break;
            case TileOwner.Emir:
                deathVFX = deathGreen_VFX;
                break;
            default:
                deathVFX = deathParticles;
                break;
        }
        Instantiate(deathVFX, player.transform.position, deathVFX.transform.rotation);
    }

    private void PlayerDeadActions(PlayerState player)
    {
        List<TileInfo> playerTiles = TileManagment.GetCharacterTiles(player);
        TileManagment.SetEasyCapState(playerTiles, true);

        player.SetDead();
        //Debug.Log("player " + player.name + " dead");

        if (deathParticles)
        {
            SpawnPlayerDeathParticles(player);
        }

        if (GameData.isSFXAllowed)
        {
            deathSrc.Play();
        }

        //////////////////////////PLAYER DEATH//////////////
        if (player.ownerIndex == TileOwner.Ariost)
        {
            TileManagment.OnAnyTileCaptured = null;
            TileManagment.OnInitialized = null;
            CustomInput.OnTouchDown = null;
            CustomInput.OnTouchUp = null;
            CharSpawner.OnPlayerSpawned = null;
            StartCoroutine(GoToMenuAfter(3f));
        }
        //////////////////////////PLAYER DEATH//////////////
        CharSpawner.OnPlayerSpawned -= player.ResetEnemies;

        Destroy(player.gameObject); //for test purp
    }

    private IEnumerator GoToMenuAfter(float time)
    {
        yield return new WaitForSeconds(time);
        SceneLoader.LoadScene(0);
    }

    private void PlayerResActions(PlayerState player)
    {

        List<TileInfo> playerTiles = TileManagment.GetCharacterTiles(player);
        TileInfo resTile = GetAvailableResTile(player, playerTiles);
        if (resTile)
        {
            TileManagment.SetEasyCapState(playerTiles, false);            

            player.SetAlive(resTile.tilePosition);
            
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
