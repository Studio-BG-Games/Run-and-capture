using System.IO;
using DefaultNamespace;
using DG.Tweening;
using MainMenu;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = MainMenu.AudioSettings;

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
    private AudioSettings _audioSettings;
    private Vector3 defailtPosition;

    private void Start()
    {
        dataFilePath = Application.dataPath + dataFilePath;
        
        if(File.Exists(dataFilePath))
            _audioSettings = JsonUtility.FromJson<AudioSettings>(File.ReadAllText(dataFilePath));
        else
        {
            _audioSettings = new AudioSettings(GameData);
            File.WriteAllText(dataFilePath, JsonUtility.ToJson(_audioSettings));
        }
        
        defailtPosition = transform.position;
        SetMenuMusicState();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        musImg.sprite = _audioSettings.isMusicAllowed ? musOnSpr : musOffSpr;
        sfxImg.sprite = _audioSettings.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
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
        _audioSettings.isMusicAllowed = !_audioSettings.isMusicAllowed;
        musImg.sprite = _audioSettings.isMusicAllowed ? musOnSpr : musOffSpr;
        SetMenuMusicState();
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_audioSettings));
    }

    public void OnSFXBtnClick()
    {
        _audioSettings.isSFXAllowed = !_audioSettings.isSFXAllowed;
        sfxImg.sprite = _audioSettings.isSFXAllowed ? sfxOnSpr : sfxOffSpr;
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_audioSettings));
    }

    private void SetMenuMusicState()
    {
        if (_audioSettings.isMusicAllowed)
        {
            menuMusSRC.Play();
        }
        else
        {
            menuMusSRC.Pause();
        }
    }
}