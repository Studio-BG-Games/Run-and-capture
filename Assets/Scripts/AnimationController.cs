using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState))]
public class AnimationController : MonoBehaviour
{
    public Animator characterAnimator;

    private PlayerState _playerState;

    private void Start()
    {
        _playerState = GetComponent<PlayerState>();

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
            characterAnimator.SetTrigger(activationTrigger);
        }
    }
    
}
