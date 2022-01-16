using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace DefaultNamespace
{
    [CustomEditor(typeof(TimerHelper))]
    public class TimerHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var timerHelper = (TimerHelper)target;

            if (GUILayout.Button("SetScale"))
            {
                timerHelper.SetTimerScale();
            }
        }
    }
}
#endif