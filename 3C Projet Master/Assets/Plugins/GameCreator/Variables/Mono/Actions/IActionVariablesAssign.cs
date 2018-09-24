namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public abstract class IActionVariablesAssign : IAction
	{
        public enum ValueFrom
        {
            Player,
            Invoker,
            Constant
        }

        public ValueFrom valueFrom = ValueFrom.Constant;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            this.ExecuteAssignement(target);
            return true;
        }

        public abstract void ExecuteAssignement(GameObject target);

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Assign";
        protected const string NODE_TITLE = "Assign {0} to {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spVariable;
        private SerializedProperty spValueFrom;
        private SerializedProperty spValue;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, "(none)", "(none)");
		}

		protected override void OnEnableEditorChild ()
		{
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spValueFrom = this.serializedObject.FindProperty("valueFrom");
            this.spValue = this.serializedObject.FindProperty("value");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spVariable = null;
            this.spValueFrom = null;
            this.spValue = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spVariable);
            EditorGUILayout.Space();

            if (this.PaintInspectorTarget())
            {
                EditorGUILayout.PropertyField(this.spValueFrom);
                if (this.spValueFrom.intValue == (int)ValueFrom.Constant)
                {
                    EditorGUILayout.PropertyField(this.spValue);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(this.spValue);
            }

			this.serializedObject.ApplyModifiedProperties();
		}

        public abstract bool PaintInspectorTarget();

		#endif
	}
}
