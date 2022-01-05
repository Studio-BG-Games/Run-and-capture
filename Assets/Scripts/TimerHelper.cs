using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class TimerHelper : MonoBehaviour
    {
        private static TimerHelper _instance;

        public static TimerHelper Instance => _instance;

        private void Start()
        {
            if (_instance == null)
                _instance = this;
            else
            {
                Destroy(this);
            }
        }

        public void StartTimer(Action action, int time)
        {
            StartCoroutine(Timer(action, time));
        }

        IEnumerator Timer(Action action, int time)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}