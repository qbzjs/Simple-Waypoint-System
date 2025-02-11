﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Glitch))]
public class CC_GlitchEditor : Editor
{
	SerializedProperty p_mode;
	SerializedProperty p_interferencesSettings;
	SerializedProperty p_tearingSettings;

	SerializedProperty p_interferencesSpeed;
	SerializedProperty p_interferencesDensity;
	SerializedProperty p_interferencesMaxDisplacement;

	SerializedProperty p_tearingSpeed;
	SerializedProperty p_tearingIntensity;
	SerializedProperty p_tearingMaxDisplacement;
	SerializedProperty p_tearingAllowFlipping;
	SerializedProperty p_tearingYuvColorBleeding;
	SerializedProperty p_tearingYuvOffset;

	void OnEnable()
	{
		p_mode = serializedObject.FindProperty("mode");
		p_interferencesSettings = serializedObject.FindProperty("interferencesSettings");
		p_tearingSettings = serializedObject.FindProperty("tearingSettings");

		p_interferencesSpeed = p_interferencesSettings.FindPropertyRelative("speed");
		p_interferencesDensity = p_interferencesSettings.FindPropertyRelative("density");
		p_interferencesMaxDisplacement = p_interferencesSettings.FindPropertyRelative("maxDisplacement");

		p_tearingSpeed = p_tearingSettings.FindPropertyRelative("speed");
		p_tearingIntensity = p_tearingSettings.FindPropertyRelative("intensity");
		p_tearingMaxDisplacement = p_tearingSettings.FindPropertyRelative("maxDisplacement");
		p_tearingAllowFlipping = p_tearingSettings.FindPropertyRelative("allowFlipping");
		p_tearingYuvColorBleeding = p_tearingSettings.FindPropertyRelative("yuvColorBleeding");
		p_tearingYuvOffset = p_tearingSettings.FindPropertyRelative("yuvOffset");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(p_mode);

		if (p_mode.enumValueIndex == (int)CC_Glitch.Mode.Interferences)
		{
			DoInterferencesUI();
		}
		else if (p_mode.enumValueIndex == (int)CC_Glitch.Mode.Tearing)
		{
			DoTearingUI();
		}
		else // Complete
		{
			EditorGUILayout.LabelField("Interferences", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			DoInterferencesUI();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Tearing", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			DoTearingUI();
			EditorGUI.indentLevel--;
		}

		serializedObject.ApplyModifiedProperties();
	}

	void DoInterferencesUI()
	{
		EditorGUILayout.PropertyField(p_interferencesSpeed);
		EditorGUILayout.PropertyField(p_interferencesDensity);
		EditorGUILayout.PropertyField(p_interferencesMaxDisplacement);
	}

	void DoTearingUI()
	{
		EditorGUILayout.PropertyField(p_tearingSpeed);
		EditorGUILayout.PropertyField(p_tearingIntensity);
		EditorGUILayout.PropertyField(p_tearingMaxDisplacement);
		EditorGUILayout.PropertyField(p_tearingYuvColorBleeding, new GUIContent("YUV Color Bleeding"));

		if (p_tearingYuvColorBleeding.boolValue)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(p_tearingYuvOffset, new GUIContent("Offset"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PropertyField(p_tearingAllowFlipping);
	}
}
