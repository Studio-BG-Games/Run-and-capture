using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Sprite musOnSpr, musOffSpr, sfxOnSpr, sfxOffSpr;
    [SerializeField] private Image musImg, sfxImg;
    [SerializeField] private AudioSource menuMusSRC;
    private Animator _ac;
    private bool _isActive = false;
    private bool _isMusicAllowed = true;
    private bool _isSFXAllowed = true;

    private void Start()
    {
        _ac = GetComponent<Animator>();
        GameData.LoadSettings();
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
        _ac.SetBool("isActive", _isActive);
    }

    public void OnMusicBtnClick()
    {
        GameData.isMusicAllowed = !GameData.isMusicAllowed;
        musImg.sprite = GameData.isMusicAllowed ? musOnSpr : musOffSpr;
        SetMenuMusicState();
        GameData.SaveSettings();

    }

    public void OnSFXBtnClick()
    {
        GameData.isSFXAllowed = !GameData.isSFXAllowed;
        sfxImg.sprite = GameData.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
        GameData.SaveSettings();
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
