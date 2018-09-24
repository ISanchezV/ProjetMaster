﻿namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Characters;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionPlayerMovementInput : IAction 
	{
		public PlayerCharacter.INPUT_TYPE inputType = PlayerCharacter.INPUT_TYPE.Directional;
		public PlayerCharacter.MOUSE_BUTTON mouseButton = PlayerCharacter.MOUSE_BUTTON.LeftClick;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (HookPlayer.Instance != null)
            {
                PlayerCharacter player = HookPlayer.Instance.Get<PlayerCharacter>();
                player.inputType = this.inputType;

                if (this.inputType == PlayerCharacter.INPUT_TYPE.PointAndClick)
                {
                    player.mouseButtonMove = this.mouseButton;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Player Movement Input";
		private const string NODE_TITLE = "Change input to {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spInputType;
		private SerializedProperty spMouseButton;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string value = this.spInputType.enumDisplayNames[this.spInputType.intValue];
			if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.PointAndClick)
			{
				value = string.Format(
					"{0} ({1})", 
					value, 
					this.spMouseButton.enumDisplayNames[this.spMouseButton.intValue]
				);
			}

			return string.Format(NODE_TITLE, value);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spInputType = this.serializedObject.FindProperty("inputType");
			this.spMouseButton = this.serializedObject.FindProperty("mouseButton");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spInputType = null;
			this.spMouseButton = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spInputType);
			if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.PointAndClick)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.spMouseButton);
				EditorGUI.indentLevel--;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}