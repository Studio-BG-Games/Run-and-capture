using System.IO;
using DefaultNamespace;
using DG.Tweening;
using MainMenu;
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
    [SerializeField] private string dataFilePath;
    private bool _isActive = false;
    private bool _isMusicAllowed = true;
    private bool _isSFXAllowed = true;
    private Settings _settings;
    private Vector3 defailtPosition;

    private void Start()
    {
        dataFilePath = Application.dataPath + dataFilePath;
        
        if(File.Exists(dataFilePath))
            _settings = JsonUtility.FromJson<Settings>(File.ReadAllText(dataFilePath));
        else
        {
            _settings = new Settings(GameData);
            File.WriteAllText(dataFilePath, JsonUtility.ToJson(_settings));
        }
        
        defailtPosition = transform.position;
        SetMenuMusicState();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        musImg.sprite = _settings.isMusicAllowed ? musOnSpr : musOffSpr;
        sfxImg.sprite = _settings.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
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
        _settings.isMusicAllowed = !_settings.isMusicAllowed;
        musImg.sprite = _settings.isMusicAllowed ? musOnSpr : musOffSpr;
        SetMenuMusicState();
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_settings));
    }

    public void OnSFXBtnClick()
    {
        _settings.isSFXAllowed = !_settings.isSFXAllowed;
        sfxImg.sprite = _settings.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_settings));
    }

    private void SetMenuMusicState()
    {
        if (_settings.isMusicAllowed)
        {
            menuMusSRC.Play();
        }
        else
        {
            menuMusSRC.Pause();
        }
    }
}