namespace GameCreator.Inventory
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using GameCreator.Core;

	public class InventoryUIManager : MonoBehaviour
	{
		private static InventoryUIManager Instance;
		private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "GameCreator/Inventory/InventoryRPG";

		// PROPERTIES: ----------------------------------------------------------------------------

		public ScrollRect itemsContaineScrollRect;
		public Text currencyText;

		[Space]
		public Image floatingItem;
        public GameObject itemUIPrefab;

		private CanvasScaler canvasScaler;
		private RectTransform floatingItemRT;
		private Animator inventoryAnimator;
		private GameObject inventoryRoot;
		private bool isOpen = false;

		private Dictionary<int, InventoryUIItem> currentItems;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void Awake()
		{
			InventoryUIManager.Instance = this;
			this.currentItems = new Dictionary<int, InventoryUIItem>();

			if (this.itemsContaineScrollRect == null)
			{
				this.itemsContaineScrollRect.GetComponentInChildren<ScrollRect>();
			}

			if (transform.childCount >= 1) 
			{
				this.inventoryRoot = transform.GetChild(0).gameObject;
				this.inventoryAnimator = this.inventoryRoot.GetComponent<Animator>();
			}

			if (this.floatingItem != null) this.floatingItemRT = this.floatingItem.GetComponent<RectTransform>();
			InventoryUIManager.OnDragItem(null, false);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void Open()
		{
			if (this.isOpen) return;

			this.ChangeState(true);
			if (DATABASE_INVENTORY.inventorySettings.inventoryStopTime)
			{
				Time.timeScale = 0.0f;
				Time.fixedDeltaTime = 0.0f;
			}

			InventoryManager.Instance.eventChangePlayerInventory.AddListener(this.UpdateItems);
			InventoryManager.Instance.eventChangePlayerCurrency.AddListener(this.UpdateCurrency);

			this.UpdateItems();
			this.UpdateCurrency();
		}

		public void Close()
		{
			if (!this.isOpen) return;

			if (DATABASE_INVENTORY.inventorySettings.inventoryStopTime)
			{
				Time.timeScale = 1.0f;
				Time.fixedDeltaTime = 0.02f;
			}

			InventoryManager.Instance.eventChangePlayerInventory.RemoveListener(this.UpdateItems);
			InventoryManager.Instance.eventChangePlayerCurrency.RemoveListener(this.UpdateCurrency);
			this.ChangeState(false);
		}

		// STATIC METHODS: ------------------------------------------------------------------------

		public static void OpenInventory()
		{
			InventoryUIManager.RequireInstance();
			InventoryUIManager.Instance.Open();
		}

		public static void CloseInventory()
		{
			InventoryUIManager.RequireInstance();
			InventoryUIManager.Instance.Close();
		}

		private static void RequireInstance()
		{
			if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.LoadDatabase<DatabaseInventory>();
			if (InventoryUIManager.Instance == null)
			{
				EventSystemManager.Instance.Wakeup();
				if (DATABASE_INVENTORY.inventorySettings == null)
				{
                    Debug.LogError("No inventory database found");
					return;
                }

                GameObject prefab = DATABASE_INVENTORY.inventorySettings.inventoryUIPrefab;
                if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

				Instantiate(prefab, Vector3.zero, Quaternion.identity);
			}
		}

		public static void OnDragItem(Sprite sprite, bool dragging)
		{
			InventoryUIManager.Instance.floatingItem.gameObject.SetActive(dragging);
			if (!dragging) return;

			InventoryUIManager.Instance.floatingItem.sprite = sprite;

			Vector2 position = InventoryUIManager.Instance.GetPonterPositionUnscaled(Input.mousePosition);
			InventoryUIManager.Instance.floatingItemRT.anchoredPosition = position;
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void ChangeState(bool toOpen)
		{
			if (this.inventoryRoot == null) 
			{
				Debug.LogError("Unable to find inventoryRoot");
				return;
			}

			this.isOpen = toOpen;

			if (this.inventoryAnimator == null)
			{
				this.inventoryRoot.SetActive(toOpen);
				return;
			}

			switch (toOpen)
			{
			case true  : this.inventoryAnimator.SetTrigger("Show"); break;
			case false : this.inventoryAnimator.SetTrigger("Hide"); break;
			}
		}

		private void UpdateItems()
		{
			Dictionary<int, InventoryUIItem> remainingItems = new Dictionary<int, InventoryUIItem>(this.currentItems);

			foreach(KeyValuePair<int, int> entry in InventoryManager.Instance.playerInventory.items)
			{
				Item currentItem = InventoryManager.Instance.itemsCatalogue[entry.Key];
				int currentItemAmount = InventoryManager.Instance.playerInventory.items[currentItem.uuid];
				if (currentItemAmount <= 0) continue;

				if (this.currentItems != null && this.currentItems.ContainsKey(currentItem.uuid))
				{
					this.currentItems[currentItem.uuid].UpdateUI(currentItem, currentItemAmount);
					remainingItems.Remove(currentItem.uuid);
				}
				else
				{
					GameObject itemUIPrefab = this.itemUIPrefab;
					if (itemUIPrefab == null)
					{
						string error = "No inventory item UI prefab found. Fill the required field at {0}";
						string errorPath = "GameCreator/Preferences and head to Inventory -> Settings";
						Debug.LogErrorFormat(error, errorPath);
						return;
					}

					GameObject itemUIAsset = Instantiate(itemUIPrefab, this.itemsContaineScrollRect.content);
					InventoryUIItem itemUI = itemUIAsset.GetComponent<InventoryUIItem>();
					itemUI.Setup(currentItem, currentItemAmount);
					this.currentItems.Add(currentItem.uuid, itemUI);
				}
			}

			foreach(KeyValuePair<int, InventoryUIItem> entry in remainingItems)
			{
				this.currentItems.Remove(entry.Key);
				Destroy(entry.Value.gameObject);
			}
		}

		private void UpdateCurrency()
		{
			if (this.currencyText == null) return;
			this.currencyText.text = InventoryManager.Instance.GetCurrency().ToString();
		}

		private Vector2 GetPonterPositionUnscaled(Vector2 mousePosition)
		{
			if (this.canvasScaler == null) this.canvasScaler = transform.GetComponentInParent<CanvasScaler>();
			if (this.canvasScaler == null) return mousePosition;

			Vector2 referenceResolution = this.canvasScaler.referenceResolution;
			Vector2 currentResolution = new Vector2(Screen.width, Screen.height);

			float widthRatio = currentResolution.x / referenceResolution.x;
			float heightRatio = currentResolution.y / referenceResolution.y;
			float ratio = Mathf.Lerp(widthRatio, heightRatio, this.canvasScaler.matchWidthOrHeight);

			return mousePosition/ratio;
		}
	}
}