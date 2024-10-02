using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BotController))]
public class BotControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get a reference to the target script (the one being edited)
        BotController botController = (BotController)target;

        // Draw the dropdown for bot difficulty
        botController.botDifficulty = (BotDifficulty)EditorGUILayout.EnumPopup("Bot Difficulty", botController.botDifficulty);


        if( botController.botDifficulty == BotDifficulty.Easy)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"));
        }
        else if (botController.botDifficulty == BotDifficulty.Medium)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("agent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shootVelocity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turretRotationSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
        }
        else if (botController.botDifficulty == BotDifficulty.Hard)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("agent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shootVelocity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turretRotationSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
