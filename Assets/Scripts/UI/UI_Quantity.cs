using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quantity : MonoBehaviour
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

    public void UpdateBar(float currentQuantity, float maxQuantity)
    {
        _progressSlider.fillAmount = currentQuantity / maxQuantity;
    }
}
