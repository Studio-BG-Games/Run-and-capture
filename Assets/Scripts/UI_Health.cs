using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Health : MonoBehaviour
{
    [SerializeField]
    private Image _progressSlider;    

    private Transform _mainCam;

    private void Start()
    {
        _mainCam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.forward = _mainCam.forward;
    }

    public void UpdateHealthBar(float currnetHealth, float maxHealth)
    {
        _progressSlider.fillAmount = currnetHealth / maxHealth;
    }
}
