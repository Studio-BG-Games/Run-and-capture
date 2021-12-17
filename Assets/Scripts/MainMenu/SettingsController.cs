using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Sprite musOnSpr, musOffSpr, sfxOnSpr, sfxOffSpr;
    [SerializeField] private Image musImg, sfxImg;
    [SerializeField] private AudioSource menuMusSRC;
    [SerializeField] private GameMenuData GameData;
    [SerializeField] private Transform targetSlideTransform;
    [SerializeField] private float slideTime;
    private bool _isActive = false;
    private bool _isMusicAllowed = true;
    private bool _isSFXAllowed = true;
    private Vector3 defailtPosition;

    private void Start()
    {
        defailtPosition = transform.position;
        SetMenuMusicState();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        musImg.sprite = GameData.isMusicAllowed ? musOnSpr : musOffSpr;
        sfxImg.sprite = GameData.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
    }

    public void OnSettingsBtnClick()
    {
        _isActive = !_isActive;
        SlideSettings();
    }

    private void SlideSettings()
    {
        transform.DOMove(_isActive ? targetSlideTransform.position : defailtPosition, slideTime);
    }

    public void OnMusicBtnClick()
    {
        GameData.isMusicAllowed = !GameData.isMusicAllowed;
        musImg.sprite = GameData.isMusicAllowed ? musOnSpr : musOffSpr;
        SetMenuMusicState();
    }

    public void OnSFXBtnClick()
    {
        GameData.isSFXAllowed = !GameData.isSFXAllowed;
        sfxImg.sprite = GameData.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
    }

    private void SetMenuMusicState()
    {
        if (GameData.isMusicAllowed)
        {
            menuMusSRC.Play();
        }
        else
        {
            menuMusSRC.Pause();
        }
    }
}