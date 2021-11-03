using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerEditor : Editor
{
	private PlayerController myTarget;
	private SerializedObject soTarget;

	private SerializedProperty playerId;
	private SerializedProperty moveSpeed;
	private SerializedProperty rotationSpeed;
	private SerializedProperty useGamePad;
	
	private SerializedProperty PewPrefab;
	private SerializedProperty SpawnLocation;
	private SerializedProperty soundEffect;
	private SerializedProperty soundFire1;
	private SerializedProperty Player;
	private SerializedProperty intVar5;

	public void OnEnable()
	{
		myTarget = (PlayerController)target;
		soTarget = new SerializedObject(target);

		playerId = soTarget.FindProperty("playerId");
		moveSpeed = soTarget.FindProperty("moveSpeed");
		rotationSpeed = soTarget.FindProperty("rotationSpeed");
		useGamePad = soTarget.FindProperty("useGamePad");
		
		PewPrefab = soTarget.FindProperty("PewPrefab");
		SpawnLocation = soTarget.FindProperty("SpawnLocation");
		soundEffect = soTarget.FindProperty("soundEffect");
		soundFire1 = soTarget.FindProperty("soundFire1");
		Player = soTarget.FindProperty("Player");
		intVar5 = soTarget.FindProperty("intVar5");
	}

	public override void OnInspectorGUI()
	{
		soTarget.Update();

		EditorGUI.BeginChangeCheck();

		myTarget.toolbarTop = GUILayout.Toolbar(myTarget.toolbarTop, new string[] { "Movement", "Basic Weapon", "Tab3", "Tab4" });

		switch (myTarget.toolbarTop)
		{
			case 0:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Movement";
				break;
			case 1:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Basic Weapon";
				break;
			case 2:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Tab3";
				break;
			case 3:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Tab4";
				break;
		}

		myTarget.toolbarBottom = GUILayout.Toolbar(myTarget.toolbarBottom, new string[] { "Tab5", "Tab6", "Tab7", "Tab8" });

		switch (myTarget.toolbarBottom)
		{
			case 0:
				myTarget.toolbarTop = 4;
				myTarget.currentTab = "Tab5";
				break;
			case 1:
				myTarget.toolbarTop = 4;
				myTarget.currentTab = "Tab6";
				break;
			case 2:
				myTarget.toolbarTop = 4;
				myTarget.currentTab = "Tab7";
				break;
			case 3:
				myTarget.toolbarTop = 4;
				myTarget.currentTab = "Tab8";
				break;
		}

		if (EditorGUI.EndChangeCheck())
		{
			soTarget.ApplyModifiedProperties();
			GUI.FocusControl(null);
		}

		EditorGUI.BeginChangeCheck();

		switch (myTarget.currentTab)
		{
			case "Movement":
				EditorGUILayout.PropertyField(playerId);
				EditorGUILayout.PropertyField(moveSpeed);
				EditorGUILayout.PropertyField(rotationSpeed);
				EditorGUILayout.PropertyField(useGamePad);
				break;
			case "Basic Weapon":
				EditorGUILayout.PropertyField(PewPrefab);
				EditorGUILayout.PropertyField(SpawnLocation);
				EditorGUILayout.PropertyField(soundEffect);
				EditorGUILayout.PropertyField(soundFire1);
				EditorGUILayout.PropertyField(Player);
				EditorGUILayout.PropertyField(intVar5);
				break;
			case "Tab3":
				break;
			case "Tab4":
				break;

		}

		if (EditorGUI.EndChangeCheck())
		{
			soTarget.ApplyModifiedProperties();
			GUI.FocusControl(null);
		}
	}
}
