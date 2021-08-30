using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagment : MonoBehaviour
{
    [SerializeField]
    private PlayerState _controllablePlayer;

    [SerializeField]
    private CustomInput _customInput;

    private BonusUI _bonusUIManager;

    private void Awake()
    {
        _bonusUIManager = FindObjectOfType<BonusUI>();

        _bonusUIManager.OnBonusSelected += SetupBonusJoysticks;

        _controllablePlayer.OnDefaultAction += SetupDefault;
    }

    private void SetupDefault()
    {
        _customInput.SetupDeafultControls();
    }

    private void SetupBonusJoysticks(Bonus selectedBonus)
    {
        _customInput.SetupActiveJoystick(selectedBonus.bonusType);
    }
    
}
