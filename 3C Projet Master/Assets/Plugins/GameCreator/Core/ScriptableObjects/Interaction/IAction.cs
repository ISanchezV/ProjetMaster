namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
	public abstract class IAction : MonoBehaviour 
	{
        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            return false;
        }

        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            return this.InstantExecute(target, actions, index);
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            yield return 0;
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            IEnumerator execute = this.Execute(target, actions, index);
            object result = null;

            while (execute.MoveNext())
            {
                result = execute.Current;
                yield return result;
            }
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PROPERTIES: ----------------------------------------------------------------------------

		public static string NAME = "General/Empty Action";

		protected SerializedObject serializedObject;
        public bool isExpanded = false;

		// METHODS: -------------------------------------------------------------------------------

		private void Awake()
		{
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnEnable()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnValidate()
		{
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		public void OnEnableEditor(UnityEngine.Object action)
		{
			this.serializedObject = new SerializedObject(action);
			this.serializedObject.Update();

			this.OnEnableEditorChild();
		}

		public void OnDisableEditor()
		{
			this.serializedObject = null;
			this.OnDisableEditorChild();
		}

        // VIRTUAL AND ABSTRACT METHODS: ----------------------------------------------------------

		public abstract string GetNodeTitle();
		public abstract void OnInspectorGUI();
		protected virtual void OnEnableEditorChild()  {}
		protected virtual void OnDisableEditorChild() {}

        public virtual float GetOpacity()
        {
            return 1.0f;
        }

		#endif
	}
}