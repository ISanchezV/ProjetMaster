namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.SceneManagement;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionLoadScene : IAction 
	{
        public StringProperty sceneName = new StringProperty();
        public LoadSceneMode mode = LoadSceneMode.Single;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            SceneManager.LoadScene(this.sceneName.GetValue(), this.mode);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Scene/Load Scene";
		private const string NODE_TITLE = "Load scene {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spSceneName;
        private SerializedProperty spMode;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.sceneName);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spSceneName = this.serializedObject.FindProperty("sceneName");
            this.spMode = this.serializedObject.FindProperty("mode");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spSceneName);
            EditorGUILayout.PropertyField(this.spMode);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}