using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    private void LateUpdate()
    {
        transform.position = _player.transform.position;
    }
}