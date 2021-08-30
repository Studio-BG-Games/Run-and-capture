using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFinder : MonoBehaviour
{
    public float updateRate = 0.5f;
    [SerializeField]
    private PlayerState _controllablePlayer;

    [SerializeField]
    private GameObject _treeAttackBtn;

    [SerializeField]
    private PlayerAction _treeAttack;
    private ActionTriggerSystem _triggerSystem;

    public static TreeHealthController _targetTree;

    private void Awake()
    {
        //_controllablePlayer.OnCharStateChanged += CheckAdjacentTrees;
        _triggerSystem = _controllablePlayer.GetComponent<ActionTriggerSystem>();

        _treeAttackBtn.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(CheckCoroutine(updateRate));
    }
    private IEnumerator CheckCoroutine(float updateRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRate);
            CheckIfTargetFarFromPLayer();
            CheckAdjacentTrees(_controllablePlayer.currentState);
        }       

    }

    private void CheckAdjacentTrees(CharacterState newState)
    {        
        if (newState == CharacterState.Idle)
        {
            
            int treeCounter = 0;
            List<TileInfo> adjacentTiles = TileManagment.GetAllAdjacentTiles(_controllablePlayer.currentTile);            
            foreach (TileInfo tile in adjacentTiles)
            {
                if (tile.buildingOnTile == null)
                    continue;
                var tree = tile.buildingOnTile.GetComponent<TreeHealthController>();
                if (tree != null)
                {
                    treeCounter++;
                    if (_targetTree == null)
                    {
                        _targetTree = tree;
                        _treeAttackBtn.SetActive(true);
                    }                    
                }

            }
            if (treeCounter == 0)
            {
                _treeAttackBtn.SetActive(false);
                _targetTree = null;
            }
            //Debug.Log(treeCounter);
            
        }

    }

    private void CheckIfTargetFarFromPLayer()
    {
        if (_targetTree == null)
        {
            return;
        }
        float distToTarget = Vector3.Distance(_controllablePlayer.currentTile.tilePosition, _targetTree.transform.position);
        if (distToTarget > 1.5f*TileManagment.tileOffset)
        {
            _targetTree = null;
            _treeAttackBtn.SetActive(false);
        }
    }

    public void OnAttackTreeBtnClick()
    {
        Debug.Log("tree attack");
        if (_targetTree == null)
        {
            return;
        }
        float distToTarget = Vector3.Distance(_controllablePlayer.currentTile.tilePosition, _targetTree.transform.position);
        if (distToTarget > 1.5f*TileManagment.tileOffset)
        {
            return;
        }    
        _controllablePlayer.SetCurrentAction(_treeAttack);
        TileInfo targetTile = TileManagment.GetTile(_targetTree.transform.position);
        _triggerSystem.TriggerAction(targetTile, _controllablePlayer.currentAction);
    }
}
