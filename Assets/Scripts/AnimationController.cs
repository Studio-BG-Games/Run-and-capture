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

    private void SetNewStateAnimation(CharacterState newState)
    {
        /*if (newState != CharacterState.Action)
        {
            return;
        }*/
        string activationTrigger = "";
        switch(newState)
        {
            case CharacterState.Move:
                activationTrigger = "Move";
                break;
            case CharacterState.Attack:
                activationTrigger = "Attack";
                break;
            case CharacterState.Build:
                activationTrigger = "Build";
                break;
            case CharacterState.TreeAttack:
                activationTrigger = "TreeAttack";
                break;

        }
        if (activationTrigger != "")
        {
            _characterAnimator.SetTrigger(activationTrigger);
        }
    }
    
}
