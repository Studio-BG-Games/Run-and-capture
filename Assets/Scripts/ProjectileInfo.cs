using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInfo : MonoBehaviour
{
    public TileOwner owner = TileOwner.Neutral;
    public float velocity = 10f;
    public float damage = 100f;

    public GameObject playerImpactVFX, groundImpactVFX;
    private Rigidbody _rb;

    private bool _isMoving = false;
    private float _tileOffset;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
       
    }

    private void Update()
    {
        if (!_isMoving)
            return;
        CheckProjectileState();
    }

    private void CheckProjectileState()
    {
        //check if need delete;
    }

    public void SetinitialParams(TileOwner owner, Vector3 direction, float tileOffset)
    {
        _isMoving = true;
        _rb.velocity = direction.normalized * velocity;
        _tileOffset = tileOffset;
        this.owner = owner;

        transform.LookAt(direction);
    }
}
