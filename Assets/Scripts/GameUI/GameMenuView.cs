using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameMenuView : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject menu;
    


    private void Awake()
    {
        menu.SetActive(false);
        SetUpButtons();
    }

    private void SetUpButtons()
    {
        pauseButton.onClick.AddListener(ShowMenu);
        playButton.onClick.AddListener(HideMenu);
        exitButton.onClick.AddListener(Exit);
        settingsButton.onClick.AddListener(ShowSettings);
    }
    
    private void ShowMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }

    private void HideMenu()
    {
        Time.timeScale = 1f;
        menu.SetActive(false);
    }
    private void Exit()
    {
        Time.timeScale = 1f;
        DOTween.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }

    private void ShowSettings()
    {
    }
}