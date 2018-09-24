namespace GameCreator.Inventory
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
	using GameCreator.Localization;

	#if UNITY_EDITOR
	using UnityEditor;
	using System.IO;
	#endif

	[System.Serializable]
	public class Item : ScriptableObject
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		public int uuid = -1;
        [LocStringNoPostProcess] public LocString itemName = new LocString();
        [LocStringNoPostProcess] public LocString itemDescription = new LocString();

		public Sprite sprite;
		public GameObject prefab;

		public int price = 0;
		public int maxStack = 99;

		public bool consumable = true;
		public IActionsList actionsList;

		// CONSTRUCTOR: ------------------------------------------------------------------------------------------------

		#if UNITY_EDITOR
		
        public static Item CreateItemInstance()
		{
			Item item = ScriptableObject.CreateInstance<Item>();
			Guid guid = Guid.NewGuid();

			item.name = "item." + Mathf.Abs(guid.GetHashCode());
			item.uuid = Mathf.Abs(guid.GetHashCode());

			item.itemName = new LocString();
			item.itemDescription = new LocString();
			item.price = 1;
			item.maxStack = 99;
			item.hideFlags = HideFlags.HideInHierarchy;

			DatabaseInventory databaseInventory = DatabaseInventory.Load();
			AssetDatabase.AddObjectToAsset(item, databaseInventory);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(item));
			return item;
		}

		private void OnDestroy()
		{
            if (this.actionsList == null) return;
            GameObject prefabAction = this.actionsList.gameObject;
            if (prefabAction != null) DestroyImmediate(prefabAction, true);
		}

        #endif
	}
}