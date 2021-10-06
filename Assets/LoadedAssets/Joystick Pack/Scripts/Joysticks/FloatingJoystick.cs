using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingJoystick : Joystick
{
    //public Action OnTouchDown, OnTouchUp;
    public bool isPressed { get; set; }
    private Vector3 _initPos;
    
    private Image _bgImage;
    private Image _hndImage;

    private Color _white = Color.white;
    
    protected override void Start()
    {
        base.Start();
        //background.gameObject.SetActive(false);
        _bgImage = background.GetComponent<Image>();
        _hndImage = handle.GetComponent<Image>();
        
        _initPos = background.anchoredPosition;

        SetOpacityJoystick(0.5f);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        SetOpacityJoystick(1f);
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        //background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        //OnTouchDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
       // background.gameObject.SetActive(false);
       
        SetOpacityJoystick(0.5f);
        background.anchoredPosition = _initPos;
        base.OnPointerUp(eventData);
        //OnTouchUp?.Invoke();
    }    
    
    private void SetOpacityJoystick(float amount)
    {
        _white.a = amount;
        _bgImage.color = _white;
        _hndImage.color = _white;
    }
}