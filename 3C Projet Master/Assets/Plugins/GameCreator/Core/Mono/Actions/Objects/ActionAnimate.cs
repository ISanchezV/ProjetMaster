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
	public class ActionAnimate : IAction 
	{
		public Animator animator = null;
		public string parameterName = "Parameter Name";
		public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;

		public int parameterInteger = 1;
		public float parameterFloat = 1.0f;
		public bool parameterBool   = true;

        // EXECUTABLE: -------------------------------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.animator != null)
            {
                switch (this.parameterType)
                {
                    case AnimatorControllerParameterType.Trigger:
                        this.animator.SetTrigger(this.parameterName);
                        break;

                    case AnimatorControllerParameterType.Int:
                        this.animator.SetInteger(this.parameterName, this.parameterInteger);
                        break;

                    case AnimatorControllerParameterType.Float:
                        this.animator.SetFloat(this.parameterName, this.parameterFloat);
                        break;

                    case AnimatorControllerParameterType.Bool:
                        this.animator.SetBool(this.parameterName, this.parameterBool);
                        break;
                }
            }

            return true;
        }

		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR                                                                                                    |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		private static readonly GUIContent GUICONTENT_ANIMATOR = new GUIContent("Animator");
		private static readonly GUIContent GUICONTENT_PARAM_NAME = new GUIContent("Parameter Name");
		private static readonly GUIContent GUICONTENT_PARAM_TYPE = new GUIContent("Parameter Type");

		private static readonly GUIContent GUICONTENT_PARAM_INT = new GUIContent("Integer");
		private static readonly GUIContent GUICONTENT_PARAM_FLO = new GUIContent("Float");
		private static readonly GUIContent GUICONTENT_PARAM_BOL = new GUIContent("Bool");

		public static new string NAME = "Object/Animate";
		private const string NODE_TITLE = "Animate {0}";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spAnimator;
		private SerializedProperty spParameterName;
		private SerializedProperty spParameterType;

		private SerializedProperty spParameterInteger;
		private SerializedProperty spParameterFloat;
		private SerializedProperty spParameterBool;

		// INSPECTOR METHODS: ------------------------------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, (this.animator == null ? "none" : this.animator.name));
		}

		protected override void OnEnableEditorChild ()
		{
			this.spAnimator = this.serializedObject.FindProperty("animator");
			this.spParameterName = this.serializedObject.FindProperty("parameterName");
			this.spParameterType = this.serializedObject.FindProperty("parameterType");

			this.spParameterInteger = this.serializedObject.FindProperty("parameterInteger");
			this.spParameterFloat = this.serializedObject.FindProperty("parameterFloat");
			this.spParameterBool = this.serializedObject.FindProperty("parameterBool");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spAnimator = null;
			this.spParameterName = null;
			this.spParameterType = null;

			this.spParameterInteger = null;
			this.spParameterFloat = null;
			this.spParameterBool = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spAnimator, GUICONTENT_ANIMATOR);
            EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.spParameterName, GUICONTENT_PARAM_NAME);
			EditorGUILayout.PropertyField(this.spParameterType, GUICONTENT_PARAM_TYPE);
			int paramTypeInt = this.spParameterType.intValue;
			AnimatorControllerParameterType paramType = (AnimatorControllerParameterType)paramTypeInt;

			switch (paramType)
			{
			case AnimatorControllerParameterType.Int : 
				EditorGUILayout.PropertyField(this.spParameterInteger, GUICONTENT_PARAM_INT);
				break;

			case AnimatorControllerParameterType.Float :
				EditorGUILayout.PropertyField(this.spParameterFloat, GUICONTENT_PARAM_FLO);
				break;

			case AnimatorControllerParameterType.Bool :
				EditorGUILayout.PropertyField(this.spParameterBool, GUICONTENT_PARAM_BOL);
				break;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}