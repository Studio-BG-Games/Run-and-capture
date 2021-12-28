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
            if (_system.isStopped)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            MusicController.Instance.RemoveAudioSource(gameObject);
        }
    }
}