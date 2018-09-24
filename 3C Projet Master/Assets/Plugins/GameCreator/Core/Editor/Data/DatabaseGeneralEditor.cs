namespace GameCreator.Localization
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using UnityEditor.SceneManagement;
	using UnityEditorInternal;
	using System.Linq;
	using System.Reflection;
	using GameCreator.Core;

	[CustomEditor(typeof(DatabaseGeneral))]
	public class DatabaseGeneralEditor : IDatabaseEditor
	{
        private SerializedProperty spGeneralRenderMode;
        private SerializedProperty spPrefabMessage;
        private SerializedProperty spPrefabTouchstick;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void OnEnable()
		{
            if (serializedObject == null || target == null) return;
            this.spGeneralRenderMode = serializedObject.FindProperty("generalRenderMode");
            this.spPrefabMessage = serializedObject.FindProperty("prefabMessage");
            this.spPrefabTouchstick = serializedObject.FindProperty("prefabTouchstick");
		}

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "https://docs.gamecreator.io/";
		}

		public override string GetName ()
		{
			return "General";
		}

        public override int GetPanelWeight()
        {
            return 98;
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spGeneralRenderMode);
            EditorGUILayout.PropertyField(this.spPrefabMessage);
            EditorGUILayout.PropertyField(this.spPrefabTouchstick);

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}