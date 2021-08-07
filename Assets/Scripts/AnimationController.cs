using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState))]
public class AnimationController : MonoBehaviour
{
    private Animator _characterAnimator;

    private PlayerState _playerState;

    private void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _characterAnimator = GetComponentInChildren<Animator>();
        _playerState.OnCharStateChanged += SetNewStateAnimation;
    }

    private void SetNewStateAnimation(CharacterState newState, ActionType currentAction)
    {
        if (newState != CharacterState.Action)
        {
            return;
        }
        string activationTrigger = "";
        switch(currentAction)
        {
            case ActionType.Attack:
                activationTrigger = "Attack";
                break;
            case ActionType.Build:
                activationTrigger = "Build";
                break;

        }
        if (activationTrigger != "")
        {
            _characterAnimator.SetTrigger(activationTrigger);
        }
    }
    
}
