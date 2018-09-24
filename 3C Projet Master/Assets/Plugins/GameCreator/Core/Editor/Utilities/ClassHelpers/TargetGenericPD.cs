namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public abstract class TargetGenericPD : PropertyDrawer
    {
        public const string PROP_TARGET = "target";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        // ABSTRACT METHODS: ----------------------------------------------------------------------

        protected abstract SerializedProperty GetProperty(int option, SerializedProperty property);

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);

            Rect rectTarget = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rectTarget, spTarget, label);
            SerializedProperty spValue = this.GetProperty(spTarget.intValue, property);
            if (spValue != null)
            {
                Rect rectValue = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 1f,
                    position.width,
                    EditorGUI.GetPropertyHeight(spValue)
                );

                EditorGUI.PropertyField(rectValue, spValue, GUICONTENT_EMPTY);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);

            float height = EditorGUI.GetPropertyHeight(spTarget);

            SerializedProperty spValue = this.GetProperty(spTarget.intValue, property);
            if (spValue != null)
            {
                height += EditorGUI.GetPropertyHeight(spValue);
            }

            return height;
        }
    }
}