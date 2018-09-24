namespace GameCreator.Inventory
{
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using GameCreator.Core;

	[CustomEditor(typeof(Item))]
	public class ItemEditor : Editor 
	{
		private const float ANIM_BOOL_SPEED = 3.0f;
		private const string PATH_PREFAB_CONSUME = "Assets/Plugins/GameCreatorData/Inventory/Prefabs/Consumables/";
		private const string NAME_PREFAB_CONSUME = "consume.prefab";

		private const string PROP_UUID = "uuid";
		private const string PROP_NAME = "itemName";
		private const string PROP_DESCRIPTION = "itemDescription";
		private const string PROP_SPRITE = "sprite";
		private const string PROP_PREFAB = "prefab";
		private const string PROP_PRICE = "price";
		private const string PROP_MAXSTACK = "maxStack";
		private const string PROP_CONSUMABLE = "consumable";
		private const string PROP_ACTIONSLIST = "actionsList";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spUUID;
		private SerializedProperty spName;
		private SerializedProperty spDescription;
		private SerializedProperty spSprite;
		private SerializedProperty spPrefab;
		private SerializedProperty spPrice;
		private SerializedProperty spMaxStack;

		private SerializedProperty spConsumable;
		private SerializedProperty spActionsList;
		private IActionsListEditor actionsListEditor;

		private AnimBool animUnfold;

		// METHODS: ----------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
			this.spUUID = serializedObject.FindProperty(PROP_UUID);
			this.spName = serializedObject.FindProperty(PROP_NAME);
			this.spDescription = serializedObject.FindProperty(PROP_DESCRIPTION);
			this.spSprite = serializedObject.FindProperty(PROP_SPRITE);
			this.spPrefab = serializedObject.FindProperty(PROP_PREFAB);
			this.spPrice = serializedObject.FindProperty(PROP_PRICE);
			this.spMaxStack = serializedObject.FindProperty(PROP_MAXSTACK);

			this.spConsumable = serializedObject.FindProperty(PROP_CONSUMABLE);
			this.spActionsList = serializedObject.FindProperty(PROP_ACTIONSLIST);
			if (this.spActionsList.objectReferenceValue == null)
			{
				GameCreatorUtilities.CreateFolderStructure(PATH_PREFAB_CONSUME);
				string actionsPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(
					PATH_PREFAB_CONSUME, NAME_PREFAB_CONSUME)
				);

				GameObject sceneInstance = new GameObject("ConsumeActions");
				sceneInstance.AddComponent<Actions>();

				GameObject prefabInstance = PrefabUtility.CreatePrefab(actionsPath, sceneInstance);
				DestroyImmediate(sceneInstance);

				Actions prefabActions = prefabInstance.GetComponent<Actions>();
				prefabActions.destroyAfterFinishing = true;
				this.spActionsList.objectReferenceValue = prefabActions.actionsList;
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

			this.actionsListEditor = (IActionsListEditor)IActionsListEditor.CreateEditor(
				this.spActionsList.objectReferenceValue, typeof(IActionsListEditor)
			);

			this.animUnfold = new AnimBool(false);
			this.animUnfold.speed = ANIM_BOOL_SPEED;
			this.animUnfold.valueChanged.AddListener(this.Repaint);
		}

		public void OnDestroyItem()
		{
			if (this.spActionsList.objectReferenceValue != null)
			{
				IActionsList list = (IActionsList)this.spActionsList.objectReferenceValue;
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list.gameObject));
				AssetDatabase.SaveAssets();
			}
		}

		public override void OnInspectorGUI ()
		{
			EditorGUILayout.HelpBox(
				"This Item can only be edited in the Inventory section of the Preferences window", 
				MessageType.Info
			);

			if (GUILayout.Button("Open Preferences"))
			{
				PreferencesWindow.OpenWindow();
			}
		}

        public bool OnPreferencesWindowGUI(DatabaseInventoryEditor inventoryEditor, int index)
		{
			serializedObject.Update();
            inventoryEditor.searchText = inventoryEditor.searchText.ToLower();
			string spNameString = this.spName.FindPropertyRelative("content").stringValue;
			string spDescString = this.spDescription.FindPropertyRelative("content").stringValue;

            if (!string.IsNullOrEmpty(inventoryEditor.searchText) && 
                !spNameString.ToLower().Contains(inventoryEditor.searchText) && 
                !spDescString.ToLower().Contains(inventoryEditor.searchText))
			{
				return false;
			}

            bool result = this.PaintHeader(inventoryEditor, index);
			using (var group = new EditorGUILayout.FadeGroupScope (this.animUnfold.faded))
			{
				if (group.visible)
				{
					EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
					this.PaintContent();
					EditorGUILayout.EndVertical();
				}
			}

			serializedObject.ApplyModifiedProperties();
			return result;
		}

        private bool PaintHeader(DatabaseInventoryEditor inventoryEditor, int index)
		{
			bool removeItem = false;

			EditorGUILayout.BeginHorizontal();

            bool forceSortRepaint = false;
            if (inventoryEditor.itemsHandleRect.ContainsKey(index))
            {
                EditorGUIUtility.AddCursorRect(inventoryEditor.itemsHandleRect[index], MouseCursor.Pan);
                forceSortRepaint = inventoryEditor.editorSortableListItems.CaptureSortEvents(
                    inventoryEditor.itemsHandleRect[index], index
                );
            }

            if (forceSortRepaint) inventoryEditor.Repaint();

            GUILayout.Label("=", CoreGUIStyles.GetButtonLeft(), GUILayout.Width(25f));
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                Rect dragRect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.itemsHandleRect.ContainsKey(index))
                {
                    inventoryEditor.itemsHandleRect[index] = dragRect;
                }
                else
                {
                    inventoryEditor.itemsHandleRect.Add(index, dragRect);
                }
            }

            if (inventoryEditor.itemsHandleRectRow.ContainsKey(index))
            {
                inventoryEditor.editorSortableListItems.PaintDropPoints(
                    inventoryEditor.itemsHandleRectRow[index],
                    index,
                    inventoryEditor.spItems.arraySize
                );
            }

			string name = (this.animUnfold.target ? "▾ " : "▸ ");
			string spNameString = this.spName.FindPropertyRelative("content").stringValue;
			name += (string.IsNullOrEmpty(spNameString) ? "No-name" :  spNameString);

			GUIStyle style = (this.animUnfold.target
				? CoreGUIStyles.GetToggleButtonMidOn() 
				: CoreGUIStyles.GetToggleButtonMidOff()
			);

			if (GUILayout.Button(name, style))
			{
				this.animUnfold.target = !this.animUnfold.value;
			}

			if (GUILayout.Button("×", CoreGUIStyles.GetButtonRight(), GUILayout.Width(25)))
			{
				removeItem = true;
			}

			EditorGUILayout.EndHorizontal();
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                if (inventoryEditor.itemsHandleRectRow.ContainsKey(index))
                {
                    inventoryEditor.itemsHandleRectRow[index] = rect;
                }
                else
                {
                    inventoryEditor.itemsHandleRectRow.Add(index, rect);
                }
            }

			return removeItem;
		}

		private void PaintContent()
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(this.spUUID);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(this.spName);
			EditorGUILayout.PropertyField(this.spDescription);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.spSprite);
			EditorGUILayout.PropertyField(this.spPrefab);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(this.spPrice);
			EditorGUILayout.PropertyField(this.spMaxStack);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spConsumable);

			EditorGUI.BeginDisabledGroup(!this.spConsumable.boolValue);
			if (this.actionsListEditor != null)
			{
				this.actionsListEditor.OnInspectorGUI();
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}