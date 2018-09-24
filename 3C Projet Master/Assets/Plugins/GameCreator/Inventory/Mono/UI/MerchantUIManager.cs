namespace GameCreator.Inventory
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;
	using UnityEngine.UI;
	using GameCreator.Core;

	public class MerchantUIManager : MonoBehaviour
	{
        private static MerchantUIManager Instance;
        private static DatabaseInventory DATABASE_INVENTORY;

        private const string DEFAULT_UI_PATH = "GameCreator/Inventory/MerchantUI";

		// PROPERTIES: ----------------------------------------------------------------------------

        public ScrollRect container;
        public Text textTitle;
        public Text textDescription;

		[Space]
        public GameObject itemUIPrefab;

        [HideInInspector]
        public Merchant currentMerchant;

		private Animator merchantAnimator;
		private GameObject merchantRoot;
		private bool isOpen = false;

        [Space]
        public UnityEvent onSelect = new UnityEvent();
        public UnityEvent onBuy = new UnityEvent();
        public UnityEvent onCantBuy = new UnityEvent();

        private Dictionary<int, MerchantUIItem> merchantItems;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void Awake()
		{
            MerchantUIManager.Instance = this;
            this.merchantItems = new Dictionary<int, MerchantUIItem>();

			if (this.container == null)
			{
				this.container.GetComponentInChildren<ScrollRect>();
			}

			if (transform.childCount >= 1) 
			{
				this.merchantRoot = transform.GetChild(0).gameObject;
                this.merchantAnimator = this.merchantRoot.GetComponent<Animator>();
			}
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void Open(Merchant merchant)
		{
            this.currentMerchant = merchant;
            this.ChangeState(true);

			if (DATABASE_INVENTORY.inventorySettings.inventoryStopTime)
			{
				Time.timeScale = 0.0f;
				Time.fixedDeltaTime = 0.0f;
			}

            this.UpdateItems(merchant);

            if (this.textTitle != null) this.textTitle.text = merchant.title;
            if (this.textDescription != null) this.textDescription.text = merchant.description;

            InventoryManager.Instance.eventChangePlayerCurrency.AddListener(this.UpdateItems);
		}

		public void Close()
		{
			if (!this.isOpen) return;

			if (DATABASE_INVENTORY.inventorySettings.inventoryStopTime)
			{
				Time.timeScale = 1.0f;
				Time.fixedDeltaTime = 0.02f;
			}

			this.ChangeState(false);
            InventoryManager.Instance.eventChangePlayerCurrency.RemoveListener(this.UpdateItems);
		}

		// STATIC METHODS: ------------------------------------------------------------------------

        public static void OpenMerchant(Merchant merchant)
		{
            MerchantUIManager.RequireInstance(merchant);
            MerchantUIManager.Instance.Open(merchant);
		}

		public static void CloseMerchant()
		{
            MerchantUIManager.RequireInstance(null);
			MerchantUIManager.Instance.Close();
		}

        private static void RequireInstance(Merchant merchant)
		{
            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = DatabaseInventory.Load();
            if (MerchantUIManager.Instance == null)
			{
				EventSystemManager.Instance.Wakeup();
				if (DATABASE_INVENTORY.inventorySettings == null)
				{
                    Debug.LogError("No inventory database found");
					return;
                }

                GameObject prefab = merchant.merchantUI;
                if (prefab == null) prefab = Resources.Load<GameObject>(DEFAULT_UI_PATH);

				Instantiate(prefab, Vector3.zero, Quaternion.identity);
			}
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void ChangeState(bool toOpen)
		{
			if (this.merchantRoot == null) 
			{
				Debug.LogError("Unable to find merchantRoot");
				return;
			}

			this.isOpen = toOpen;

            if (this.merchantAnimator == null)
			{
				this.merchantRoot.SetActive(toOpen);
				return;
			}

			switch (toOpen)
			{
                case true  : this.merchantAnimator.SetTrigger("Show"); break;
                case false : this.merchantAnimator.SetTrigger("Hide"); break;
			}
		}

        private void UpdateItems()
        {
            this.UpdateItems(this.currentMerchant);
        }

        private void UpdateItems(Merchant merchant)
		{
            for (int i = this.container.content.childCount - 1; i >= 0; --i)
            {
                Destroy(this.container.content.GetChild(i).gameObject);
            }

            this.merchantItems = new Dictionary<int, MerchantUIItem>();

            for (int i = 0; i < merchant.warehouse.wares.Length; ++i)
            {
                if (this.merchantItems.ContainsKey(merchant.warehouse.wares[i].item.item.uuid)) continue;

                GameObject instance = Instantiate(this.itemUIPrefab, this.container.content);
                MerchantUIItem item = instance.GetComponent<MerchantUIItem>();
                this.merchantItems.Add(merchant.warehouse.wares[i].item.item.uuid, item);
                item.Setup(merchant.warehouse.wares[i], this);
            }
		}
	}
}