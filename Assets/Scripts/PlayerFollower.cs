using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] GameObject _player;

    private void LateUpdate()
    {
        if (_player == null)
        {
            return;
        }
        transform.position = _player.transform.position;
    }
}
