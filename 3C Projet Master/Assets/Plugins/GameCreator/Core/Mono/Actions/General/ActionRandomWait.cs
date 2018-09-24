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
	public class ActionRandomWait : IAction 
	{
		public float minWaitTime = 1.0f;
		public float maxWaitTime = 2.0f;
		private WaitForSeconds waitForSeconds;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
			float time = Random.Range(this.minWaitTime, this.maxWaitTime);
			this.waitForSeconds = new WaitForSeconds(time);

			yield return this.waitForSeconds;
			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "General/Wait Random";
		private const string NODE_TITLE = "Wait between {0} and {1} seconds";

		private static readonly GUIContent GUICONTENT_MINWAITTIME = new GUIContent("Min time to wait (s)");
		private static readonly GUIContent GUICONTENT_MAXWAITTIME = new GUIContent("Max time to wait (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMinWaitTime;
		private SerializedProperty spMaxWaitTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.minWaitTime, this.maxWaitTime);
		}

		protected override void OnEnableEditorChild()
		{
			this.spMinWaitTime = this.serializedObject.FindProperty("minWaitTime");
			this.spMaxWaitTime = this.serializedObject.FindProperty("maxWaitTime");
		}

		protected override void OnDisableEditorChild()
		{
			this.spMinWaitTime = null;
			this.spMaxWaitTime = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spMinWaitTime, GUICONTENT_MINWAITTIME);
			EditorGUILayout.PropertyField(this.spMaxWaitTime, GUICONTENT_MAXWAITTIME);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}