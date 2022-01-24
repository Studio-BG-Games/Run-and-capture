using System.IO;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = MainMenu.AudioSettings;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private AudioSource menuMusSrc;
    [SerializeField] private string dataFilePath;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Image musicImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    
    [SerializeField] private Image sfxImage;
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;
    
    private AudioSettings _audioSettings;

    private void Start()
    {
        dataFilePath = Application.persistentDataPath + "/" + dataFilePath;
        if (File.Exists(dataFilePath))
            _audioSettings = JsonUtility.FromJson<AudioSettings>(File.ReadAllText(dataFilePath));
        else
        {
            _audioSettings = new AudioSettings(1f, 1f);
            FileStream stream = new FileStream(dataFilePath, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(JsonUtility.ToJson(_audioSettings));
            stream.Close();
            writer.Close();
        }

        musicSlider.value = _audioSettings.musicVolume;
        sfxSlider.value = _audioSettings.sfxVolume;
        
        musicSlider.onValueChanged.AddListener(x => OnMusicSliderValueChanged());
        sfxSlider.onValueChanged.AddListener(x => OnSFXSliderValueChanged());

        UpdateVisuals();
        SetMenuMusicState();
        gameObject.SetActive(false);
    }

    private void UpdateVisuals()
    {
        musicImage.sprite = _audioSettings.musicVolume == 0f ? musicOffSprite : musicOnSprite;
        
        sfxImage.sprite = _audioSettings.sfxVolume == 0f ? sfxOffSprite : sfxOnSprite;
    }

    private void OnMusicSliderValueChanged()
    {
        _audioSettings.musicVolume = musicSlider.value;
        SetMenuMusicState();
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_audioSettings));
        UpdateVisuals();
    }

    private void OnSFXSliderValueChanged()
    {
        _audioSettings.sfxVolume = sfxSlider.value;
        File.WriteAllText(dataFilePath, JsonUtility.ToJson(_audioSettings));
        UpdateVisuals();
    }

    private void SetMenuMusicState()
    {
        menuMusSrc.volume = musicSlider.value;
    }
}