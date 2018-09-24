namespace GameCreator.Inventory
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using GameCreator.Core;
    using GameCreator.Core.Hooks;

    public class MerchantUIItem : MonoBehaviour 
	{
		private static DatabaseInventory DATABASE_INVENTORY;

        // PROPERTIES: ----------------------------------------------------------------------------

        private MerchantUIManager merchantUIManager;
        private Merchant.Ware ware;

		[SerializeField] private Image image;
		[SerializeField] private Text textName;
		[SerializeField] private Text textDescription;
        [SerializeField] private Text textPrice;

        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject wrapAmount;
        [SerializeField] private Text textCurrentAmount;
        [SerializeField] private Text textMaxAmount;

		// CONSTRUCTOR & UPDATER: -----------------------------------------------------------------

        public void Setup(Merchant.Ware ware, MerchantUIManager merchantUIManager)
		{
            this.merchantUIManager = merchantUIManager;
            this.UpdateUI(ware);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void UpdateUI(Merchant.Ware ware)
		{
            this.ware = ware;
            Item item = this.ware.item.item;

            int curAmount = MerchantManager.Instance.GetMerchantAmount(
                this.merchantUIManager.currentMerchant,
                ware.item.item
            );

			if (this.image != null && item.sprite != null) this.image.sprite = item.sprite;
            if (this.textName != null) this.textName.text = item.itemName.GetText();
            if (this.textDescription != null) this.textDescription.text = item.itemDescription.GetText();
            if (this.textPrice != null) this.textPrice.text = item.price.ToString();

            this.wrapAmount.SetActive(ware.limitAmount);
            if (this.textCurrentAmount != null)
            {
                this.textCurrentAmount.text = curAmount.ToString();
            }
            if (this.textMaxAmount != null) this.textMaxAmount.text = ware.maxAmount.ToString();

            this.canvasGroup.interactable = (
                InventoryManager.Instance.GetCurrency() >= item.price && 
                (!ware.limitAmount || curAmount > 0)
            );
		}

		public void OnBuy()
		{
            Merchant merchant = this.merchantUIManager.currentMerchant;
            if (MerchantManager.Instance.BuyFromMerchant(merchant, this.ware.item.item, 1))
            {
                this.UpdateUI(this.ware);
                if (this.merchantUIManager.onBuy != null)
                {
                    this.merchantUIManager.onBuy.Invoke();
                }
            }
            else
            {
                if (this.merchantUIManager.onCantBuy != null)
                {
                    this.merchantUIManager.onCantBuy.Invoke();
                }
            }
		}

        public void OnSelect()
        {
            if (this.merchantUIManager.onSelect != null)
            {
                this.merchantUIManager.onSelect.Invoke();
            }
        }
	}
}