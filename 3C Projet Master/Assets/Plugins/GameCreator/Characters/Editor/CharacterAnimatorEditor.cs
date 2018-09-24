namespace GameCreator.Characters
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(CharacterAnimator))]
	public class CharacterAnimatorEditor : Editor 
	{
		private const string MSG_EMPTY_MODEL = "Drop a model from your project or load the default Character.";
		private const string PATH_DEFAULT_MODEL = "Assets/Plugins/GameCreator/Characters/Models/Character.fbx";
		private const string PATH_DEFAULT_RCONT = "Assets/Plugins/GameCreator/Characters/Animations/Controllers/Locomotion.controller";

		// PROPERTIES: ----------------------------------------------------------------------------

		private CharacterAnimator characterAnimator;
		private CharacterEditor.Section sectionAnimator;
		private CharacterEditor.Section sectionModel;
        private CharacterEditor.Section sectionIK;

        private SerializedProperty spMoveForwardSpeed;
        private SerializedProperty spMoveSidesSpeed;
        private SerializedProperty spMovementSpeed;
        private SerializedProperty spTargetLock;
		private SerializedProperty spVerticalSpeed;
		private SerializedProperty spIsGrounded;
		private SerializedProperty spJump;
        private SerializedProperty spFallForce;
        private SerializedProperty spNormalX;
        private SerializedProperty spNormalY;
        private SerializedProperty spNormalZ;

		private SerializedProperty spAnimator;
		private bool showChangeModel = false;

        private SerializedProperty spUseFootIK;
        private SerializedProperty spUseHandIK;
        private SerializedProperty spUseSmartHeadIK;

		// INITIALIZERS: --------------------------------------------------------------------------

		protected void OnEnable()
		{
			this.characterAnimator = (CharacterAnimator)this.target;

			string iconAnimatorPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimParams.png");
			Texture2D iconAnimator = AssetDatabase.LoadAssetAtPath<Texture2D>(iconAnimatorPath);
			this.sectionAnimator = new CharacterEditor.Section("Animator Parameters", iconAnimator, this.Repaint);

			string iconModelPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimModel.png");
			Texture2D iconModel = AssetDatabase.LoadAssetAtPath<Texture2D>(iconModelPath);
			this.sectionModel = new CharacterEditor.Section("Character Model", iconModel, this.Repaint);

            string iconIKPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimIK.png");
            Texture2D iconIK = AssetDatabase.LoadAssetAtPath<Texture2D>(iconIKPath);
            this.sectionIK = new CharacterEditor.Section("Inverse Kinematics", iconIK, this.Repaint);

            this.spMoveForwardSpeed = serializedObject.FindProperty("moveForwardSpeed");
            this.spMoveSidesSpeed = serializedObject.FindProperty("moveSidesSpeed");
            this.spMovementSpeed = serializedObject.FindProperty("movementSpeed");
            this.spTargetLock = serializedObject.FindProperty("targetLock");
			this.spVerticalSpeed = serializedObject.FindProperty("verticalSpeed");
			this.spIsGrounded = serializedObject.FindProperty("isGrounded");
			this.spJump = serializedObject.FindProperty("jump");
            this.spFallForce = serializedObject.FindProperty("fallForce");
			this.spNormalX = serializedObject.FindProperty("normalX");
            this.spNormalY = serializedObject.FindProperty("normalY");
            this.spNormalZ = serializedObject.FindProperty("normalZ");

			this.spAnimator = serializedObject.FindProperty("animator");

            this.spUseFootIK = serializedObject.FindProperty("useFootIK");
            this.spUseHandIK = serializedObject.FindProperty("useHandIK");
            this.spUseSmartHeadIK = serializedObject.FindProperty("useSmartHeadIK");
		}

		// INSPECTOR GUI: -------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			this.PaintAnimParams();
            this.PaintAnimModel();
            this.PaintAnimIK();

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		private void PaintAnimParams()
		{
			this.sectionAnimator.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionAnimator.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.PropertyField(this.spMoveForwardSpeed);
					EditorGUILayout.PropertyField(this.spMoveSidesSpeed);
                    EditorGUILayout.PropertyField(this.spMovementSpeed);
                    EditorGUILayout.PropertyField(this.spTargetLock);
					EditorGUILayout.PropertyField(this.spVerticalSpeed);

					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(this.spIsGrounded);
					EditorGUILayout.PropertyField(this.spNormalX);
                    EditorGUILayout.PropertyField(this.spNormalY);
                    EditorGUILayout.PropertyField(this.spNormalZ);

					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(this.spJump);
                    EditorGUILayout.PropertyField(this.spFallForce);

					EditorGUILayout.EndVertical();
				}
			}
		}

		private void PaintAnimModel()
		{
			this.sectionModel.PaintSection();
			using (var group = new EditorGUILayout.FadeGroupScope (this.sectionModel.state.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

					EditorGUILayout.PropertyField(this.spAnimator);

					if (this.spAnimator.objectReferenceValue == null)
					{
						EditorGUILayout.Space();
						EditorGUILayout.HelpBox(MSG_EMPTY_MODEL, MessageType.Warning);
						this.PaintChangeModel();
					}
					else
					{
						if (!this.showChangeModel && GUI.Button(this.GetButtonRect(), "Change Model"))
						{
							this.showChangeModel = true;
						}

						if (this.showChangeModel)
						{
							EditorGUILayout.Space();
							this.PaintChangeModel();
						}

						if (((Animator)this.spAnimator.objectReferenceValue).applyRootMotion)
						{
                            Animator reference = (Animator)this.spAnimator.objectReferenceValue;
                            reference.applyRootMotion = false;
                            this.spAnimator.objectReferenceValue = reference;

                            serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();
						}
					}

					EditorGUILayout.EndVertical();
				}
			}
		}

        private void PaintAnimIK()
        {
            this.sectionIK.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionIK.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(this.spUseFootIK);
                    EditorGUILayout.PropertyField(this.spUseHandIK);
                    EditorGUILayout.PropertyField(this.spUseSmartHeadIK);
                    EditorGUILayout.EndVertical();
                }
            }
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void PaintChangeModel()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GameObject prefabCustom = (GameObject)EditorGUILayout.ObjectField(
				"Drop Project Model", null, typeof(GameObject), true
			);

			if (prefabCustom != null)
			{
				this.LoadCharacter(prefabCustom);
			}

			Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			buttonRect = new Rect(
				buttonRect.x + EditorGUIUtility.labelWidth, buttonRect.y,
				buttonRect.width - EditorGUIUtility.labelWidth, buttonRect.height
			);

			if (GUI.Button(buttonRect, "Load Default Character")) 
			{
				GameObject prefabDefault = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_DEFAULT_MODEL);
				this.LoadCharacter(prefabDefault);
			}

			EditorGUILayout.EndVertical();
		}

		private Rect GetButtonRect()
		{
			Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			return new Rect(
				buttonRect.x + EditorGUIUtility.labelWidth, buttonRect.y,
				buttonRect.width - EditorGUIUtility.labelWidth, buttonRect.height
			);
		}

		private void LoadCharacter(GameObject prefab)
		{
			if (prefab == null) return;
			if (prefab.GetComponent<Animator>() == null) return;

			GameObject instance = Instantiate<GameObject>(prefab);
			instance.name = prefab.name;

			instance.transform.SetParent(this.characterAnimator.transform);
			instance.transform.localPosition = Vector3.zero;
			instance.transform.localRotation = Quaternion.identity;

			Animator instanceAnimator = instance.GetComponent<Animator>();
			RuntimeAnimatorController rc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PATH_DEFAULT_RCONT);
			instanceAnimator.runtimeAnimatorController = rc;

			if (this.spAnimator.objectReferenceValue != null)
			{
				Animator previous = (Animator)this.spAnimator.objectReferenceValue;
				DestroyImmediate(previous.gameObject);
			}

			this.spAnimator.objectReferenceValue = instanceAnimator;
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();

			this.showChangeModel = false;
		}
	}
}