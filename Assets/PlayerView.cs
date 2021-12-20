using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Action OnStep;
    
    public void Step()
    {
        OnStep?.Invoke();
    }
}
