namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	public abstract class ICondition : MonoBehaviour 
	{
        public virtual bool Check()
		{
			return true;
		}

        public virtual bool Check(GameObject target)
        {
            return this.Check();
        }

        public virtual bool Check(GameObject target, params object[] parameters)
        {
            return this.Check(target);
        }

		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                    |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		public static string NAME = "General/Empty Condition";
		protected SerializedObject serializedObject;
        public bool isExpanded = false;

		// METHODS: ----------------------------------------------------------------------------------------------------
        
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

		public void OnEnableEditor(UnityEngine.Object condition)
		{
			this.serializedObject = new SerializedObject(condition);
			this.serializedObject.Update();

			this.OnEnableEditorChild();
		}

		// VIRTUAL AND ABSTRACT METHODS: -------------------------------------------------------------------------------

		public abstract string GetNodeTitle();
		public abstract void OnInspectorGUI();
		protected abstract void OnEnableEditorChild();
        protected virtual void OnDisableEditorChild() {}

		#endif
	}
}