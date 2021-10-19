using LoadedAssets.Joystick_Pack.Scripts.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatingJoystick))]
public class FloatingJoystickEditor : OpacityJoystickEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (background != null)
        {
            RectTransform backgroundRect = (RectTransform) background.objectReferenceValue;
            backgroundRect.pivot = center;
        }
    }
}