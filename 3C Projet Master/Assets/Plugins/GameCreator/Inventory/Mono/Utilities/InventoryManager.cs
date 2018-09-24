namespace GameCreator.Inventory
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("Game Creator/Managers/Inventory Manager", 100)]
	public class InventoryManager : Singleton<InventoryManager>, IGameSave
	{
		[System.Serializable]
		public class PlayerInventory
		{
			public Dictionary<int, int> items;
			public int currencyAmount;

			public PlayerInventory()
			{
				this.items = new Dictionary<int, int>();
				this.currencyAmount = 0;
			}
		}

		[System.Serializable]
		private class InventorySaveData
		{
			public int[] playerItemsUUIDS;
			public int[] playerItemsStack;
			public int playerCurrencyAmount;

			public InventorySaveData()
			{
				this.playerItemsUUIDS = new int[0];
				this.playerItemsStack = new int[0];
				this.playerCurrencyAmount = 0;
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public PlayerInventory playerInventory {private set; get;}
		public Dictionary<int, Item> itemsCatalogue {private set; get;}
		public Dictionary<Recipe.Key, Recipe> recipes {private set; get;}

		[HideInInspector] public UnityEvent eventChangePlayerInventory;
		[HideInInspector] public UnityEvent eventChangePlayerCurrency;

		// INITIALIZE: ----------------------------------------------------------------------------

		protected override void OnCreate ()
		{
			DatabaseInventory dbInventory = DatabaseInventory.LoadDatabase<DatabaseInventory>();
			this.eventChangePlayerInventory = new UnityEvent();
			this.eventChangePlayerCurrency = new UnityEvent();

			this.itemsCatalogue = new Dictionary<int, Item>();
			for (int i = 0; i < dbInventory.inventoryCatalogue.items.Length; ++i)
			{
				this.itemsCatalogue.Add(
					dbInventory.inventoryCatalogue.items[i].uuid, 
					dbInventory.inventoryCatalogue.items[i]
				);
			}

			this.recipes = new Dictionary<Recipe.Key, Recipe>();
			for (int i = 0; i < dbInventory.inventoryCatalogue.recipes.Length; ++i)
			{
				this.recipes.Add(
					new Recipe.Key(
						dbInventory.inventoryCatalogue.recipes[i].itemToCombineA.item.uuid,
						dbInventory.inventoryCatalogue.recipes[i].itemToCombineB.item.uuid
					),
					dbInventory.inventoryCatalogue.recipes[i]
				);
			}

			this.playerInventory = new PlayerInventory();
			SaveLoadManager.Instance.Initialize(this);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public bool AddItemToInventory(int uuid, int amount = 1)
		{
			if (!this.itemsCatalogue.ContainsKey(uuid))
			{
				Debug.LogError("Could not find item UUID in item catalogue");
				return false;
			}

			if (this.playerInventory.items.ContainsKey(uuid)) this.playerInventory.items[uuid] += amount;
			else this.playerInventory.items.Add(uuid, amount);

			if (this.playerInventory.items[uuid] > this.itemsCatalogue[uuid].maxStack)
			{
				this.playerInventory.items[uuid] = this.itemsCatalogue[uuid].maxStack;
			}

			if (this.eventChangePlayerInventory != null) this.eventChangePlayerInventory.Invoke();
			return true;
		}

		public bool SubstractItemFromInventory(int uuid, int amount = 1)
		{
			if (!this.itemsCatalogue.ContainsKey(uuid))
			{
				Debug.LogError("Could not find item UUID in item catalogue");
				return false;
			}

			if (this.playerInventory.items.ContainsKey(uuid)) 
			{
				this.playerInventory.items[uuid] -= amount;
				if (this.playerInventory.items[uuid] <= 0)
				{
					this.playerInventory.items.Remove(uuid);
				}

				if (this.eventChangePlayerInventory != null) this.eventChangePlayerInventory.Invoke();
				return true;
			}

			return false;
		}

		public bool ConsumeItem(int uuid)
		{
			Item item = this.itemsCatalogue[uuid];

			if (item == null) return false;
			if (!item.consumable) return false;
			if (item.actionsList.isExecuting) return false;
			if (this.GetInventoryAmountOfItem(uuid) <= 0) return false;

			this.SubstractItemFromInventory(item.uuid, 1);

			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			if (HookPlayer.Instance != null)
			{
				position = HookPlayer.Instance.transform.position;
				rotation = HookPlayer.Instance.transform.rotation;
			}

			GameObject instance = Instantiate<GameObject>(item.actionsList.gameObject, position, rotation);
			Actions actions = instance.GetComponent<Actions>();
            actions.Execute(HookPlayer.Instance.gameObject ?? gameObject);
			return true;
		}

		public int GetInventoryAmountOfItem(int uuid)
		{
			if (!this.itemsCatalogue.ContainsKey(uuid))
			{
				Debug.LogError("Could not find item UUID in item catalogue");
				return 0;
			}

			if (this.playerInventory.items.ContainsKey(uuid)) 
			{
				return this.playerInventory.items[uuid];
			}

			return 0;
		}

		public bool BuyItem(int uuid, int amount)
		{
			if (!this.itemsCatalogue.ContainsKey(uuid))
			{
				Debug.LogError("Could not find item UUID in item catalogue");
				return false;
			}

			int price = this.itemsCatalogue[uuid].price * amount;
			if (price > this.playerInventory.currencyAmount) return false;

			bool couldAdd = this.AddItemToInventory(uuid, amount);
			if (couldAdd) this.SubstractCurrency(price);

			return couldAdd;
		}

		public bool SellItem(int uuid, int amount)
		{
			if (!this.itemsCatalogue.ContainsKey(uuid))
			{
				Debug.LogError("Could not find item UUID in item catalogue");
				return false;
			}

			if (this.GetInventoryAmountOfItem(uuid) < amount) return false;
			this.SubstractItemFromInventory(uuid, amount);

			int price = this.itemsCatalogue[uuid].price * amount;
			this.AddCurrency(price);

			return true;
		}

		public void AddCurrency(int amount)
		{
			this.playerInventory.currencyAmount += amount;
			if (this.eventChangePlayerCurrency != null) this.eventChangePlayerCurrency.Invoke();
		}

		public void SubstractCurrency(int amount)
		{
			this.playerInventory.currencyAmount -= amount;
			this.playerInventory.currencyAmount = Mathf.Max(0, this.playerInventory.currencyAmount);
			if (this.eventChangePlayerCurrency != null) this.eventChangePlayerCurrency.Invoke();
		}

		public int GetCurrency()
		{
			return this.playerInventory.currencyAmount;
		}

		// RECIPES: -------------------------------------------------------------------------------

		public bool ExistsRecipe(int uuid1, int uuid2)
		{
			bool order1 = this.recipes.ContainsKey(new Recipe.Key(uuid1, uuid2));
			bool order2 = this.recipes.ContainsKey(new Recipe.Key(uuid2, uuid1));
			return (order1 || order2);
		}

		public bool UseRecipe(int uuid1, int uuid2)
		{
			if (!this.ExistsRecipe(uuid1, uuid2)) return false;
			Recipe recipe = this.recipes[new Recipe.Key(uuid1, uuid2)];
			if (recipe == null) recipe = this.recipes[new Recipe.Key(uuid2, uuid1)];
			if (recipe == null) return false;
			if (recipe.actionsList.isExecuting) return false;

			if (recipe.itemToCombineA.item.uuid == uuid2 && recipe.itemToCombineB.item.uuid == uuid1)
			{
				int auxiliar = uuid1;
				uuid1 = uuid2;
				uuid2 = auxiliar;
			}

			if (this.GetInventoryAmountOfItem(uuid1) < recipe.amountA ||
				this.GetInventoryAmountOfItem(uuid2) < recipe.amountB)
			{
				return false;
			}

			if (recipe.removeItemsOnCraft)
			{
				this.SubstractItemFromInventory(recipe.itemToCombineA.item.uuid, recipe.amountA);
				this.SubstractItemFromInventory(recipe.itemToCombineB.item.uuid, recipe.amountB);
			}

			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			if (HookPlayer.Instance != null)
			{
				position = HookPlayer.Instance.transform.position;
				rotation = HookPlayer.Instance.transform.rotation;
			}

			GameObject instance = Instantiate<GameObject>(recipe.actionsList.gameObject, position, rotation);
			Actions actions = instance.GetComponent<Actions>();
			actions.Execute();
			return true;
		}

		// INTERFACE ISAVELOAD: -------------------------------------------------------------------

		public string GetUniqueName()
		{
			return "inventory";
		}

		public System.Type GetSaveDataType()
		{
			return typeof(InventorySaveData);
		}

		public System.Object GetSaveData()
		{
			InventorySaveData inventorySaveData = new InventorySaveData();
			if (this.playerInventory != null)
			{
				if (this.playerInventory.items != null && this.playerInventory.items.Count > 0)
				{
					int playerInventoryItemsCount = this.playerInventory.items.Count;
					inventorySaveData.playerItemsUUIDS = new int[playerInventoryItemsCount];
					inventorySaveData.playerItemsStack = new int[playerInventoryItemsCount];

					int itemIndex = 0;
					foreach(KeyValuePair<int, int> entry in this.playerInventory.items)
					{
						inventorySaveData.playerItemsUUIDS[itemIndex] = entry.Key;
						inventorySaveData.playerItemsStack[itemIndex] = entry.Value;
						++itemIndex;
					}
				}

				inventorySaveData.playerCurrencyAmount = this.playerInventory.currencyAmount;
			}
				
			return inventorySaveData;
		}

		public void ResetData()
		{
			this.playerInventory = new PlayerInventory();
		}

		public void OnLoad(System.Object generic)
		{
			InventorySaveData inventorySaveData = (InventorySaveData)generic;

			this.playerInventory = new PlayerInventory();
			this.playerInventory.currencyAmount = inventorySaveData.playerCurrencyAmount;

			int playerInventoryItemsCount = inventorySaveData.playerItemsUUIDS.Length;
			for (int i = 0; i < playerInventoryItemsCount; ++i)
			{
				this.playerInventory.items.Add(
					inventorySaveData.playerItemsUUIDS[i],
					inventorySaveData.playerItemsStack[i]
				);
			}
		}
	}
}