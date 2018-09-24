namespace GameCreator.Dialogue
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

	[CustomEditor(typeof(DatabaseDialogue))]
    public class DatabaseDialogueEditor : IDatabaseEditor
	{
        private const string DEFAULT_SKIN_PATH = "Assets/Plugins/GameCreator/Dialogue/Resources/GameCreator/DefaultDialogueSkin.prefab";
        private const string TITLE_DATA = "Default Data";

        private const string PROP_DATA = "defaultConfig";
        private const string PROP_DATA_SKIN = "dialogueSkin";

        // PROPERTIES: ----------------------------------------------------------------------------

		private bool stylesInitialized = false;
        private SerializedProperty spData;

		// INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
		{
            this.spData = serializedObject.FindProperty(PROP_DATA);

            SerializedProperty spDataSkin = this.spData.FindPropertyRelative(PROP_DATA_SKIN);
            if (spDataSkin.objectReferenceValue == null)
            {
                GameObject skin = AssetDatabase.LoadAssetAtPath<GameObject>(DEFAULT_SKIN_PATH);
                spDataSkin.objectReferenceValue = skin;

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
		}

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "https://docs.gamecreator.io/manual/dialogue.html";
		}

		public override string GetName ()
		{
			return "Dialogue";
		}

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnInspectorGUI()
		{
			if (!this.stylesInitialized)
			{
				this.InitializeStyles();
				this.stylesInitialized = true;
			}

			this.serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(TITLE_DATA, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.spData);

            EditorGUILayout.EndVertical();

			this.serializedObject.ApplyModifiedProperties();
		}

        // PRIVATE METHODS: -----------------------------------------------------------------------

		private void InitializeStyles()
		{
			
		}
	}
}