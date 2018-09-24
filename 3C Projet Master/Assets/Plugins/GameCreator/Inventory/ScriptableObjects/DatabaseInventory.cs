namespace GameCreator.Inventory
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

	public class DatabaseInventory : IDatabase
	{
		[System.Serializable]
		public class InventoryCatalogue
		{
			public Item[] items;
			public Recipe[] recipes;
		}

		[System.Serializable]
		public class InventorySettings
		{
			public GameObject inventoryUIPrefab;
            public bool onDragGrabItem = true;

			public Texture2D cursorDrag;
			public Vector2 cursorDragHotspot;

            [Tooltip("Allow to execute a Recipe dropping an item onto another one")]
            public bool dragItemsToCombine = true;

            [Tooltip("Check if you want to pause the game when opening the Inventory menu")]
            public bool inventoryStopTime = false;

            [Tooltip("Check if you want to drop items dragging them out of the Inventory menu")]
            public bool canDropItems = true;

            [Tooltip("Max distance an item can be dropped from the Player")]
            public float dropItemMaxDistance = 2.0f;
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public InventoryCatalogue inventoryCatalogue;
		public InventorySettings inventorySettings;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public List<int> GetItemSuggestions(string hint)
		{
			hint = hint.ToLower();
			List<int> suggestions = new List<int>();
			for (int i = 0; i < this.inventoryCatalogue.items.Length; ++i)
			{
				if (this.inventoryCatalogue.items[i].itemName.content.ToLower().Contains(hint) ||
					this.inventoryCatalogue.items[i].itemDescription.content.ToLower().Contains(hint))
				{
					suggestions.Add(i);
				}
			}

			return suggestions;
		}

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseInventory Load()
        {
            return IDatabase.LoadDatabase<DatabaseInventory>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseInventory>();
        }

		protected override string GetProjectPath()
		{
            return "Assets/Plugins/GameCreatorData/Inventory/Resources";
		}

        #endif
	}
}