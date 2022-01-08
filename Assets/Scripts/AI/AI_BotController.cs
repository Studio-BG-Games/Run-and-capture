// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using AI;
// using HexFiled;
// using Items;
//
// public class AI_BotController : MonoBehaviour
// {
//     public bool isAIActive = true;
//
//     public AIManager.BotState botState = AIManager.BotState.Patrol;
//     public Vector2 leftInput, rightInput;
//
//     public float agressiveTime = 5f;
//     public float attackTime = 2f;
//     public float updateBehaviourIn = 1f;
//     public float neutralCapDistance = 6f;
//     public float detectDistance = 4f;
//     public float bonusDetectDistance = 4f;
//     public float attackPlayerDistance = 4f;
//     public float bonusPlaceColdown = 4f;
//     public float protectionDistance = 4f;
//     public Action OnTouchDown, OnTouchUp;
//     public Action OnCurrentPathFinished, OnAttack;
//     public PlayerState _currentEnemy;
//     public HexCell _currentTargetTile;
//
//     private Item _currentBonus;
//
//     public List<HexCell> _currentFollowingPath = new List<HexCell>();
//
//     private PlayerState _playerState;
//     private AttackEnergyController _attackEnergyController;
//     private ActionTriggerSystem _actionTriggerSystem;
//     private PlayerBonusController _playerBonuses;
//     private BonusSpawner _bonusController;
//     private CaptureController _captureController;
//     //private PlayerActionManager _actionManager;
//     //private Attack _attack;
//
//     private Vector3 _startBotPoint;
//
//     private bool isAttackedOnce = false;
//     private bool isUseBonusEnabled = true;
//
//     private int _maxTriesToCalculatePath = 15;
//     private int _triesToCalculatePath = 0;
//
//     private void Awake()
//     {
//         _playerState = GetComponent<PlayerState>();
//         _attackEnergyController = GetComponent<AttackEnergyController>();
//         _actionTriggerSystem = GetComponent<ActionTriggerSystem>();
//         _playerBonuses = GetComponent<PlayerBonusController>();
//         _bonusController = FindObjectOfType<BonusSpawner>();
//         _captureController = GetComponent<CaptureController>();
//
//         _captureController.OnCaptureEnemyTile += TryToSuperJump;
//         _playerState.OnDefaultAction += RemoveCurrentBonus;
//
//         if (isAIActive)
//         {
//             _playerState.OnInitializied += ActivateAI;
//             _playerState.OnCharStateChanged += ChangeBotBehaviour;
//         }
//
//         OnCurrentPathFinished += StartPatrolBehaviour;
//     }
//
//     private void RemoveCurrentBonus()
//     {
//         if (_currentBonus != null && _playerState.prevState == CharacterState.Action)
//         {
//             _playerBonuses.RemoveBonus(_currentBonus);
//             _currentBonus = null;
//         }        
//     }
//
//     private void TryToSuperJump(HexCell current)
//     {
//         //Debug.Log("try SJ");
//         if (!isUseBonusEnabled)
//         {
//             return;
//         }
//         List<HexCell> adjTiles = TileManagment.GetOtherTiles(current, _playerState.ownerIndex);
//         if (adjTiles.Count == 0 || _playerBonuses.attackBonuses.Count == 0)
//         {
//             return;
//         }
//         //Debug.Log("try SJ");
//         HexCell targetTile = adjTiles[UnityEngine.Random.Range(0, adjTiles.Count)];
//         Bonus chosenBonus;
//         chosenBonus = _playerBonuses.attackBonuses[UnityEngine.Random.Range(0, _playerBonuses.attackBonuses.Count)];
//
//         botState = AIManager.BotState.AttackBonusUsage;
//         isUseBonusEnabled = false;
//         _actionTriggerSystem.TriggerAction(targetTile, chosenBonus.bonusAction);
//         _currentBonus = chosenBonus;
//         StartCoroutine(WaitTillBonusReset(chosenBonus.bonusAction.duration + bonusPlaceColdown));
//     }
//
//     private IEnumerator WaitTillBonusReset(float time)
//     {
//         yield return new WaitForSeconds(time);
//         isUseBonusEnabled = true;
//         StartPatrolBehaviour();
//     }
//
//     private void Start()
//     {
//         //ActivateAI();
//
//         /* TileInfo currentTile = _playerState.currentTile;
//         _startBotPoint = currentTile.tilePosition;
//         TileInfo targetPathTile = TileManagment.GetClosestOtherTile(currentTile, _playerState.ownerIndex, _startBotPoint);
//         Debug.Log("current tile "+ currentTile);
//         Debug.Log("target tile "+ targetPathTile);
//         Debug.Log("Calculate path");
//         _currentFollowingPath = Pathfinding.FindPath(currentTile, targetPathTile, TileManagment.levelTiles, TileManagment.tileOffset);*/
//     }
//
//     private void ChangeBotBehaviour(CharacterState newState)
//     {
//         switch (newState)
//         {
//             /*case CharacterState.Attack:
//
//                 break;
//             case CharacterState.Capture:
//
//                 break;*/
//             case CharacterState.Move:
//                 leftInput = Vector2.zero;
//                 return;
//                 /*case CharacterState.Idle:
//
//                     return;
//                 case CharacterState.Build:
//
//                     break;
//                 default:
//                     return;*/
//
//         }
//     }
//
//     private void ActivateAI()
//     {
//         //Debug.Log("Activate AI bot");
//         SetInitialBotParams();
//         StartPatrolBehaviour();
//         StartCoroutine(CheckBotState(updateBehaviourIn));
//     }
//
//
//
//     private void StartPatrolBehaviour() //looking for available tiles, calculating path and set patrol state
//     {
//         //calculate path        
//         HexCell currentTile = _playerState.currentTile;        
//         HexCell targetPathTile = TileManagment.GetClosestOtherTile(currentTile, _playerState.ownerIndex, _startBotPoint);
//         //Debug.Log(targetPathTile + " in pos "+ targetPathTile.tilePosition);
//         _triesToCalculatePath++;
//         if (_triesToCalculatePath < _maxTriesToCalculatePath)
//         {
//             if (!RecalculatePath(currentTile, targetPathTile))
//             {
//                 Debug.Log("recursion");
//                 StartPatrolBehaviour();
//                 return;
//             }            
//         }
//         else
//         {
//             Debug.Log("not found path by max tries");
//
//             targetPathTile = TileManagment.GetRandomOtherTile(_playerState.ownerIndex);
//             RecalculatePath(currentTile, targetPathTile);
//         }
//         if (_currentFollowingPath!=null)
//         {
//             botState = AIManager.BotState.Patrol;
//             _triesToCalculatePath = 0;
//         }
//
//     }
//
//     private void SetInitialBotParams()
//     {
//         _currentEnemy = null;
//         _startBotPoint = _playerState.currentTile.tilePosition;
//         //Debug.Log(_startBotPoint);
//         //botState = BotState.Patrol;
//     }
//
//     private IEnumerator CheckBotState(float updateTime)
//     {
//         yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1f));
//
//         while (true)
//         {
//             CheckBotState();
//             yield return new WaitForSeconds(updateTime);
//         }
//     }
//
//     private void CheckBotState()
//     {
//         //Debug.Log("CheckState");
//         AIManager.BotState newBotState;
//         if (CanPlaceProtectBonus())
//         {
//             newBotState = AIManager.BotState.ProtectBonusUsage;
//         }
//         else if (IsBonusDetected())
//         {
//             newBotState = AIManager.BotState.CollectingBonus;
//             //Debug.Log("CollectBonus");
//         }
//         else if (IsEnemyEnabledForAttack())
//         {
//             newBotState = AIManager.BotState.Attack;
//         }
//         else if (IsEnemyCloseOrRude())
//         {
//             newBotState = AIManager.BotState.Agressive;
//         }
//         else
//         {
//             newBotState = AIManager.BotState.Patrol;
//         }
//
//         SetNewBotState(newBotState);
//         SetBehaviour(newBotState);
//     }
//
//     private bool CanPlaceProtectBonus()
//     {
//         
//         if (botState == AIManager.BotState.ProtectBonusUsage || botState == AIManager.BotState.AttackBonusUsage)
//         {
//             return false;
//         }
//         if (_playerBonuses.protectBonuses.Count == 0 || !isUseBonusEnabled)
//         {
//             return false;
//         }
//         foreach (var enemy in _playerState.enemies)
//         {
//             if (enemy == null)
//             {
//                 continue;
//             }
//             if (!enemy.gameObject.activeSelf)
//             {
//                 continue;
//             }
//             if (Vector3.Distance(_playerState.transform.position, enemy.transform.position) < protectionDistance)
//             {
//                 //Debug.Log("can place");
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     private bool IsBonusDetected()
//     {
//         //Debug.Log("Detect bonus");
//         if (botState == AIManager.BotState.CollectingBonus)
//         {
//             return false;
//         }
//         foreach (var bonusObj in _bonusController.activeBonuses)
//         {
//             if (!_playerBonuses.CanTakeBonus(bonusObj.bonus.bonusType) || bonusObj ==null)
//             {
//                 //Debug.Log("Can`t take or bonus doesn`t exists");
//
//                 continue;
//             }
//             if (Vector3.Distance(bonusObj.transform.position, _playerState.currentTile.tilePosition) < bonusDetectDistance)
//             {
//                 //Debug.Log("All good");
//                 _currentTargetTile = TileManagment.GetTile(bonusObj.transform.position);
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     private bool SetNewBotState(AIManager.BotState newState)
//     {
//         if (botState != newState)
//         {
//             botState = newState;
//             return true;
//         }
//         else 
//         {
//             return false;
//         }
//     }
//
//     private bool IsEnemyEnabledForAttack()
//     {
//         if (isAttackedOnce)
//             return false;
//         if (botState != AIManager.BotState.Attack && _attackEnergyController.IsReady())
//         {
//             foreach (PlayerState enemy in _playerState.enemies)
//             {
//                 if (enemy == null)
//                 {
//                     continue;
//                 }
//                 if (!enemy.gameObject.activeSelf)
//                 {
//                     continue;
//                 }
//                 foreach (var dir in TileManagment.basicDirections)
//                 {
//                     for (int i = 1; i <= attackPlayerDistance; i++)
//                     {
//                         HexCell checkTile = TileManagment.GetTile(_playerState.currentTile.tilePosition, dir, i);
//                         if (checkTile == null || checkTile.tileOwnerIndex == TileOwner.Neutral)
//                         {
//                             break;
//                         }
//                         if (enemy.currentTile == checkTile)
//                         {
//                             _currentEnemy = enemy;
//                             return true;
//                         }
//                     }
//                 }                
//             }
//         }
//         return false;
//     }
//
//     /*private bool IsEnemyEnabledForAttack()
//     {      
//         if (isAttackedOnce)
//             return false;
//         if (botState != BotState.Attack && _attackEnergyController.IsReady())
//         {
//             foreach (PlayerState enemy in _playerState.enemies)
//             {
//                 if (!enemy.gameObject.activeSelf)
//                 {
//                     continue;
//                 }
//                 if (Vector3.Distance(enemy.currentTile.tilePosition, _playerState.currentTile.tilePosition) <= TileManagment.tileOffset * 1.1f)
//                 {
//                                         
//                     _currentEnemy = enemy;
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }*/
//
//     private bool IsEnemyCloseOrRude()
//     {
//         if (isAttackedOnce)
//             return false;
//         if (botState == AIManager.BotState.Patrol && _attackEnergyController.IsReady())
//         {
//             foreach (PlayerState enemy in _playerState.enemies)
//             {
//                 if (enemy == null)
//                 {
//                     continue;
//                 }
//                 if (!enemy.gameObject.activeSelf)
//                 {
//                     continue;
//                 }
//                 if (TileManagment.charTiles[(int)_playerState.ownerIndex].Contains(enemy.currentTile)) //if enemy standing on our tiles
//                 {
//                     
//                     _currentEnemy = enemy;
//                     return true;
//                 }
//                 float distanceToEnemy = Vector3.Distance(enemy.currentTile.tilePosition, _playerState.currentTile.tilePosition);
//                 if (distanceToEnemy < detectDistance) //if enemy is close enough to us we starting to chase him fo some time
//                 {
//                     _currentEnemy = enemy;
//                     StartCoroutine(CalmDown(agressiveTime));
//                     return true;
//                 }                
//                 
//             }
//         }
//         return false;
//     }
//
//     private void SetBehaviour(AIManager.BotState state)
//     {
//         //leftInput = Vector2.zero;
//         switch (state)
//         {
//             case AIManager.BotState.Patrol:
//                 MoveAlongPath();
//                 break;
//             case AIManager.BotState.Agressive:
//                 MoveToEnemy(_currentEnemy);
//                 break;
//             case AIManager.BotState.Attack:
//                 AttackEnemy(_currentEnemy);
//                 break;
//             case AIManager.BotState.CollectingBonus:
//                 MoveToBonus(_currentTargetTile);
//                 break;
//             case AIManager.BotState.ProtectBonusUsage:
//                 PlaceProtectBonus(_playerState.currentTile);
//                 break;
//         }
//     }
//
//     private void PlaceProtectBonus(HexCell currentTile)
//     {
//         //Debug.Log("try protect");
//         Bonus chosenBonus = _playerBonuses.protectBonuses[UnityEngine.Random.Range(0, _playerBonuses.protectBonuses.Count)];
//         List<HexCell> targets = TileManagment.GetOwnerAdjacentTiles(currentTile, _playerState.ownerIndex);
//         HexCell target = targets[UnityEngine.Random.Range(0, targets.Count)];
//         _actionTriggerSystem.TriggerAction(target, chosenBonus.bonusAction);
//         isUseBonusEnabled = false;
//         _currentBonus = chosenBonus;
//         StartCoroutine(WaitTillBonusReset(chosenBonus.bonusAction.duration + bonusPlaceColdown));
//
//     }
//
//     private void Move(HexCell tile)
//     {
//         if (_currentFollowingPath.Count > 0 && tile!=null)
//         {            
//             Vector3 dir3 = tile.tilePosition - _playerState.currentTile.tilePosition;
//             Vector2 dir2 = new Vector2(dir3.x, dir3.z);            
//             leftInput = dir2.normalized;
//         }
//     }
//
//     private void MoveToBonus(HexCell currentTarget)
//     {
//         bool isPathExists = RecalculatePath(_playerState.currentTile, currentTarget);
//         if (!isPathExists)
//         {
//             StartPatrolBehaviour();
//         }
//         MoveAlongPath();
//     }
//
//     private void AttackEnemy(PlayerState currentEnemy)
//     {
//         leftInput = Vector2.zero;
//         _currentFollowingPath.Clear();
//         _actionTriggerSystem.TriggerAction(currentEnemy.currentTile, _playerState.defaultAction);
//         StartPatrolBehaviour();
//     }
//
//     private void MoveToEnemy(PlayerState currentEnemy)
//     {
//         List<HexCell> allAdjacentTilesToTarget = TileManagment.GetAllAdjacentTiles(currentEnemy.currentTile);
//         HexCell adjacentTarget = allAdjacentTilesToTarget[0];
//         foreach (var tile in allAdjacentTilesToTarget)
//         {
//             if (tile.canMove)
//             {
//                 adjacentTarget = tile;
//                 break;
//             }
//         }
//         HexCell currentPos = _playerState.currentTile;
//         RecalculatePath(currentPos, adjacentTarget);
//         if (!RecalculatePath(currentPos, adjacentTarget))
//         {
//             //StartPatrolBehaviour();
//             return;
//         }
//         MoveAlongPath();
//     }
//
//     private bool RecalculatePath(HexCell curentPos, HexCell target)
//     {
//         if (_currentFollowingPath != null)
//         {
//             _currentFollowingPath.Clear();
//         }        
//         _currentFollowingPath = Pathfinding.FindPath(curentPos, target, TileManagment.levelTiles, TileManagment.tileOffset);
//         //Debug.Log("created path to " + target);
//         return _currentFollowingPath != null;
//         
//     }
//
//     private void MoveAlongPath()
//     {
//         //Debug.Log("try to move next point");
//         if (_currentFollowingPath != null) //recalculate existing path or start anew one
//         {
//             if (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex)
//             {
//                 if (_playerState.currentTile == _currentFollowingPath[_currentFollowingPath.Count - 1])
//                 {
//                     //Debug.Log("finished path");
//                     OnCurrentPathFinished?.Invoke();                    
//                     return;
//                 }
//                 var currentTile = _playerState.currentTile;
//                 var endTile = _currentFollowingPath[_currentFollowingPath.Count - 1];
//                 if (!endTile.canMove)
//                 {
//                     endTile = TileManagment.GetClosestOtherTile(currentTile, _playerState.ownerIndex, _startBotPoint);
//                 }
//                 if (RecalculatePath(currentTile, endTile))
//                 {
//                     HexCell nextPathTile = _currentFollowingPath[1];                    
//                     Move(nextPathTile);
//                 }
//             }
//         }
//         else if (_playerState.currentTile.tileOwnerIndex == _playerState.ownerIndex)
//         {
//             //Debug.Log("finished path");
//             OnCurrentPathFinished?.Invoke();
//         }
//
//     }
//
//     private IEnumerator CalmDown(float time)
//     {
//         yield return new WaitForSeconds(time);
//         _currentEnemy = null;        
//     }    
// }
//
//
//
