using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoystickChanger : MonoBehaviour
{
    [SerializeField]
    private BuildingSelectionTool _selectionTool;
    [SerializeField]
    private PlayerActionManager _actionManager;

    [SerializeField]
    private Image joystickBack, joystickHandle;

    [SerializeField]
    private Sprite joystickBackAtt, joystickHandleAtt , joystickBackDef, joystickHandleDef;

    private void Start()
    {
        _selectionTool.OnProtectBonusSelected += ChangeJoystickDefState;
        _actionManager.OnActionStart += BackToNormal;
        _actionManager.OnActionEnd += BackToNormal;
    }

    private void BackToNormal(ActionType arg1, CharacterState arg2)
    {
        joystickBack.sprite = joystickBackAtt;
        joystickHandle.sprite = joystickHandleAtt;
    }

    private void ChangeJoystickDefState()
    {
        joystickBack.sprite = joystickBackDef;
        joystickHandle.sprite = joystickHandleDef;
    }
}
