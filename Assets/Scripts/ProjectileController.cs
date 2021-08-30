using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public TileOwner owner = TileOwner.Neutral;
    public float velocity = 10f;
    public float damage = 100f;

    private Rigidbody _rb;

    private bool _isMoving = false;
    private float _tileOffset;
    private TileOwner _enemy = TileOwner.Neutral;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        if (!_isMoving)
            return;
        CheckProjectileState();
    }

    private void CheckProjectileState()
    {
        var currentTile = TileManagment.GetTile(transform.position);
        if (!currentTile)
            return;
        var currentTileOwnerIndex = currentTile.tileOwnerIndex;
        if (currentTileOwnerIndex != owner)
        {
            if (currentTileOwnerIndex == TileOwner.Neutral)
            {
                Destroy(gameObject);
            }
            else
            {
                if (_enemy == TileOwner.Neutral)
                {
                    _enemy = currentTileOwnerIndex;
                    //Debug.Log(_enemy);
                }
                if (currentTileOwnerIndex != _enemy)
                {
                    //Debug.Log("another tile");
                    Destroy(gameObject);
                }
            }
        }        
        
    }

    public void SetinitialParams(TileOwner owner, Vector3 direction, float tileOffset)
    {
        _isMoving = true;
        _rb.velocity = direction.normalized * velocity;
        _tileOffset = tileOffset;
        this.owner = owner;

        transform.LookAt(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collision");
        var healthController = other.gameObject.GetComponent<HealthController>();
        var playerState = other.gameObject.GetComponent<PlayerState>();
        var treeController = other.gameObject.GetComponent<TreeHealthController>();
        if (healthController && owner != playerState.ownerIndex)
        {
            healthController.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (treeController)
        {
            treeController.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
