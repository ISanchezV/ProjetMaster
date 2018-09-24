namespace GameCreator.Localization
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEditor;
	using UnityEditor.UI;
	using GameCreator.Core;

	[CustomEditor(typeof(TextLocalized), true)]
	[CanEditMultipleObjects]
	public class TextLocalizedEditor : UnityEditor.UI.TextEditor
	{
		private SerializedProperty spText;
		private SerializedProperty spLocString;
		private SerializedProperty spLocStringContent;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.spLocString = serializedObject.FindProperty("locString");
			this.spLocStringContent = this.spLocString.FindPropertyRelative("content");

			this.spText = base.serializedObject.FindProperty("m_Text");

			if (!Application.isPlaying)
			{
				this.spText.stringValue = this.spLocStringContent.stringValue;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(this.spLocString);
			base.OnInspectorGUI();

			if (this.spLocStringContent.stringValue != this.spText.stringValue)
			{
				this.spLocStringContent.stringValue = this.spText.stringValue;
			}

			serializedObject.ApplyModifiedProperties();
		}

		// STATIC METHODS: ---------------------------------------------------------------------------------------------

		[MenuItem("GameObject/UI/Text Localized", false, 0)]
		public static void CreateTextLocalized()
		{
			GameObject text = CreateSceneObject.Create("Text Localized");
			text.AddComponent<TextLocalized>();

            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;
		}
	}
}