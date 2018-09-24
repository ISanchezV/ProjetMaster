namespace GameCreator.Characters
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(PlayerCharacter))]
	public class PlayerCharacterEditor : CharacterEditor 
	{
		private const string PLAYER_PREFAB_PATH = "Assets/Plugins/GameCreator/Characters/Prefabs/Player.prefab";
		private const string SECTION_INPUT = "Player Input";

		private const string PROP_INPUTT = "inputType";
		private const string PROP_MOUSEB = "mouseButtonMove";
		private const string PROP_INPUT_JMP = "jumpKey";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Section sectionInput;
		private SerializedProperty spInputType;
		private SerializedProperty spMouseButtonMove;
		private SerializedProperty spInputJump;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		protected new void OnEnable()
		{
			base.OnEnable();

			string iconInputPath = Path.Combine(CHARACTER_ICONS_PATH, "PlayerInput.png");
			Texture2D iconInput = AssetDatabase.LoadAssetAtPath<Texture2D>(iconInputPath);
			this.sectionInput = new Section(SECTION_INPUT, iconInput, this.Repaint);

			this.spInputType = serializedObject.FindProperty(PROP_INPUTT);
			this.spMouseButtonMove = serializedObject.FindProperty(PROP_MOUSEB);
			this.spInputJump = serializedObject.FindProperty(PROP_INPUT_JMP);
		}

		protected new void OnDisable()
		{
			base.OnDisable();
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			base.PaintInspector();
			this.sectionInput.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionInput.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.PropertyField(this.spInputType);
					if (this.spInputType.intValue == (int)PlayerCharacter.INPUT_TYPE.PointAndClick)
					{
						EditorGUILayout.PropertyField(this.spMouseButtonMove);
					}

					EditorGUILayout.PropertyField(this.spInputJump);

					EditorGUILayout.EndVertical();
				}
			}

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		// MENU ITEM: --------------------------------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Characters/Player", false, 0)]
		public static void CreatePlayer()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PLAYER_PREFAB_PATH);
			if (prefab == null) return;

			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			instance = CreateSceneObject.Create(instance, true);
		}
	}
}