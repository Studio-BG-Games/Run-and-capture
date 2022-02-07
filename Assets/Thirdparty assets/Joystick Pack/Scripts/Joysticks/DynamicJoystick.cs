using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : Joystick
{
    private float MoveThreshold
    {
        get { return moveThreshold; }
        set { moveThreshold = Mathf.Abs(value); }
    }

    private Vector2 starPos;

    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private float returnDuration = 0.5f;

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        starPos = background.anchoredPosition;
        FadeJoystick(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.DOComplete();
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        FadeJoystick(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        FadeJoystick(false);
        background.DOAnchorPos(starPos, returnDuration);
       
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
}