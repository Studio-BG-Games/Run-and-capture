using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Custom Snap Move", typeof(CustomSnap))]
public class CustomSnappingTool : EditorTool
{
    public Texture2D ToolIcon;

    public override GUIContent toolbarIcon {
        get
        {
            
            return new GUIContent
            {
                image = ToolIcon,
                text = "Custom Snape Move Tool",
                tooltip = "Custom Snape Move Tool - best tool ever"
            };
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        Transform targetTransform = ((CustomSnap)target).transform;

        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(targetTransform.position, Quaternion.identity);

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetTransform, name: "Move with snap tool");
        }   MoveWithSnapping(targetTransform, newPosition);
    }

    
    private void MoveWithSnapping(Transform targetTransform, Vector3 newPosition)
    {
        CustomSnapPoint[] allPoints = FindObjectsOfType<CustomSnapPoint>();
        CustomSnapPoint[] targetPoints = targetTransform.GetComponentsInChildren<CustomSnapPoint>();

        Vector3 bestPosition = newPosition;
        float closestDistance = float.PositiveInfinity;

        foreach (CustomSnapPoint point in allPoints)
        {
            if (point.transform.parent == targetTransform) continue;

            foreach (CustomSnapPoint ownPoint in targetPoints)
            {
                Vector3 targetPos = point.transform.position - (ownPoint.transform.position - targetTransform.position);
                float distance = Vector3.Distance(a: targetPos, b: newPosition);

                if(distance < closestDistance)
                    {
                      closestDistance = distance;
                      bestPosition = targetPos;
                    }
            }
        }

        if(closestDistance < 0.5f)
        {
            targetTransform.position = bestPosition;
        }
        else
        {
            targetTransform.position = newPosition;
        }

        
        
    }
}

