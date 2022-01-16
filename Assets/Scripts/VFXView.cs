using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VFXView : MonoBehaviour
    {
        private ParticleSystem _system;
        public Action OnPlayEnd;

        private void Start()
        {
            _system = GetComponent<ParticleSystem>();
            
        }

        private void Update()
        {
            if (_system != null && !_system.IsAlive())
            {
                MusicController.Instance.RemoveAudioSource(gameObject);
                OnPlayEnd?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}