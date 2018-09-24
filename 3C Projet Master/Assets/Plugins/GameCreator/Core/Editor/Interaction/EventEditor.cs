namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Event))]
	public class EventEditor : MultiSubEditor<InteractionEditor, Interaction>
	{
        private static Interaction CLIPBOARD_INTERACTION;

		private const string MSG_EMTPY_EVENTS = "There are no Interactions associated with this Event.";
		private const string MSG_REMOVE_TITLE = "Are you sure you want to delete this Interaction?";
		private const string MSG_REMOVE_DESCR = "All information associated with this Interaction will be lost.";
		private const string MSG_PREFAB_UNSUPPORTED = "<b>Game Creator</b> does not support creating <i>Prefabs</i> from <b>Events</b>";

		private const string PROP_INSTANCEID = "instanceID";
		private const string PROP_INTERACTIONS = "interactions";
		private const string PROP_DEFREAC = "defaultActions";

		private Event instance;
		private SerializedProperty spInteractions;
		private SerializedProperty spDefaultActions;

		private ActionsEditor actionsEditor;
		private EditorSortableList editorSortableList;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
            this.forceInitialize = true;

            this.instance = (Event)target;
            this.spInteractions = serializedObject.FindProperty(PROP_INTERACTIONS);
            this.spDefaultActions = serializedObject.FindProperty(PROP_DEFREAC);

            if (this.instance.defaultActions != null)
            {
                this.actionsEditor = Editor.CreateEditor(this.instance.defaultActions) as ActionsEditor;
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            this.UpdateSubEditors(this.instance.interactions);
            this.editorSortableList = new EditorSortableList();
		}

		protected override void Setup(InteractionEditor eventEditor, int editorIndex)
		{
			eventEditor.spInteractions = this.spInteractions;
			eventEditor.parentEvent = this.instance;
		}

		// INSPECTOR: --------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            if (target == null || serializedObject == null) return;

            serializedObject.Update();
			this.UpdateSubEditors(this.instance.interactions);

			this.PaintEvent();

			serializedObject.ApplyModifiedProperties();
		}

		private void PaintEvent()
		{
			if (this.spInteractions != null && this.spInteractions.arraySize > 0)
			{
				this.PaintInteractions();
			}
			else
			{
				EditorGUILayout.HelpBox(MSG_EMTPY_EVENTS, MessageType.None);
			}

            float widthAddInteraction = 100f;
            Rect rectControls = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
            Rect rectAddInteraction = new Rect(
                rectControls.x + (rectControls.width/2.0f) - (widthAddInteraction + 25f)/2.0f,
                rectControls.y,
                widthAddInteraction,
                rectControls.height
            );

            Rect rectPaste = new Rect(
                rectAddInteraction.x + rectAddInteraction.width,
                rectControls.y,
                25f,
                rectControls.height
            );

            if (GUI.Button(rectAddInteraction, "Add Interaction", CoreGUIStyles.GetButtonLeft()))
			{
				Interaction interactionCreated = this.instance.gameObject.AddComponent<Interaction>();

				int interactionCreatedIndex = this.spInteractions.arraySize;
				this.spInteractions.InsertArrayElementAtIndex(interactionCreatedIndex);
				this.spInteractions.GetArrayElementAtIndex(interactionCreatedIndex).objectReferenceValue = interactionCreated;

				this.AddSubEditorElement(interactionCreated, -1, true);

				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

            GUIContent gcPaste = InteractionUtilities.Get(InteractionUtilities.Icon.Paste);
            EditorGUI.BeginDisabledGroup(CLIPBOARD_INTERACTION == null);
            if (GUI.Button(rectPaste, gcPaste, CoreGUIStyles.GetButtonRight()))
            {
                Interaction copy = this.instance.gameObject.AddComponent<Interaction>();
                EditorUtility.CopySerialized(CLIPBOARD_INTERACTION, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    EventEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(InteractionEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedProperties();
                    soCopy.Update();
                }

                int interactionIndex = this.spInteractions.arraySize;
                this.spInteractions.InsertArrayElementAtIndex(interactionIndex);
                this.spInteractions.GetArrayElementAtIndex(interactionIndex).objectReferenceValue = copy;

                this.AddSubEditorElement(copy, -1, true);

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                DestroyImmediate(CLIPBOARD_INTERACTION.gameObject, true);
                CLIPBOARD_INTERACTION = null;

                /*
                int srcIndex = duplicateInteractionIndex;
                int dstIndex = duplicateInteractionIndex + 1;

                Interaction source = (Interaction)this.subEditors[srcIndex].target;
                Interaction copy = (Interaction)this.instance.gameObject.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    EventEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(InteractionEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedProperties();
                    soCopy.Update();
                }

                this.spInteractions.InsertArrayElementAtIndex(dstIndex);
                this.spInteractions.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                this.spInteractions.serializedObject.ApplyModifiedProperties();
                this.spInteractions.serializedObject.Update();

                this.AddSubEditorElement(copy, dstIndex, true);
                */
            }
            EditorGUI.EndDisabledGroup();

            GUIContent gcElse = InteractionUtilities.Get(InteractionUtilities.Icon.Else);
            Rect rectElse = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectElse, gcElse, EditorStyles.boldLabel);

			ActionsEditor.Return returnActions = ActionsEditor.PaintActionsGUI(
				this.instance.gameObject, 
				this.spDefaultActions,
				this.actionsEditor
			);

			if (returnActions != null)
			{
				this.spDefaultActions = returnActions.spParentActions;
				this.actionsEditor = returnActions.parentActionsEditor;

				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

			EditorGUILayout.Space();
		}

		private void PaintInteractions()
		{
			int removeInteractionIndex = -1;
            int duplicateInteractionIndex = -1;
            int copyInteractionIndex = -1;

			bool forceRepaint = false;
			int interactionsSize = this.spInteractions.arraySize;

			for (int i = 0; i < interactionsSize; ++i)
			{
				if (this.subEditors == null || i >= this.subEditors.Length || this.subEditors[i] == null) continue;

				bool repaint = this.editorSortableList.CaptureSortEvents(this.subEditors[i].handleDragRect, i);
				forceRepaint = repaint || forceRepaint;

				EditorGUILayout.BeginVertical();
                Rect rectHeader = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetToggleButtonNormalOn());
                this.PaintDragHandle(i, rectHeader);

				EditorGUIUtility.AddCursorRect(this.subEditors[i].handleDragRect, MouseCursor.Pan);
				string name = (this.isExpanded[i].target ? "▾ " : "▸ ") + this.instance.interactions[i].description;
				GUIStyle style = (this.isExpanded[i].target 
					? CoreGUIStyles.GetToggleButtonMidOn() 
					: CoreGUIStyles.GetToggleButtonMidOff()
				);

                Rect rectDelete = new Rect(
                    rectHeader.x + rectHeader.width - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectDuplicate = new Rect(
                    rectDelete.x - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectCopy = new Rect(
                    rectDuplicate.x - 25f,
                    rectHeader.y,
                    25f,
                    rectHeader.height
                );

                Rect rectMain = new Rect(
                    rectHeader.x + 25f,
                    rectHeader.y,
                    rectHeader.width - (25f * 4f),
                    rectHeader.height
                );

				if (GUI.Button(rectMain, name, style))
				{
                    this.ToggleExpand(i);
				}

                GUIContent gcCopy = InteractionUtilities.Get(InteractionUtilities.Icon.Copy);
                GUIContent gcDuplicate = InteractionUtilities.Get(InteractionUtilities.Icon.Duplicate);
                GUIContent gcDelete = InteractionUtilities.Get(InteractionUtilities.Icon.Delete);

                if (GUI.Button(rectCopy, gcCopy, CoreGUIStyles.GetButtonMid()))
                {
                    copyInteractionIndex = i;
                }

                if (GUI.Button(rectDuplicate, gcDuplicate, CoreGUIStyles.GetButtonMid()))
                {
                    duplicateInteractionIndex = i;
                }

                if (GUI.Button(rectDelete, gcDelete, CoreGUIStyles.GetButtonRight()))
				{
					if (EditorUtility.DisplayDialog(MSG_REMOVE_TITLE, MSG_REMOVE_DESCR, "Yes", "Cancel"))
					{
						removeInteractionIndex = i;
					}
				}

				using (var group = new EditorGUILayout.FadeGroupScope (this.isExpanded[i].faded))
				{
					if (group.visible)
					{
						EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
						this.subEditors[i].OnInteractionGUI();
						EditorGUILayout.EndVertical();
					}
				}

				EditorGUILayout.EndVertical();
				if (UnityEngine.Event.current.type == EventType.Repaint)
				{
					this.subEditors[i].interactionRect = GUILayoutUtility.GetLastRect();
				}

				this.editorSortableList.PaintDropPoints(this.subEditors[i].interactionRect, i, interactionsSize);
			}

            if (copyInteractionIndex >= 0)
            {
                Interaction source = (Interaction)this.subEditors[copyInteractionIndex].target;
                GameObject copyInstance = EditorUtility.CreateGameObjectWithHideFlags(
                    "Interaction (Copy)",
                    HideFlags.HideAndDontSave
                );

                CLIPBOARD_INTERACTION = (Interaction)copyInstance.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, CLIPBOARD_INTERACTION);

                if (CLIPBOARD_INTERACTION.conditionsList != null)
                {
                    IConditionsList conditionsListSource = CLIPBOARD_INTERACTION.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    EventEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(CLIPBOARD_INTERACTION);
                    soCopy.FindProperty(InteractionEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedProperties();
                    soCopy.Update();
                }
            }

            if (duplicateInteractionIndex >= 0)
            {
                int srcIndex = duplicateInteractionIndex;
                int dstIndex = duplicateInteractionIndex + 1;

                Interaction source = (Interaction)this.subEditors[srcIndex].target;
                Interaction copy = (Interaction)this.instance.gameObject.AddComponent(source.GetType());
                EditorUtility.CopySerialized(source, copy);

                if (copy.conditionsList != null)
                {
                    IConditionsList conditionsListSource = copy.conditionsList;
                    IConditionsList conditionsListCopy = this.instance.gameObject.AddComponent<IConditionsList>();

                    EditorUtility.CopySerialized(conditionsListSource, conditionsListCopy);
                    EventEditor.DuplicateIConditionList(conditionsListSource, conditionsListCopy);

                    SerializedObject soCopy = new SerializedObject(copy);
                    soCopy.FindProperty(InteractionEditor.PROP_CONDITIONSLIST).objectReferenceValue = conditionsListCopy;
                    soCopy.ApplyModifiedProperties();
                    soCopy.Update();
                }

                this.spInteractions.InsertArrayElementAtIndex(dstIndex);
                this.spInteractions.GetArrayElementAtIndex(dstIndex).objectReferenceValue = copy;

                this.spInteractions.serializedObject.ApplyModifiedProperties();
                this.spInteractions.serializedObject.Update();

                this.AddSubEditorElement(copy, dstIndex, true);
            }

			if (removeInteractionIndex >= 0)
			{
				this.subEditors[removeInteractionIndex].OnDestroyInteraction();
				Interaction rmInteraction = (Interaction)this.spInteractions
					.GetArrayElementAtIndex(removeInteractionIndex).objectReferenceValue;

				this.spInteractions.DeleteArrayElementAtIndex(removeInteractionIndex);
				this.spInteractions.RemoveFromObjectArrayAt(removeInteractionIndex);
				DestroyImmediate(rmInteraction, true);
			}

			EditorSortableList.SwapIndexes swapIndexes = this.editorSortableList.GetSortIndexes();
			if (swapIndexes != null)
			{
				this.spInteractions.MoveArrayElement(swapIndexes.src, swapIndexes.dst);
				this.MoveSubEditorsElement(swapIndexes.src, swapIndexes.dst);
			}

			if (forceRepaint) this.Repaint();
		}

        private void PaintDragHandle(int i, Rect rectHeader)
        {
            Rect rect = new Rect(
                rectHeader.x,
                rectHeader.y,
                25f,
                rectHeader.height
            );

            GUI.Label(rect, "=", CoreGUIStyles.GetButtonLeft());
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                this.subEditors[i].handleDragRect = rect;
            }
        }

        public static void DuplicateIConditionList(IConditionsList source, IConditionsList dest)
        {
            if (source == null || source.conditions == null || source.conditions.Length == 0) return;
            ICondition[] conditions = new ICondition[source.conditions.Length];

            for (int i = 0; i < source.conditions.Length; i++)
            {
                ICondition sourceAction = source.conditions[i];
                if (sourceAction == null) continue;
                conditions[i] = dest.gameObject.AddComponent(sourceAction.GetType()) as ICondition;
                EditorUtility.CopySerialized(sourceAction, conditions[i]);
            }

            dest.conditions = conditions;
        }

		// HIERARCHY CONTEXT MENU: -------------------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/Event", false, 0)]
		public static void CreateEvent()
		{
			GameObject eventAsset = CreateSceneObject.Create("Event");
			eventAsset.AddComponent<Event>();
		}
	}
}