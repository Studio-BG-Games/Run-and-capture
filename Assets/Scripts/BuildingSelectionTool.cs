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
    private GameObject _buildingsPanel;
   

    private GameObject _selectedBuilding;
    private bool _isBuildBtnActivated;

    public Action OnBuildingSelected;


    private void Start()
    {
        SetInitialParams();
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

        OnBuildingSelected?.Invoke();
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
