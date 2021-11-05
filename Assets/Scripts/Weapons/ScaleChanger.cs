using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleChanger : MonoBehaviour
{
    [SerializeField] private Transform _startScale;
    private float minScale = 0;
    private float maxScale = 1f;
    [SerializeField] private float target = 0.1f;
    [SerializeField] private float _setSize = 1f;

    private void Awake() 
    {
        //_startPosition.radius = 0.0f;
        _startScale.localScale = new Vector3(minScale, minScale);
    //    _maxRadius.radius = 0.1f;
    }
    private void Update() 
    {
        for(float i = minScale; i <= _setSize; i++)
        {
            var x = target;
            minScale += x;
            
            _startScale.localScale = new Vector3(minScale, 1f, minScale);
        }
    }
}
