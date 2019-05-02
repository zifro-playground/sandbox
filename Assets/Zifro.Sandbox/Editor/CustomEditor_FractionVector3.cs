using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Zifro.Sandbox.Entities;

[CustomPropertyDrawer(typeof(FractionVector3))]
public class CustomEditor_FractionVector3 : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		
		// Property fields
		SerializedProperty fractionXSerialized = property.FindPropertyRelative($"<{nameof(FractionVector3.fractionX)}>k__BackingField");
		SerializedProperty fractionYSerialized = property.FindPropertyRelative($"<{nameof(FractionVector3.fractionY)}>k__BackingField");
		SerializedProperty fractionZSerialized = property.FindPropertyRelative($"<{nameof(FractionVector3.fractionZ)}>k__BackingField");

		var fraction = new FractionVector3(
			fractionXSerialized.intValue,
			fractionYSerialized.intValue,
			fractionZSerialized.intValue
		);

		Vector3 result = EditorGUI.Vector3Field(position, label, fraction);

		fraction = (FractionVector3) result;

		fractionXSerialized.intValue = fraction.fractionX;
		fractionYSerialized.intValue = fraction.fractionY;
		fractionZSerialized.intValue = fraction.fractionZ;

		

		EditorGUI.EndProperty();
	}
}
