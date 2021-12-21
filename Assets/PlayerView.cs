using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Action OnStep;
    public GameObject charBarCanvas;
    
    public void Step()
    {
        OnStep?.Invoke();
    }
}
