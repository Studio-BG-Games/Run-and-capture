using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartParticles : MonoBehaviour
{
    
    [SerializeField] private Transform _startSize;

     private void Awake() 
     {
         _startSize.localScale = Vector3.zero;
     }

     private void Update() 
     {
         Scale();
     }

    private Vector3 Scale() 
    {
        //_startSize.localScale = Vector3.zero * i;
        for( float i = 0; i <= 0.8f; i++)
        {
             _startSize.localScale += Vector3.one * i * 0.01f * Time.deltaTime;         
        }
        return _startSize.localScale;
    }
}
