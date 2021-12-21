using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public GameObject charBarCanvas;
    
    public void Step()
    {
        OnStep?.Invoke();
    }

    public void AttackEnd()
    {
        OnAttackEnd?.Invoke();
    }
}
