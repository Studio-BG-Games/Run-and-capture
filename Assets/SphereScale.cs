using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScale : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereRadius;

    private void Update() {
        Radius(0);
    }
    private float Radius(float radius)
    {
        radius = 0f;
        for(float i = 0; i < 1; i++)
        {
            radius += i * Time.deltaTime;
            _sphereRadius.radius = radius;
        }
        return _sphereRadius.radius;
    }
}
