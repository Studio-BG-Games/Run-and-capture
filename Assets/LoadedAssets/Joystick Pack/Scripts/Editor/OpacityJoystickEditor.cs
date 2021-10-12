using LoadedAssets.Joystick_Pack.Scripts.Joysticks;
using UnityEditor;
using UnityEngine;

namespace LoadedAssets.Joystick_Pack.Scripts.Editor
{
    [CustomEditor(typeof(OpacityJoystick), true)]
    public class OpacityJoystickEditor : JoystickEditor
    {
        private SerializedProperty _idleStateOpacity;
        private SerializedProperty _activeStateOpacity;

        protected override void OnEnable()
        {
            base.OnEnable();
            _idleStateOpacity = serializedObject.FindProperty("_idleStateOpacity");
            _activeStateOpacity = serializedObject.FindProperty("_activeStateOpacity");
        }

        protected override void DrawValues()
        {
            base.DrawValues();
            EditorGUILayout.PropertyField(_idleStateOpacity, new GUIContent("Idle State Opacity", "Joystick opacity when player doesn't touch it."));
            EditorGUILayout.PropertyField(_activeStateOpacity, new GUIContent("Active State Opacity", "Joystick opacity when player touches it."));
        }

    }
}