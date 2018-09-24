namespace GameCreator.Quests
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class QuestConfigWindow : EditorWindow
    {
		private const int WINDOW_W = 400;
		private const int WINDOW_H = 300;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static QuestConfigWindow Instance { get; private set; }

		private IQuestEditor questEditor;
        private Vector2 scroll = Vector2.zero;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static void Open(IQuestEditor questEditor)
        {
            if (QuestConfigWindow.Instance != null) QuestConfigWindow.Instance.Close();

            Rect windowRect = new Rect(0, 0, WINDOW_W, WINDOW_H);
            QuestConfigWindow.Instance = EditorWindow.GetWindowWithRect<QuestConfigWindow>(
                windowRect, true, questEditor.GetInternalName(), true
            );

            QuestConfigWindow.Instance.questEditor = questEditor;
            QuestConfigWindow.Instance.Show();
        }

        public void OnGUI()
        {
            if (this.questEditor == null)
            {
                Close();
                return;
            }

            this.scroll = EditorGUILayout.BeginScrollView(
                this.scroll,
				EditorStyles.inspectorFullWidthMargins
            );

			this.questEditor.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();         

			this.Repaint();
        }
    }
}