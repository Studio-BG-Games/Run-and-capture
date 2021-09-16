using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomInput : MonoBehaviour
{
    public static Vector2 leftInput, rightInput;
    //public static Vector2 attackActionInput, protectActionInput;
    //public static float rightRegistrationDeadZone = 0.3f;

    public static Action OnTouchDown, OnTouchUp;
   /* public static Action OnDefendTouchDown, OnDefendTouchUp;
    public static Action OnAttackTouchDown, OnAttackTouchUp;*/

    [Header("Character Control Parameters")]
    public CharControlType controlType = CharControlType.UI_Joysticks;    
    [SerializeField]
    private DynamicJoystick leftJoystick;
    [SerializeField]
    private FloatingJoystick rightJoystick;
    [SerializeField]
    private FixedJoystick attackJoystick, defendJoystick;

    private Joystick _currentRightJoystick;


    private void Awake()
    {
        SetupDeafultControls();

        rightJoystick.OnTouchDown += InvokeTouchDown;
        rightJoystick.OnTouchUp += InvokeTouchUp;

        attackJoystick.OnTouchDown += InvokeTouchDown;
        attackJoystick.OnTouchUp += InvokeTouchUp;

        defendJoystick.OnTouchDown += InvokeTouchDown;
        defendJoystick.OnTouchUp += InvokeTouchUp;
    }

    public void SetupDeafultControls()
    {
        rightJoystick.gameObject.SetActive(true);
        defendJoystick.gameObject.SetActive(false);
        attackJoystick.gameObject.SetActive(false);
        _currentRightJoystick = rightJoystick;
    }

    public void SetupActiveJoystick(BonusType bonusType)
    {
        Joystick newActiveJoystick;
        if (bonusType == BonusType.Attack)
        {
            newActiveJoystick = attackJoystick;
        }
        else
        {
            newActiveJoystick = defendJoystick;
        }
        rightJoystick.gameObject.SetActive(false);
        defendJoystick.gameObject.SetActive(false);
        attackJoystick.gameObject.SetActive(false);

        newActiveJoystick.gameObject.SetActive(true);
        _currentRightJoystick = newActiveJoystick;
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

        rightInput.x = _currentRightJoystick.Horizontal;
        rightInput.y = _currentRightJoystick.Vertical;

    }
}



public enum CharControlType 
{
    UI_Joysticks,
    //Else //test purp for now
}
