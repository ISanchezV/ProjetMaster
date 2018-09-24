namespace GameCreator.Characters
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
	public class ActionCharacterModel : IAction
	{
        public TargetCharacter character = new TargetCharacter();
        public GameObject prefabModel;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget != null && this.prefabModel != null)
            {
                RuntimeAnimatorController runtimeController = null;

                CharacterAnimator targetCharAnim = charTarget.GetComponent<CharacterAnimator>();
                if (targetCharAnim.animator != null)
                {
                    runtimeController = targetCharAnim.animator.runtimeAnimatorController;
                    Destroy(targetCharAnim.animator.gameObject);
                }

                GameObject instance = Instantiate<GameObject>(this.prefabModel, charTarget.transform);
                instance.name = this.prefabModel.name;

                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;

                Animator instanceAnimator = instance.GetComponent<Animator>();
                if (instanceAnimator != null)
                {
                    targetCharAnim.animator = instanceAnimator;
                    targetCharAnim.ResetControllerTopology(runtimeController);
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

        public static new string NAME = "Character/Character Model";
        private const string NODE_TITLE = "Change character {0} model";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spPrefabModel;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            Character c = this.character.GetCharacter(gameObject);
            return string.Format(NODE_TITLE, (c == null ? "none" : c.name));
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spPrefabModel = this.serializedObject.FindProperty("prefabModel");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spPrefabModel = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spPrefabModel);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
