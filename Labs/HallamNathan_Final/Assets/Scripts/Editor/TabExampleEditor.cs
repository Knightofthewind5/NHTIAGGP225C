using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabExample))]
public class TabExampleEditor : Editor
{
	private TabExample myTarget;
	private SerializedObject soTarget;

	private SerializedProperty stringVar1;
	private SerializedProperty stringVar2;
	private SerializedProperty stringVar3;
	private SerializedProperty stringVar4;
	private SerializedProperty stringVar5;

	private SerializedProperty intVar1;
	private SerializedProperty intVar2;
	private SerializedProperty intVar3;
	private SerializedProperty intVar4;
	private SerializedProperty intVar5;

	public void OnEnable()
	{
		myTarget = (TabExample)target;
		soTarget = new SerializedObject(target);

		stringVar1 = soTarget.FindProperty("stringVar1");
		stringVar2 = soTarget.FindProperty("stringVar2");
		stringVar3 = soTarget.FindProperty("stringVar3");
		stringVar4 = soTarget.FindProperty("stringVar4");
		stringVar5 = soTarget.FindProperty("stringVar5");

		intVar1 = soTarget.FindProperty("intVar1");
		intVar2 = soTarget.FindProperty("intVar2");
		intVar3 = soTarget.FindProperty("intVar3");
		intVar4 = soTarget.FindProperty("intVar4");
		intVar5 = soTarget.FindProperty("intVar5");
	}

	public override void OnInspectorGUI()
	{
		soTarget.Update();

		EditorGUI.BeginChangeCheck();

		myTarget.toolbarTop = GUILayout.Toolbar(myTarget.toolbarTop, new string[] { "Strings", "Integers", "Tab3", "Tab4" });

		switch (myTarget.toolbarTop)
		{
			case 0:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Strings";
				break;
			case 1:
				myTarget.toolbarBottom = 4;
				myTarget.currentTab = "Integers";
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
			case "Strings":
				EditorGUILayout.PropertyField(stringVar1);
				EditorGUILayout.PropertyField(stringVar2);
				EditorGUILayout.PropertyField(stringVar3);
				EditorGUILayout.PropertyField(stringVar4);
				EditorGUILayout.PropertyField(stringVar5);
				break;
			case "Integers":
				EditorGUILayout.PropertyField(intVar1);
				EditorGUILayout.PropertyField(intVar2);
				EditorGUILayout.PropertyField(intVar3);
				EditorGUILayout.PropertyField(intVar4);
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
		}
	}

}






