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
	public class ActionEnableComponent : IAction 
	{
        public GameObject target;
        public MonoBehaviour component;
        public BoolProperty enable = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.component != null) this.component.enabled = this.enable.GetValue();
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Enable Component";
		private const string NODE_TITLE = "{0} component {1}";
        private const string SELECT_TEXT = "Select Target Component";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
        private SerializedProperty spComponent;
		private SerializedProperty spEnable;

        private MonoBehaviour[] selectMonoBehaviors;
        private string[] selectNames;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
                this.enable,
                this.GetName(this.component)
            );
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
            this.spComponent = this.serializedObject.FindProperty("component");
            this.spEnable = this.serializedObject.FindProperty("enable");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
            this.spComponent = null;
            this.spEnable = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            Object prevTarget = this.spTarget.objectReferenceValue;
			EditorGUILayout.PropertyField(this.spTarget);
            Object currTarget = this.spTarget.objectReferenceValue;

            if (currTarget == null || this.selectMonoBehaviors == null)
            {
                this.selectMonoBehaviors = new MonoBehaviour[1];
                this.selectNames = new string[1];

                this.selectMonoBehaviors[0] = null;
                this.selectNames[0] = SELECT_TEXT;
            }

            if (currTarget != null && prevTarget != currTarget)
            {
                GameObject targetGO = (GameObject)this.spTarget.objectReferenceValue;
                MonoBehaviour[] tmpMonoBehaviours = targetGO.GetComponents<MonoBehaviour>();

                this.selectMonoBehaviors = new MonoBehaviour[tmpMonoBehaviours.Length + 1];
                this.selectNames = new string[tmpMonoBehaviours.Length + 1];

                this.selectMonoBehaviors[0] = null;
                this.selectNames[0] = SELECT_TEXT;

                for (int i = 0; i < tmpMonoBehaviours.Length; ++i)
                {
                    this.selectMonoBehaviors[i + 1] = tmpMonoBehaviours[i];
                    this.selectNames[i + 1] = this.GetName(tmpMonoBehaviours[i]);
                }
            }

            bool componentCondition = this.spTarget.objectReferenceValue == null;
            componentCondition = componentCondition || this.selectMonoBehaviors == null;
            EditorGUI.BeginDisabledGroup(componentCondition);
            EditorGUILayout.PropertyField(this.spComponent);

            Rect selectRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label);
            selectRect = new Rect(
                selectRect.x + EditorGUIUtility.labelWidth,
                selectRect.y,
                selectRect.width - EditorGUIUtility.labelWidth,
                selectRect.height
            );

            int selection = EditorGUI.Popup(selectRect, 0, this.selectNames);
            if (selection != 0)
            {
                this.spComponent.objectReferenceValue = this.selectMonoBehaviors[selection];
            }

            EditorGUILayout.PropertyField(this.spEnable);
            EditorGUI.EndDisabledGroup();

			this.serializedObject.ApplyModifiedProperties();
		}

        private string GetName(MonoBehaviour mono)
        {
            if (mono == null) return "none";

            string typeName = mono.GetType().ToString();
            int splitIndex = typeName.LastIndexOf('.');
            if (splitIndex >= 0)
            {
                typeName = typeName.Substring(splitIndex + 1);
            }

            return typeName;
        }

		#endif
	}
}