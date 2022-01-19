using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace
{
    public class VFXView : MonoBehaviour
    {
        private ParticleSystem _system;
        public Action OnPlayEnd;
        private Action OnTime;
        private float timeInvoke;

        private void Start()
        {
            _system = GetComponent<ParticleSystem>();
            
        }

        public void OnTimeInvoke(float time, Action action)
        {
            timeInvoke = time;
            OnTime += action;
        }
        
        private void Update()
        {
            if (_system != null && !_system.IsAlive())
            {
                MusicController.Instance.RemoveAudioSource(gameObject);
                OnPlayEnd?.Invoke();
                Destroy(gameObject);
            }

            if (timeInvoke > 0f && Math.Abs(_system.time - timeInvoke) < Time.deltaTime)
            {
                OnTime?.Invoke();
            }
        }
    }
}