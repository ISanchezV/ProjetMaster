namespace GameCreator.Characters
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCharacterGesture : IAction
	{
        public TargetCharacter character = new TargetCharacter();
        public AnimationClip clip;
        public AvatarMask avatarMask;
        public bool waitTillComplete = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character charTarget = this.character.GetCharacter(target);
            if (this.clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                charTarget.GetCharacterAnimator().PlayGesture(this.clip, this.avatarMask);
            }

            return !this.waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            Character charTarget = this.character.GetCharacter(target);
            if (this.clip != null && charTarget != null && charTarget.GetCharacterAnimator() != null)
            {
                if (this.waitTillComplete)
                {
                    WaitForSeconds waitForSeconds = new WaitForSeconds(this.clip.length);
                    yield return waitForSeconds;
                }
            }

			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Character/Character Gesture";
        private const string NODE_TITLE = "Character {0} do gesture {1}";

        private static readonly GUIContent GC_MASK = new GUIContent("Mask (optional)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
        private SerializedProperty spClip;
        private SerializedProperty spAvatarMask;
        private SerializedProperty spWaitTillComplete;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string clipName = (this.clip == null ? "none" : this.clip.name);
            if (clipName.Contains("@"))
            {
                string[] split = clipName.Split(new char[] {'@'}, 2, StringSplitOptions.RemoveEmptyEntries);
                clipName = split[split.Length - 1];
            }
            
            return string.Format(NODE_TITLE, this.character.ToString(), clipName);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spClip = this.serializedObject.FindProperty("clip");
            this.spAvatarMask = this.serializedObject.FindProperty("avatarMask");
            this.spWaitTillComplete = this.serializedObject.FindProperty("waitTillComplete");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spClip = null;
            this.spAvatarMask = null;
            this.spWaitTillComplete = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spClip);
            EditorGUILayout.PropertyField(this.spAvatarMask, GC_MASK);
            EditorGUILayout.PropertyField(this.spWaitTillComplete);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
