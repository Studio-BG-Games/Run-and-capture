using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooser : MonoBehaviour
{
    [SerializeField] private List<LevelModel> _levels;
    [SerializeField] private Image _levelImage;
    [SerializeField] private TextMeshProUGUI _levelNameText;

    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _prevButton;

    public LevelModel CurrentLevel => _levels[_currentLevelIndex];

    private int _currentLevelIndex;

    private void Start()
    {
        _currentLevelIndex = 0;
        UpdateUI();
    }

    public void ShowPreviousLevel()
    {
        _currentLevelIndex--;
        if (_currentLevelIndex < 0)
        {
            _currentLevelIndex = 0;
        }

        UpdateUI();
    }

    public void ShowNextLevel()
    {
        _currentLevelIndex++;
        if (_currentLevelIndex > _levels.Count - 1)
        {
            _currentLevelIndex = _levels.Count - 1;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        _levelImage.sprite = CurrentLevel.MenuSprite;
        _levelNameText.text = CurrentLevel.Name;

        UpdateButtonsVisibility();
    }

    private void UpdateButtonsVisibility()
    {
        bool isFirstLevel = _currentLevelIndex == 0;
        bool isLastLevel = _currentLevelIndex == _levels.Count - 1;
        bool hasMultipleLevels = _levels.Count > 1;

        _prevButton.SetActive(hasMultipleLevels && !isFirstLevel);
        _nextButton.SetActive(hasMultipleLevels && !isLastLevel);
    }
}