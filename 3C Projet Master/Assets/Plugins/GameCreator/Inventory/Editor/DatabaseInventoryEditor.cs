namespace GameCreator.Inventory
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using System.Linq;
	using System.Reflection;
	using GameCreator.Core;

	[CustomEditor(typeof(DatabaseInventory))]
	public class DatabaseInventoryEditor : IDatabaseEditor
	{
		private const string PROP_INVENTORY_CATALOGUE = "inventoryCatalogue";
		private const string PROP_INVENTORY_CATALOGUE_ITEMS = "items";
		private const string PROP_INVENTORY_CATALOGUE_RECIPE = "recipes";

		private const string PROP_INVENTORY_SETTINGS = "inventorySettings";
		private const string PROP_INVENTORY_UI_PREFAB = "inventoryUIPrefab";
		private const string PROP_INVENTORY_ONDRAG_GRABITEM = "onDragGrabItem";
		private const string PROP_ITEM_CURSOR_DRAG = "cursorDrag";
		private const string PROP_ITEM_CURSOR_DRAG_HS = "cursorDragHotspot";

		private const string PROP_ITEM_DRAG_TO_COMBINE = "dragItemsToCombine";
        private const string PROP_STOPTIME_ONOPEN = "inventoryStopTime";
		private const string PROP_DROP_ITEM_OUTSIDE = "canDropItems";
        private const string PROP_DROP_MAX_DISTANCE = "dropItemMaxDistance";

		private const string MSG_EMPTY_CATALOGUE = "There are no items. Add one clicking the 'Create Item' button";
		private const string MSG_EMPTY_RECIPES = "There are no recipes. Add one clicking the 'Create Recipe' button";

		private const string SEARCHBOX_NAME = "searchbox";

		private class ItemsData
		{
			public ItemEditor cachedEditor;
			public SerializedProperty spItem;

			public ItemsData(SerializedProperty item)
			{
				this.spItem = item;

				Editor cachedEditor = this.cachedEditor;
				Editor.CreateCachedEditor(item.objectReferenceValue, typeof(ItemEditor), ref cachedEditor);
				this.cachedEditor = (ItemEditor)cachedEditor;
			}
		}

		private class RecipeData
		{
			public RecipeEditor cachedEditor;
			public SerializedProperty spRecipe;

            public RecipeData(SerializedProperty recipe)
			{
                this.spRecipe = recipe;

				Editor cachedEditor = this.cachedEditor;
				Editor.CreateCachedEditor(recipe.objectReferenceValue, typeof(RecipeEditor), ref cachedEditor);
				this.cachedEditor = (RecipeEditor)cachedEditor;
			}
		}

		private static readonly GUIContent[] TAB_NAMES = new GUIContent[]
		{
			new GUIContent("Catalogue"),
			new GUIContent("Recipes"),
			new GUIContent("Settings")
		};

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private int tabIndex = 0;

		public SerializedProperty spItems;
        public SerializedProperty spRecipes;

		private SerializedProperty spInventoryUIPrefab;
		private SerializedProperty spItemOnDragGrabItem;
		private SerializedProperty spItemCursorDrag;
		private SerializedProperty spItemCursorDragHotspot;

		private SerializedProperty spItemDragToCombine;
        private SerializedProperty spInventoryStopTime;
        private SerializedProperty spCanDropItems;
        private SerializedProperty spDropMaxDistance;

		private List<ItemsData> itemsData;
		private List<RecipeData> recipesData;

		private GUIStyle searchFieldStyle;
		private GUIStyle searchCloseOnStyle;
		private GUIStyle searchCloseOffStyle;

		public string searchText = "";
		public bool searchFocus = true;

        public EditorSortableList editorSortableListItems;
        public EditorSortableList editorSortableListRecipes;

        public Dictionary<int, Rect> itemsHandleRect = new Dictionary<int, Rect>();
        public Dictionary<int, Rect> recipesHandleRect = new Dictionary<int, Rect>();

        public Dictionary<int, Rect> itemsHandleRectRow = new Dictionary<int, Rect>();
        public Dictionary<int, Rect> recipesHandleRectRow = new Dictionary<int, Rect>();

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
			SerializedProperty spInventoryCatalogue = serializedObject.FindProperty(PROP_INVENTORY_CATALOGUE);
			this.spItems = spInventoryCatalogue.FindPropertyRelative(PROP_INVENTORY_CATALOGUE_ITEMS);
			this.spRecipes = spInventoryCatalogue.FindPropertyRelative(PROP_INVENTORY_CATALOGUE_RECIPE);

			SerializedProperty spInventorySettings = serializedObject.FindProperty(PROP_INVENTORY_SETTINGS);
			this.spInventoryUIPrefab = spInventorySettings.FindPropertyRelative(PROP_INVENTORY_UI_PREFAB);
			this.spItemOnDragGrabItem = spInventorySettings.FindPropertyRelative(PROP_INVENTORY_ONDRAG_GRABITEM);
			this.spItemCursorDrag = spInventorySettings.FindPropertyRelative(PROP_ITEM_CURSOR_DRAG);
			this.spItemCursorDragHotspot = spInventorySettings.FindPropertyRelative(PROP_ITEM_CURSOR_DRAG_HS);

			this.spItemDragToCombine = spInventorySettings.FindPropertyRelative(PROP_ITEM_DRAG_TO_COMBINE);
			this.spInventoryStopTime = spInventorySettings.FindPropertyRelative(PROP_STOPTIME_ONOPEN);
			this.spCanDropItems = spInventorySettings.FindPropertyRelative(PROP_DROP_ITEM_OUTSIDE);
            this.spDropMaxDistance = spInventorySettings.FindPropertyRelative(PROP_DROP_MAX_DISTANCE);

			int itemsSize = this.spItems.arraySize;
			this.itemsData = new List<ItemsData>();
			for (int i = 0; i < itemsSize; ++i)
			{
				this.itemsData.Add(new ItemsData(this.spItems.GetArrayElementAtIndex(i)));
			}

			int recipesSize = this.spRecipes.arraySize;
			this.recipesData = new List<RecipeData>();
			for (int i = 0; i < recipesSize; ++i)
			{
				this.recipesData.Add(new RecipeData(this.spRecipes.GetArrayElementAtIndex(i)));
			}

            this.editorSortableListItems = new EditorSortableList();
            this.editorSortableListRecipes = new EditorSortableList();
		}

		// OVERRIDE METHODS: -------------------------------------------------------------------------------------------

		public override string GetName ()
		{
			return "Inventory";
		}

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI ()
		{
			this.OnPreferencesWindowGUI();
		}

		public override void OnPreferencesWindowGUI()
		{
			this.serializedObject.Update();

			int prevTabIndex = this.tabIndex;
			this.tabIndex = GUILayout.Toolbar(this.tabIndex, TAB_NAMES);
			if (prevTabIndex != this.tabIndex) this.ResetSearch();

			EditorGUILayout.Space();

			switch (this.tabIndex)
			{
			case 0 : this.PaintCatalogue(); break;
			case 1 : this.PaintRecipes(); break;
			case 2 : this.PaintSettings(); break;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private void PaintCatalogue()
		{
			int removeIndex = -1;
			this.PaintSearch();

			int itemsCatalogueSize = this.spItems.arraySize;
			if (itemsCatalogueSize == 0)
			{
				EditorGUILayout.HelpBox(MSG_EMPTY_CATALOGUE, MessageType.Info);
			}

			for (int i = 0; i < itemsCatalogueSize; ++i)
			{
				if (this.itemsData[i].cachedEditor == null) continue;
				bool removeItem = this.itemsData[i].cachedEditor.OnPreferencesWindowGUI(this, i);
				if (removeItem) removeIndex = i;
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Create Item", GUILayout.MaxWidth(200)))
			{
				this.ResetSearch();

				int insertIndex = itemsCatalogueSize;
				this.spItems.InsertArrayElementAtIndex(insertIndex);

				Item item = Item.CreateItemInstance();
				this.spItems.GetArrayElementAtIndex(insertIndex).objectReferenceValue = item;
				this.itemsData.Insert(insertIndex, new ItemsData(this.spItems.GetArrayElementAtIndex(insertIndex)));
			}

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			if (removeIndex != -1)
			{
				this.itemsData[removeIndex].cachedEditor.OnDestroyItem();
				UnityEngine.Object deleteItem = this.itemsData[removeIndex].cachedEditor.target;
				this.spItems.RemoveFromObjectArrayAt(removeIndex);
				this.itemsData.RemoveAt(removeIndex);

				string path = AssetDatabase.GetAssetPath(deleteItem);
				DestroyImmediate(deleteItem, true);
				AssetDatabase.ImportAsset(path);
			}

            EditorSortableList.SwapIndexes swapIndexes = this.editorSortableListItems.GetSortIndexes();
            if (swapIndexes != null)
            {
                this.spItems.MoveArrayElement(swapIndexes.src, swapIndexes.dst);

                ItemsData tempItem = this.itemsData[swapIndexes.src];
                this.itemsData[swapIndexes.src] = this.itemsData[swapIndexes.dst];
                this.itemsData[swapIndexes.dst] = tempItem;
            }
		}

		private void PaintRecipes()
		{
			int removeIndex = -1;

			int recipeCatalogueSize = this.spRecipes.arraySize;
			if (recipeCatalogueSize == 0)
			{
				EditorGUILayout.HelpBox(MSG_EMPTY_RECIPES, MessageType.Info);
			}

			for (int i = 0; i < recipeCatalogueSize; ++i)
			{
				bool removeRecipe = this.recipesData[i].cachedEditor.OnPreferencesWindowGUI(this, i);
				if (removeRecipe) removeIndex = i;
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Create Recipe", GUILayout.MaxWidth(200)))
			{
				this.ResetSearch();

				int insertIndex = recipeCatalogueSize;
				this.spRecipes.InsertArrayElementAtIndex(insertIndex);

				Recipe recipe = Recipe.CreateRecipeInstance();
				this.spRecipes.GetArrayElementAtIndex(insertIndex).objectReferenceValue = recipe;
				this.recipesData.Insert(insertIndex, new RecipeData(this.spRecipes.GetArrayElementAtIndex(insertIndex)));
			}

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			if (removeIndex != -1)
			{
				this.recipesData[removeIndex].cachedEditor.OnDestroyRecipe();
				UnityEngine.Object deleteRecipe = this.recipesData[removeIndex].cachedEditor.target;

				this.spRecipes.RemoveFromObjectArrayAt(removeIndex);
				this.recipesData.RemoveAt(removeIndex);

				string path = AssetDatabase.GetAssetPath(deleteRecipe);
				DestroyImmediate(deleteRecipe, true);
				AssetDatabase.ImportAsset(path);
			}

            EditorSortableList.SwapIndexes swapIndexes = this.editorSortableListRecipes.GetSortIndexes();
            if (swapIndexes != null)
            {
                this.spRecipes.MoveArrayElement(swapIndexes.src, swapIndexes.dst);

                RecipeData tempRecipt = this.recipesData[swapIndexes.src];
                this.recipesData[swapIndexes.src] = this.recipesData[swapIndexes.dst];
                this.recipesData[swapIndexes.dst] = tempRecipt;
            }
		}

		private void PaintSettings()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			EditorGUILayout.LabelField("User Interface", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(this.spInventoryUIPrefab);
			EditorGUILayout.PropertyField(this.spItemOnDragGrabItem);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spItemCursorDrag);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(this.spItemCursorDragHotspot.displayName);
			EditorGUILayout.PropertyField(this.spItemCursorDragHotspot, GUIContent.none);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Behavior Configuration", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(this.spItemDragToCombine);
			EditorGUILayout.PropertyField(this.spInventoryStopTime);
			EditorGUILayout.PropertyField(this.spCanDropItems);
            EditorGUI.BeginDisabledGroup(!this.spCanDropItems.boolValue);
            EditorGUILayout.PropertyField(this.spDropMaxDistance);
            EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private void PaintSearch()
		{
			if (this.searchFieldStyle == null) this.searchFieldStyle = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
			if (this.searchCloseOnStyle == null) this.searchCloseOnStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
			if (this.searchCloseOffStyle == null) this.searchCloseOffStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButtonEmpty"));

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(5f);

			GUI.SetNextControlName(SEARCHBOX_NAME);
			this.searchText = EditorGUILayout.TextField(this.searchText, this.searchFieldStyle);

			if (this.searchFocus)
			{
				EditorGUI.FocusTextInControl(SEARCHBOX_NAME);
				this.searchFocus = false;
			}

			GUIStyle style = (string.IsNullOrEmpty(this.searchText) 
				? this.searchCloseOffStyle 
				: this.searchCloseOnStyle
			);

			if (GUILayout.Button("", style)) 
			{
				this.ResetSearch();
			}

			GUILayout.Space(5f);
			EditorGUILayout.EndHorizontal();
		}

		private void ResetSearch()
		{
			this.searchText = "";
			GUIUtility.keyboardControl = 0;
			EditorGUIUtility.keyboardControl = 0;
			this.searchFocus = true;
		}
	}
}