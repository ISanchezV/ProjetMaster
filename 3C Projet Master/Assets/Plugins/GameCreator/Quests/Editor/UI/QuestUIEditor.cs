namespace GameCreator.Quests.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(QuestUI))]
    public class QuestUIEditor : Editor
    {
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public QuestUIUtilities.Section sectionGeneral;
        private SerializedProperty spTitle;
        private SerializedProperty spImage;
        private SerializedProperty spDescriptionType;
        private SerializedProperty spDescription;

        public QuestUIUtilities.Section sectionButtons;
        private SerializedProperty spToggleTrack;
        private SerializedProperty spButtonActivate;
        private SerializedProperty spButtonAbandon;

        public QuestUIUtilities.Section sectionStates;
        private SerializedProperty spStatusTracking;
        private SerializedProperty spStatusInactive;
        private SerializedProperty spStatusActive;
        private SerializedProperty spStatusComplete;
        private SerializedProperty spStatusFailed;
        private SerializedProperty spStatusAbandoned;

        public QuestUIUtilities.Section sectionSubTasks;
        private SerializedProperty spSubTasks;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            this.sectionGeneral = new QuestUIUtilities.Section(
                "General",
                "General.png",
                this.Repaint
            );

            this.spTitle = serializedObject.FindProperty("title");
            this.spImage = serializedObject.FindProperty("image");
            this.spDescriptionType = serializedObject.FindProperty("descriptionType");
            this.spDescription = serializedObject.FindProperty("description");

            this.sectionButtons = new QuestUIUtilities.Section(
                "Buttons",
                "Buttons.png",
                this.Repaint
            );

            this.spToggleTrack = serializedObject.FindProperty("toggleTrack");
            this.spButtonActivate = serializedObject.FindProperty("buttonActivate");
            this.spButtonAbandon = serializedObject.FindProperty("buttonAbandon");

            this.sectionStates = new QuestUIUtilities.Section(
                "States",
                "States.png",
                this.Repaint
            );

            this.spStatusTracking = serializedObject.FindProperty("statusTracking");
            this.spStatusInactive = serializedObject.FindProperty("statusInactive");
            this.spStatusActive = serializedObject.FindProperty("statusActive");
            this.spStatusComplete = serializedObject.FindProperty("statusComplete");
            this.spStatusFailed = serializedObject.FindProperty("statusFailed");
            this.spStatusAbandoned = serializedObject.FindProperty("statusAbandoned");

            this.sectionSubTasks = new QuestUIUtilities.Section(
                "Tasks",
                "SubTasks.png",
                this.Repaint
            );

            this.spSubTasks = serializedObject.FindProperty("subQuestsGroup");
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            this.PaintGeneral();
            this.PaintButtons();
            this.PaintStates();
            this.PaintSubTasks();

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private void PaintGeneral()
        {
            this.sectionGeneral.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionGeneral.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(this.spTitle);
                    EditorGUILayout.PropertyField(this.spImage);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this.spDescriptionType);
                    EditorGUILayout.PropertyField(this.spDescription);

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintButtons()
        {
            this.sectionButtons.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionButtons.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(this.spToggleTrack);
                    EditorGUILayout.PropertyField(this.spButtonActivate);
                    EditorGUILayout.PropertyField(this.spButtonAbandon);

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintStates()
        {
            this.sectionStates.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionStates.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(this.spStatusTracking);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(this.spStatusInactive);
                    EditorGUILayout.PropertyField(this.spStatusActive);
                    EditorGUILayout.PropertyField(this.spStatusComplete);
                    EditorGUILayout.PropertyField(this.spStatusFailed);
                    EditorGUILayout.PropertyField(this.spStatusAbandoned);

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintSubTasks()
        {
            this.sectionSubTasks.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(this.sectionSubTasks.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(this.spSubTasks);

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}