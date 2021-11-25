using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleChanger : MonoBehaviour
{
    [SerializeField] private Transform _startScale;
    [SerializeField] private ProjectileController projectile;
    private float minScale = 0;
    private float maxScale = 1.5f;
    [SerializeField] private float target = 0.05f;
    [SerializeField] private float _setSize = 1f;

    private void Awake() 
    {
        //projectile.velocity = 0f;
        //_startPosition.radius = 0.0f;
        _startScale.localScale = new Vector3(minScale, minScale);
    //    _maxRadius.radius = 0.1f;
    }
    private void LateUpdate() 
    {
        for(float i = minScale; i <= _setSize; i++)
        {
            var x = target;
            minScale += x;
            
            _startScale.localScale = new Vector3(minScale,  minScale, minScale);
        }
       
        
    }

    private void FixedUpdate() 
    { 
        var speed = 1.5f;
        projectile.velocity += speed;
    }
}
