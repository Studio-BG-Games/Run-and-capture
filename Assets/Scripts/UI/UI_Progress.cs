using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Progress : MonoBehaviour
{
    [SerializeField]   
    private Image _progressSlider;
    [SerializeField]
    private TextMeshProUGUI _progressTypeText;

    private Transform _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main.transform;
    }

    public void UpdateActionType(string actionType)
    {        
        _progressTypeText.text = actionType;
    }

    public void UpdateUI(float currentProgress)
    {
        _progressSlider.fillAmount = currentProgress;
    }

    public void StopUpdateUI()
    {
        _progressSlider.fillAmount = 0f;       
    }

    private void LateUpdate()
    {        
        transform.forward = _mainCam.forward;
    }
}
