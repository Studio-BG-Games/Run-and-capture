using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VFXView : MonoBehaviour
    {
        private ParticleSystem _system;

        private void Start()
        {
            _system = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (_system != null && _system.isStopped)
            {
                MusicController.Instance.RemoveAudioSource(gameObject);
                Destroy(gameObject);
            }
        }
    }
}