namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionEvent : IAction
	{
        public Event gameCreatorEvent;
        public bool waitToFinish = true;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (!this.waitToFinish)
            {
                if (this.gameCreatorEvent != null) this.gameCreatorEvent.Interact(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            if (this.gameCreatorEvent != null)
            {
                yield return this.gameCreatorEvent.InteractCoroutine(target);
            }

			yield return 0;
		}

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        private static readonly GUIContent GUICONTENT_EVENT = new GUIContent("Event");
		public static new string NAME = "General/Call Event";
		private const string NODE_TITLE = "Call event {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spEvent;
        private SerializedProperty spWaitToFinish;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE, 
                (this.gameCreatorEvent == null ? "none" : this.gameCreatorEvent.gameObject.name)
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spEvent = this.serializedObject.FindProperty("gameCreatorEvent");
            this.spWaitToFinish = this.serializedObject.FindProperty("waitToFinish");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spEvent = null;
            this.spWaitToFinish = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spEvent, GUICONTENT_EVENT);
            EditorGUILayout.PropertyField(this.spWaitToFinish);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
