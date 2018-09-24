namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionActions : IAction 
	{
		public Actions actions;
		public bool waitToFinish = false;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
			if (this.actions != null)
			{
                Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                    this.actions.actionsList.ExecuteCoroutine(target, null)
                );

				if (this.waitToFinish) yield return coroutine;
			}

			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "General/Execute Actions";
		private const string NODE_TITLE = "Execute actions {0} {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spActions;
		private SerializedProperty spWaitToFinish;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				(this.actions == null ? "none" : this.actions.name),
				(this.waitToFinish ? "and wait" : "")
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spActions = this.serializedObject.FindProperty("actions");
			this.spWaitToFinish = this.serializedObject.FindProperty("waitToFinish");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spActions = null;
			this.spWaitToFinish = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spActions);
			EditorGUILayout.PropertyField(this.spWaitToFinish);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}