using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingSelectionTool : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buildingPrefs;
    [SerializeField]
    private UI_Action _actionUI;
    [SerializeField]
    private Build _buidAction;
    [SerializeField]
    private PlayerActionManager _action;
    [SerializeField]
    private PlayerBonusController _bonusController;
    [SerializeField]
    private GameObject _buildingsPanel;
   

    private GameObject _selectedBuilding;
    private bool _isBuildBtnActivated;

    public Action OnProtectBonusSelected;
    public Action OnAttackBonusSelected;

    public BonusSlot currentSelectedSlot;


    private void Start()
    {
        SetInitialParams();

        _action.OnActionStart += ClearBonusSlot;
    }

    private void ClearBonusSlot(ActionType newAction, CharacterState newState)
    {
        if (newAction == ActionType.Build)
        {
            currentSelectedSlot.ClearSlot();
        }
    }

    private void SetInitialParams()
    {
        _isBuildBtnActivated = false;
    }

    public void OnSelectButtonClick(int index)
    {
        _selectedBuilding = buildingPrefs[index];
        _actionUI.SetSelectedUI(index);
        _buidAction.selectedPref = _selectedBuilding;

        OnProtectBonusSelected?.Invoke();
        _bonusController.currentSelectedBonus = currentSelectedSlot.GetItem();
    }

    public void OnAttackBonusClick(int index)
    {
        OnAttackBonusSelected?.Invoke();
        _bonusController.currentSelectedBonus = currentSelectedSlot.GetItem();
    }
    



    public void OnBuildButtonClick()
    {
        _isBuildBtnActivated = !_isBuildBtnActivated;
        if (_isBuildBtnActivated)
        {
            _buildingsPanel.GetComponent<Animator>().SetTrigger("Show");
        }
        else
        {
            _buildingsPanel.GetComponent<Animator>().SetTrigger("Hide");
        }
    }

}
