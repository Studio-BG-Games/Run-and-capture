using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAIAttack : MonoBehaviour
{
    [SerializeField] private AI_BotController _botController;
    [SerializeField] private AttackEnergyController _attackEnergyController;
    private PlayerState _playerState;
    public float attackPlayerDistance = 2f;
     public PlayerState _currentEnemy;

    public BotState botState = BotState.Attack;
    private bool isAttackedOnce = false;


    private void Awake() {
        _botController.botState = botState;
    }
    private void Update() {
        _botController.botState = botState;
        //_currentEnemy = _botController._currentEnemy;
    }


/*
    private bool IsEnemyEnabledForAttack()
    {
        if (isAttackedOnce)
            return false;
        if (botState != BotState.Attack && _attackEnergyController.IsReady())
        {
            foreach (PlayerState enemy in _playerState.enemies)
            {
                if (enemy == null)
                {
                    continue;
                }
                if (!enemy.gameObject.activeSelf)
                {
                    continue;
                }
                foreach (var dir in TileManagment.basicDirections)
                {
                    for (int i = 1; i <= attackPlayerDistance; i++)
                    {
                        TileInfo checkTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, dir, i);
                        if (checkTile == null || checkTile.tileOwnerIndex == TileOwner.Neutral)
                        {
                            break;
                        }
                        if (enemy.currentTile == checkTile)
                        {
                            _currentEnemy = enemy;
                            return true;
                        }
                    }
                }                
            }
        }
        return false;
    }*/
    private bool SetNewBotState(BotState newState)
    {
        if (botState != newState)
        {
            botState = newState;
            return true;
        }
        else 
        {
            return false;
        }
    }

    private IEnumerator CheckBotState(float updateTime)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1f));

        while (true)
        {
            CheckBotState();
            yield return new WaitForSeconds(0);
        }
    }
    
    private void CheckBotState()
    {
        //Debug.Log("CheckState");
        BotState newBotState;

        

            newBotState = BotState.Attack;
        


        SetNewBotState(newBotState);

    }


}

