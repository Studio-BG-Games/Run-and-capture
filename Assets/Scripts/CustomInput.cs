using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomInput : MonoBehaviour
{
    public static Vector2 leftInput, rightInput;
    public static float rightRegistrationDeadZone = 0.3f;

    public static Action OnTouchDown, OnTouchUp;

    [Header("Character Control Parameters")]
    public CharControlType controlType = CharControlType.UI_Joysticks;    
    [SerializeField]
    private FloatingJoystick leftJoystick, rightJoystick;


    private void Awake()
    {
        rightJoystick.OnTouchDown += InvokeTouchDown;
        rightJoystick.OnTouchUp += InvokeTouchUp;
    }

    private void InvokeTouchDown()
    {
        OnTouchDown?.Invoke();
    }
    private void InvokeTouchUp()
    {
        OnTouchUp?.Invoke();
    }


    private void Start()
    {
        ResetInput();        
    }

    private void ResetInput()
    {
        leftInput = Vector2.zero;
    }

    private void Update()
    {
        leftInput.x = leftJoystick.Horizontal;
        leftInput.y = leftJoystick.Vertical;

        rightInput.x = rightJoystick.Horizontal;
        rightInput.y = rightJoystick.Vertical;

    }
}



public enum CharControlType 
{
    UI_Joysticks,
    //Else //test purp for now
}
