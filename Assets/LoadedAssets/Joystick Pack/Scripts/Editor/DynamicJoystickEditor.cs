using LoadedAssets.Joystick_Pack.Scripts.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynamicJoystick))]
public class DynamicJoystickEditor : OpacityJoystickEditor
{
    private SerializedProperty moveThreshold;

    protected override void OnEnable()
    {
        base.OnEnable();
        moveThreshold = serializedObject.FindProperty("moveThreshold");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (background != null)
        {
            RectTransform backgroundRect = (RectTransform)background.objectReferenceValue;
            backgroundRect.pivot = center;
        }
    }

    protected override void DrawValues()
    {
        base.DrawValues();
        EditorGUILayout.PropertyField(moveThreshold, new GUIContent("Move Threshold", "The distance away from the center input has to be before the joystick begins to move."));
    }
}