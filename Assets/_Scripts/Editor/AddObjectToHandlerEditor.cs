using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddObjectToHandler))]
[CanEditMultipleObjects]

public class AddObjectToHandlerEditor : Editor
{
    SerializedProperty interactionType;
    SerializedProperty isMachine;
    SerializedProperty machineInteractionType;
    SerializedProperty outputObject;

    void OnEnable()
    {
        interactionType = serializedObject.FindProperty("interactionType");
        isMachine = serializedObject.FindProperty("isMachine");
        machineInteractionType = serializedObject.FindProperty("machineInteractionType");
        outputObject = serializedObject.FindProperty("outputObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(interactionType);
        EditorGUILayout.PropertyField(isMachine);

        if (isMachine.boolValue)
        {
            EditorGUILayout.PropertyField(machineInteractionType);
            EditorGUILayout.PropertyField(outputObject);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
