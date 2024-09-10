using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddDispenserToHandler))]
[CanEditMultipleObjects]

public class AddDispenserToHandlerEditor : Editor
{
    SerializedProperty outputObject;

    void OnEnable()
    {
        outputObject = serializedObject.FindProperty("outputObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(outputObject);
        serializedObject.ApplyModifiedProperties();
    }
}
