namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
    using GameCreator.Core;

	[CustomEditor(typeof(NavigationMarker))]
	public class NavigationMarkerEditor : Editor 
	{
        private const string BTN_NAME = "Marker Labels {0}";
        private const string PROP_MARKER_COLOR = "color";
        private const string PROP_MARKER_LABEL = "label";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spMarkerColor;
        private SerializedProperty spMarkerLabel;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            this.spMarkerColor = serializedObject.FindProperty(PROP_MARKER_COLOR);
            this.spMarkerLabel = serializedObject.FindProperty(PROP_MARKER_LABEL);
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMarkerColor);
            EditorGUILayout.PropertyField(this.spMarkerLabel);

            string btnName = string.Format(
                BTN_NAME, 
                (NavigationMarker.LABEL_SHOW ? "OFF" : "ON")
            );

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniButton);
            btnRect = new Rect(
                btnRect.x + EditorGUIUtility.labelWidth,
                btnRect.y,
                btnRect.width - EditorGUIUtility.labelWidth,
                btnRect.height
            );

            if (GUI.Button(btnRect, btnName, EditorStyles.miniButton))
            {
                NavigationMarker.LABEL_SHOW = !NavigationMarker.LABEL_SHOW;
                EditorPrefs.SetBool(NavigationMarker.LABEL_KEY, NavigationMarker.LABEL_SHOW);

                this.Repaint();
                SceneView.RepaintAll();
            }

            serializedObject.ApplyModifiedProperties();
        }

		// HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Other/Marker", false, 0)]
        public static void CreateMarker()
		{
            GameObject marker = CreateSceneObject.Create("Marker");
			marker.AddComponent<NavigationMarker>();
			Selection.activeGameObject = marker;
		}
	}
}