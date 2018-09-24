namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TargetPosition))]
	public class TargetPositionPD : PropertyDrawer
	{
        private const string PROP_TARGET = "target";
        private const string PROP_OFFSET = "offset";
        private const string PROP_TRANSFORM = "targetTransform";
        private const string PROP_POSITION = "targetPosition";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);
            SerializedProperty spOffset = property.FindPropertyRelative(PROP_OFFSET);
            SerializedProperty spTargetTransform = property.FindPropertyRelative(PROP_TRANSFORM);
            SerializedProperty spTargetPosition = property.FindPropertyRelative(PROP_POSITION);

            Rect rectTarget = new Rect(
                position.x, 
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            Rect rectOffset = new Rect(
                position.x,
                position.y + rectTarget.height + 1f,
                position.width,
                (spTarget.intValue == (int)TargetPosition.Target.Position ? 0.0f : EditorGUIUtility.singleLineHeight)
            );

            Rect rectExtra = new Rect(
                position.x,
                rectOffset.y + rectOffset.height + 1f,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rectTarget, spTarget, label);
            if (spTarget.intValue != (int)TargetPosition.Target.Position)  EditorGUI.PropertyField(rectOffset, spOffset);
            if (spTarget.intValue == (int)TargetPosition.Target.Transform) EditorGUI.PropertyField(rectExtra, spTargetTransform);
            if (spTarget.intValue == (int)TargetPosition.Target.Position)  EditorGUI.PropertyField(rectExtra, spTargetPosition);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);
            SerializedProperty spOffset = property.FindPropertyRelative(PROP_OFFSET);
            SerializedProperty spTargetTransform = property.FindPropertyRelative(PROP_TRANSFORM);
            SerializedProperty spTargetPosition = property.FindPropertyRelative(PROP_POSITION);

            float height = EditorGUI.GetPropertyHeight(spTarget);

            if (spTarget.intValue != (int)TargetPosition.Target.Position)
            {
                height += EditorGUI.GetPropertyHeight(spOffset);
            }

            if (spTarget.intValue == (int)TargetPosition.Target.Transform)
            {
                height += EditorGUI.GetPropertyHeight(spTargetTransform);
            }

            if (spTarget.intValue == (int)TargetPosition.Target.Position)
            {
                height += EditorGUI.GetPropertyHeight(spTargetPosition);
            }

            return height;
		}
	}
}