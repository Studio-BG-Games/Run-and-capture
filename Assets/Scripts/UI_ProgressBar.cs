using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_ProgressBar : MonoBehaviour
{
    public GameObject UIPrefab;
    public Transform target;

    public float updateStepInSec = 0.05f;

    private Transform _ui;
    private Image _progressSlider;
    private TextMeshProUGUI _progressText;
    private Transform _cam;

    private IEnumerator _currentCoroutine;

    private CaptureController _captureController;
    private PlayerActionManager _playerActions;
    private PlayerState _playerState;

    private bool _isCaptureUIUpdating;    

    private void Start()
    {
        _cam = Camera.main.transform;

        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {   
            if (c.renderMode == RenderMode.WorldSpace)
            {
                _ui = Instantiate(UIPrefab, c.transform).transform;
                _progressSlider = _ui.GetChild(1).GetChild(0).GetComponent<Image>(); //sorry
                _progressText = _ui.GetChild(0).GetComponent<TextMeshProUGUI>();
                _ui.gameObject.SetActive(false);
                break;
            }
        }
        _captureController = GetComponent<CaptureController>();
        _playerActions = GetComponent<PlayerActionManager>();
        _playerState = GetComponent<PlayerState>();

        _captureController.OnCaptureStart += OnCaptureStart;
        _captureController.OnCaptureEnd += OnCaptureEnd;
        _captureController.OnCaptureFailed += OnCaptureFailed;

        _playerActions.OnActionStart += OnActionStart;
        _playerActions.OnActionSuccess += OnActionStop;

        _playerState.OnDeath += StopUpdate;

    }

    private void OnCaptureFailed()
    {
        if (_isCaptureUIUpdating)
        {
            _isCaptureUIUpdating = false;
            StopUpdate();
        }
    }

    private void OnActionStop()
    {
        StopUpdate();
    }

    private void OnActionStart(ActionType action, CharacterState arg2)
    {
        _progressText.text = action.ToString()+ "..." ;
        _isCaptureUIUpdating = false;
        OnStartUpdate();
    }

    private void OnCaptureEnd(TileInfo arg1, float arg2)
    {
        StopUpdate();
    }

    private void OnCaptureStart()
    {
        _progressText.text = "Capture...";
        _isCaptureUIUpdating = true;
        OnStartUpdate();
    }

    private void StopUpdate()
    {
        _ui.gameObject.SetActive(false);
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    private void OnStartUpdate()
    {
        _ui.gameObject.SetActive(true);
        if (_currentCoroutine == null)
        {
            _currentCoroutine = Updating(updateStepInSec);
            StartCoroutine(_currentCoroutine);
        }        
    }

    private IEnumerator Updating(float timeInterval)
    {
        while (true)
        {
            float currentProgress;
            if (_isCaptureUIUpdating)
            {
                currentProgress = _captureController.GetCaptureProgress();
            }
            else 
            {
                currentProgress = _playerActions.GetProgress();
            }
            SetProgress(currentProgress);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    private void LateUpdate()
    {
        _ui.position = target.position;
        _ui.forward = _cam.forward;
    }

    public void SetProgress(float progress)
    {
        _progressSlider.fillAmount = progress;
    }
}
