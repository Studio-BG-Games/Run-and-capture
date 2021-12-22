using System;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public Action OnStep;
    public Action OnAttackEnd;
    public Action OnAttack;
    public GameObject charBarCanvas;
    
    private void Step()
    {
        OnStep?.Invoke();
    }

    private void AttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    private void Attack()
    {
        OnAttack?.Invoke();
    }
}
