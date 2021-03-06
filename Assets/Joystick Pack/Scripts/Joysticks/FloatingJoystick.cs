using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    public Action OnTouchDown, OnTouchUp;
    
    public bool isPressed { get; set; }
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        isPressed = true;
        OnTouchDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        isPressed = false;
        base.OnPointerUp(eventData);
        OnTouchUp?.Invoke();
    }    
}