using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    [SerializeField] private Transform _startPosition; 

    private void Awake() 
    {
        _startPosition.localScale = new Vector3(0f, 0f, 0f);
    }
    private void FixedUpdate() 
    {
        _startPosition.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }

}
