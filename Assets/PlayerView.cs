using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private Animator _animator;
    public Action OnStep;
    
    void Start()
    {
        _animator.GetComponent<Animator>();
    }

    public void Step()
    {
        OnStep?.Invoke();
    }
}
