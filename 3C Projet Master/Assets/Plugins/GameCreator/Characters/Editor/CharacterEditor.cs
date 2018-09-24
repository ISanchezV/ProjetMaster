namespace GameCreator.Characters
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.AI;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using GameCreator.Core;

	[CustomEditor(typeof(Character), true)]
	public class CharacterEditor : Editor 
	{
		public class Section
		{
			private const string KEY_STATE = "character-section-{0}";

			public GUIContent name;
			public AnimBool state;

			public Section(string name, Texture2D icon, UnityAction repaint)
			{
				this.name = new GUIContent(string.Format(" {0}", name), icon);
				this.state = new AnimBool(this.GetState());
				this.state.speed = ANIM_BOOL_SPEED;
				this.state.valueChanged.AddListener(repaint);
			}

			public void PaintSection()
			{
				GUIStyle buttonStyle = (this.state.target
					? CoreGUIStyles.GetToggleButtonNormalOn()
					: CoreGUIStyles.GetToggleButtonNormalOff()
				);

				if (GUILayout.Button(this.name, buttonStyle))
				{
					this.state.target = !this.state.target;
					string key = string.Format(KEY_STATE, this.name.text.GetHashCode());
					EditorPrefs.SetBool(key, this.state.target);
				}
			}

			private bool GetState()
			{
				string key = string.Format(KEY_STATE, this.name.text.GetHashCode());
				return EditorPrefs.GetBool(key, true);
			}
		}

		// CONSTANTS: --------------------------------------------------------------------------------------------------

		public const string CHARACTER_ICONS_PATH = "Assets/Plugins/GameCreator/Characters/Icons/";
		protected const string CHARACTER_PREFAB_PATH = "Assets/Plugins/GameCreator/Characters/Prefabs/Character.prefab";

		private const float ANIM_BOOL_SPEED = 3f;
		private const string SECTION_CHAR_PARAMS1 = "Basic Parameters";
		private const string SECTION_CHAR_PARAMS2 = "Advanced Parameters";

		private const string PROP_CHAR_LOCOMOTION = "characterLocomotion";
		private const string PROP_MOVSYS = "movementSystem";
		private const string PROP_ISCONT = "isControllable";
		private const string PROP_CANRUN = "canRun";
		private const string PROP_WALKSP = "walkSpeed";
		private const string PROP_RUNSPD = "runSpeed";
		private const string PROP_CANJMP = "canJump";
		private const string PROP_JMPFRC = "jumpForce";
		private const string PROP_ANGSPD = "angularSpeed";
        
		private const string PROP_FACEDR = "faceDirection";
        private const string PROP_FACEDT = "faceDirectionTarget";
		private const string PROP_NAVMES = "canUseNavigationMesh";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		protected Character character;
		protected Section sectionProperties1;
		protected Section sectionProperties2;

		protected SerializedProperty spIsControllable;
		protected SerializedProperty spWalkSpeed;
		protected SerializedProperty spRunSpeed;
		protected SerializedProperty spAngularSpeed;
		protected SerializedProperty spCanRun;
		protected SerializedProperty spCanJump;
		protected SerializedProperty spJumpForce;

		protected SerializedProperty spFaceDirection;
        protected SerializedProperty spFaceDirectionTarget;
		protected SerializedProperty spUseNavmesh;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		protected void OnEnable()
		{
			SerializedProperty spCharLocomotion = serializedObject.FindProperty(PROP_CHAR_LOCOMOTION);
			this.character = (Character)target;

			string iconProperties1Path = Path.Combine(CHARACTER_ICONS_PATH, "CharacterBasicParameters.png");
			Texture2D iconProperties1 = AssetDatabase.LoadAssetAtPath<Texture2D>(iconProperties1Path);
			this.sectionProperties1 = new Section(SECTION_CHAR_PARAMS1, iconProperties1, this.Repaint);

			string iconPropertiesPath2 = Path.Combine(CHARACTER_ICONS_PATH, "CharacterAdvancedParameters.png");
			Texture2D iconProperties2 = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPropertiesPath2);
			this.sectionProperties2 = new Section(SECTION_CHAR_PARAMS2, iconProperties2, this.Repaint);

			this.spIsControllable = spCharLocomotion.FindPropertyRelative(PROP_ISCONT);
			this.spWalkSpeed      = spCharLocomotion.FindPropertyRelative(PROP_WALKSP);
			this.spRunSpeed       = spCharLocomotion.FindPropertyRelative(PROP_RUNSPD);
			this.spAngularSpeed   = spCharLocomotion.FindPropertyRelative(PROP_ANGSPD);
			this.spCanRun         = spCharLocomotion.FindPropertyRelative(PROP_CANRUN);
			this.spCanJump        = spCharLocomotion.FindPropertyRelative(PROP_CANJMP);
			this.spJumpForce      = spCharLocomotion.FindPropertyRelative(PROP_JMPFRC);

            this.spFaceDirection = spCharLocomotion.FindPropertyRelative(PROP_FACEDR);
            this.spFaceDirectionTarget = spCharLocomotion.FindPropertyRelative(PROP_FACEDT);
			this.spUseNavmesh = spCharLocomotion.FindPropertyRelative(PROP_NAVMES);
		}

		protected void OnDisable()
		{
			this.character = null;
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			this.PaintInspector();

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		public void PaintInspector()
		{
			this.PaintCharacterBasicProperties();
			this.PaintCharacterAdvancedProperties();
			this.PaintAnimatorComponent();
		}

		private void PaintCharacterBasicProperties()
		{
			this.sectionProperties1.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionProperties1.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.LabelField("Locomotion:", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.spIsControllable);
					EditorGUILayout.PropertyField(this.spCanRun);
					EditorGUILayout.PropertyField(this.spWalkSpeed);
					EditorGUILayout.PropertyField(this.spRunSpeed);
					EditorGUILayout.PropertyField(this.spAngularSpeed);
					EditorGUI.indentLevel--;

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Traversal:", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.spCanJump);
					EditorGUILayout.PropertyField(this.spJumpForce);
					EditorGUI.indentLevel--;

					EditorGUILayout.EndVertical();
				}
			}
		}

		private void PaintCharacterAdvancedProperties()
		{
			this.sectionProperties2.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionProperties2.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.LabelField("Locomotion:", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(this.spFaceDirection);
                    if (this.spFaceDirection.intValue == (int)CharacterLocomotion.FACE_DIRECTION.Target)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(this.spFaceDirectionTarget);
                        EditorGUI.indentLevel--;
                    }

					EditorGUILayout.PropertyField(this.spUseNavmesh);

					EditorGUILayout.EndVertical();
				}
			}
		}

		private void PaintAnimatorComponent()
		{
			if (!this.character.gameObject.GetComponent<CharacterAnimator>())
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUILayout.HelpBox("You might want to add a Character Animator component", MessageType.Info);
				if (GUILayout.Button("Add Character Animator"))
				{
					Undo.AddComponent<CharacterAnimator>(this.character.gameObject);
				}
				EditorGUILayout.EndVertical();
			}
		}

		// MENU ITEM: --------------------------------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Characters/Character", false, 0)]
		public static void CreateCharacter()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CHARACTER_PREFAB_PATH);
			if (prefab == null) return;

			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			instance = CreateSceneObject.Create(instance, true);
		}
	}
}