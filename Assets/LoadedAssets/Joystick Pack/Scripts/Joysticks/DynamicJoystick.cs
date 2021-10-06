using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    [SerializeField] private float moveThreshold = 1;
    
    private Vector3 _initPos;
    private Image _bgImage;
    private Image _hndImage;

    private Color _white = Color.white;
    
    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        _bgImage = background.GetComponent<Image>();
        _hndImage = handle.GetComponent<Image>();
        _initPos = background.anchoredPosition;

        SetOpacityJoystick(0.5f);

        //background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        SetOpacityJoystick(1);
        
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        //background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        
        SetOpacityJoystick(0.5f);
        
        background.anchoredPosition = _initPos;
        //background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }

    private void SetOpacityJoystick(float amount)
    {
        _white.a = amount;
        _bgImage.color = _white;
        _hndImage.color = _white;
    }
}