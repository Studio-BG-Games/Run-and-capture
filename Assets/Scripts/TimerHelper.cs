using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace
{
    public class TimerHelper : MonoBehaviour
    {
        [SerializeField] private float scale;
        private static TimerHelper _instance;

        public static TimerHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Timer").AddComponent<TimerHelper>();
                }

                return _instance;
            }
        }

       [EditorButton]
        public void SetTimerScale()
        {
            Time.timeScale = scale;
        }

        public void StartTimer(Action action, float time)
        {
            StartCoroutine(Timer(action, time));
        }
        public void StartTimer<T>(Action<T> action, float time, T param)
        {
            StartCoroutine(Timer(action, time,  param));
        }
        
        public void StartTimer<T>(List<Action<T>> actions, float time, T param)
        {
            StartCoroutine(Timer(actions, time,  param));
        }
        
        IEnumerator Timer(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
        
        IEnumerator Timer<T>(Action<T> action, float time, T param)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke(param);
        }
        
        IEnumerator Timer<T>(List<Action<T>> actions, float time, T param)
        {
            foreach (var action in actions)
            {
                yield return new WaitForSeconds(time);
                action?.Invoke(param);
            }
        }
    }
}