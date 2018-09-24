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
	public class ActionCancelActions : IAction
	{
        public Actions actions;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.actions != null && this.actions.actionsList != null)
            {
                this.actions.actionsList.Cancel();
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Cancel Actions";
        private const string NODE_TITLE = "Cancel action {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spActions;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                (this.actions == null ? "(none)" : this.actions.gameObject.name)
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spActions = serializedObject.FindProperty("actions");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spActions = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spActions);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
