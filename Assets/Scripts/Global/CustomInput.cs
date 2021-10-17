using System;
using UnityEngine;

public class CustomInput : MonoBehaviour
{
    #region Static fields

    public static Vector2 leftInput;
    public static Vector2 rightInput;
    public static Action OnTouchDown;
    public static Action OnTouchUp;

    #endregion

    [Header("Character Control Parameters")] 
    
    [SerializeField] private DynamicJoystick _leftJoystick;
    [SerializeField] private FloatingJoystick _rightJoystick;
    [SerializeField] private FixedJoystick _attackJoystick;
    [SerializeField] private FixedJoystick _defendJoystick;

    private Joystick _currentRightJoystick;


    private void Awake()
    {
        SetupDefaultControls();

        _rightJoystick.OnTouchDown += InvokeTouchDown;
        _rightJoystick.OnTouchUp += InvokeTouchUp;

        _attackJoystick.OnTouchDown += InvokeTouchDown;
        _attackJoystick.OnTouchUp += InvokeTouchUp;

        _defendJoystick.OnTouchDown += InvokeTouchDown;
        _defendJoystick.OnTouchUp += InvokeTouchUp;
    }
    
    private void Start()
    {
        ResetInput();
    }

    private void Update()
    {
        leftInput.x = _leftJoystick.Horizontal;
        leftInput.y = _leftJoystick.Vertical;

        rightInput.x = _currentRightJoystick.Horizontal;
        rightInput.y = _currentRightJoystick.Vertical;
    }

    public void SetupDefaultControls()
    {
        _rightJoystick.gameObject.SetActive(true);
        _defendJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);
        _currentRightJoystick = _rightJoystick;
    }

    public void SetupActiveJoystick(BonusType bonusType)
    {
        Joystick newActiveJoystick;
        if (bonusType == BonusType.Attack)
        {
            newActiveJoystick = _attackJoystick;
        }
        else
        {
            newActiveJoystick = _defendJoystick;
        }

        _rightJoystick.gameObject.SetActive(false);
        _defendJoystick.gameObject.SetActive(false);
        _attackJoystick.gameObject.SetActive(false);

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

    private void ResetInput()
    {
        leftInput = Vector2.zero;
    }

}