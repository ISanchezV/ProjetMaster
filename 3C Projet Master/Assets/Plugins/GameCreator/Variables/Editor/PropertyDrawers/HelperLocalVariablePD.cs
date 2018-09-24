namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomPropertyDrawer(typeof(HelperLocalVariable))]
    public class HelperLocalVariablePD : HelperGenericVariablePD
    {
        private const string PROP_TARGET_TYP = "targetType";
        private const string PROP_TARGET_OBJ = "targetObject";
        private const string PROP_INCHILDREN = "inChildren";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTargetType;
        private SerializedProperty spTargetObject;
        private SerializedProperty spInChildren;

        // GUI METHODS: ---------------------------------------------------------------------------

        private const float INCHILDREN_WIDTH = 15f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);

            this.spTargetType = property.FindPropertyRelative(PROP_TARGET_TYP);
            this.spTargetObject = property.FindPropertyRelative(PROP_TARGET_OBJ);
            this.spInChildren = property.FindPropertyRelative(PROP_INCHILDREN);
            this.spAllowTypesMask = property.FindPropertyRelative(PROP_ALLOW_TYPES_MASK);

            Rect rectTargetType = new Rect(
                position.x,
                position.y,
                position.width - INCHILDREN_WIDTH - 5f,
                EditorGUI.GetPropertyHeight(this.spTargetType, true)
            );
            Rect rectInChildren = new Rect(
                rectTargetType.x + rectTargetType.width + 5f,
                rectTargetType.y,
                INCHILDREN_WIDTH,
                rectTargetType.height
            );
            Rect rectTargetObject = new Rect(
                position.x,
                rectTargetType.y + rectTargetType.height + 2f,
                position.width,
                EditorGUI.GetPropertyHeight(this.spTargetObject, true)
            );

            Rect rectVariable = new Rect(
                position.x,
                rectTargetObject.y + rectTargetObject.height + 2f,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rectTargetType, this.spTargetType, GUICONTENT_EMPTY);
            EditorGUI.PropertyField(rectInChildren, this.spInChildren, GUIContent.none);
            EditorGUI.BeginDisabledGroup(this.spTargetType.intValue != (int)HelperLocalVariable.Target.GameObject);
            EditorGUI.PropertyField(rectTargetObject, this.spTargetObject, GUICONTENT_EMPTY);
            EditorGUI.EndDisabledGroup();

            SerializedProperty spName = property.FindPropertyRelative(PROP_NAME);
            this.PaintLocalVariable(spName, rectVariable, label);
            
            EditorGUI.EndProperty();
        }

        private void PaintLocalVariable(SerializedProperty spName, Rect rect, GUIContent label)
        {
            if (this.spTargetType.intValue == (int)HelperLocalVariable.Target.GameObject)
            {
                EditorGUI.BeginDisabledGroup(this.spTargetObject.objectReferenceValue == null);
                this.PaintVariables(rect, spName, label);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                string previousName = spName.stringValue;
                EditorGUI.PropertyField(rect, spName, label);
                if (previousName != spName.stringValue)
                {
                    spName.stringValue = VariableEditor.ProcessName(spName.stringValue);
                }
            }
        }

		protected override GenericVariableSelectWindow GetWindow(Rect ctaRect)
		{
            if (this.spTargetObject.objectReferenceValue == null) return null;

            return new LocalVariableSelectWindow(
                ctaRect,
                (GameObject)this.spTargetObject.objectReferenceValue,
                this.spInChildren.boolValue,
                this.Callback,
                (this.spAllowTypesMask == null ? 0 : spAllowTypesMask.intValue)
            );
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            return (
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_NAME), true) + 
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_TARGET_TYP), true) +
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_TARGET_OBJ), true) +
                4f
            );
		}
	}
}