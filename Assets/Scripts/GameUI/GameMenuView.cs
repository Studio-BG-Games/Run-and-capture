using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

struct ButtonNListener
{
    private Button _button;
    private UnityAction _action;
    private Vector2 _posToMove;

    public Button Button => _button;
    public UnityAction Action => _action;
    public Vector2 PosToMove => _posToMove;

    public ButtonNListener(Button button, UnityAction action, Vector2 posToMove)
    {
        _button = button;
        _action = action;
        _posToMove = posToMove;
    }
}

public class GameMenuView : MonoBehaviour
{
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite playImage;

    [SerializeField] private Ease ease;
    [SerializeField] private float animDuration;

    private Vector2 _settingsButtonActivePosition;
    private Vector2 _exitButtonActivePosition;
    private List<ButtonNListener> _buttonsToHide;
    private bool toShow = false;

    private void Awake()
    {
        _buttonsToHide = new List<ButtonNListener>()
        {
            new ButtonNListener(settingsButton, ShowSettings, settingsButton.transform.position),
            new ButtonNListener(exitButton, Exit, exitButton.transform.position)
        };

        settingsButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        settingsButton.image.DOFade(0f, 0f);
        exitButton.image.DOFade(0f, 0f);

        settingsButton.transform.position = playPauseButton.transform.position;
        exitButton.transform.position = playPauseButton.transform.position;

        playPauseButton.onClick.AddListener(() =>
        {
            SwitchMenu(!toShow);
            toShow = !toShow;
        });
    }

   

    private void SwitchMenu(bool toShow)
    {
        if (toShow)
        {
            _buttonsToHide.ForEach(x =>
            {
                x.Button.onClick.AddListener(x.Action);
                x.Button.image
                    .DOFade(1f, animDuration)
                    .SetEase(ease);
                x.Button.transform
                    .DOMove(x.PosToMove, animDuration)
                    .SetEase(ease)
                    .OnComplete(() => Time.timeScale = 0f);
                x.Button.gameObject.SetActive(true);
            });

            playPauseButton.image.sprite = playImage;
            
        }
        else
        {
            Time.timeScale = 1f;
            _buttonsToHide.ForEach(x =>
            {
                x.Button.onClick.RemoveAllListeners();
                x.Button.image.DOFade(0f, animDuration).SetEase(ease);
                x.Button.transform
                    .DOMove(playPauseButton.transform.position, animDuration)
                    .SetEase(ease)
                    .OnComplete(() => x.Button.gameObject.SetActive(false));
            });
           
            playPauseButton.image.sprite = pauseImage;
        }
    }

    

    private void Exit()
    {
        Time.timeScale = 1f;
        DOTween.CompleteAll();
        SceneManager.LoadScene(0);
    }

    private void ShowSettings()
    {
    }
}