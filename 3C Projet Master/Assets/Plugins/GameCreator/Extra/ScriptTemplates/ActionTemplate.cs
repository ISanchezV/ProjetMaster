/* ##HEADER##
namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class __CLASS_NAME__ : IAction
	{
		public int example = 0;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    //public const string CUSTOM_ICON_PATH = "Assets/[Custom Path To Icon]";

		public static new string NAME = "My Actions/Action Example";
		private const string NODE_TITLE = "Example value is {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spExample;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.example);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spExample = this.serializedObject.FindProperty("example");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spExample = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spExample, new GUIContent("Example Value"));

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
##FOOTER## */
