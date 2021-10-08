using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFireMode : MonoBehaviour
{
    [SerializeField] private Sprite[] _fireModesIcons;
    
    private int _fireModeIndx;
    private int _curModeIndx;
    private Image _img;
    private Button _btn;
    
    private void Awake()
    {
        _img = GetComponent<Image>();
        _btn = GetComponent<Button>();
    }

    private void Start()
    {
        _btn.onClick.AddListener(ToggleFireMode);
        _fireModeIndx = PlayerPrefs.GetInt("fireMode");
        _img.sprite = _fireModesIcons[_fireModeIndx];
    }

    private void ToggleFireMode()
    {
        _curModeIndx = _fireModeIndx == 0 ? 1 : 0;
        _img.sprite = _fireModesIcons[_curModeIndx];
        _fireModeIndx = _curModeIndx;
        PlayerPrefs.SetInt("fireMode", _curModeIndx);
    }
}
