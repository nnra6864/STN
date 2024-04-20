using System;
using System.Collections;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Institutions
{
    public class BuyItem : MonoBehaviour
    {
        private InventoryItem _item;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _itemName, _itemPrice, _itemsLeft;
        
        public void UpdateInfo(InventoryItem item)
        {
            _item = item;
            _itemImage.sprite = item.ItemSprite;
            _itemName.text = item.Name;
            _itemPrice.text = $"${item.BuyPrice}";
            UpdateAmount();
            _item.OnAmountChanged += UpdateAmount;
            Stats.OnMoneyChanged += UpdateButtonColors;
        }

        void UpdateAmount()
        {
            _itemsLeft.text = $"{_item.Name} Left: {_item.Amount}";
            UpdateButtonColors();
        }

        void UpdateButtonColors()
        {
            var colors = _buyButton.colors;
            if (Stats.Money >= _item.BuyPrice)
            {
                colors.normalColor = new Color32(163, 190, 140, 255);
                colors.highlightedColor = new Color32(186, 217, 160, 255);
                colors.pressedColor = new Color32(219, 255, 188, 255);
                colors.selectedColor = new Color32(186, 217, 160, 255);
                colors.disabledColor = new Color32(176, 191, 163, 100);
            }
            else
            {
                colors.normalColor = new Color32(191, 97, 106, 255);
                colors.highlightedColor = new Color32(217, 110, 120, 255);
                colors.pressedColor = new Color32(255, 130, 142, 255);
                colors.selectedColor = new Color32(217, 110, 120, 255);
                colors.disabledColor = new Color32(191, 162, 165, 100);
            }
            _buyButton.colors = colors;
        }

        public void Buy()
        {
            if (Stats.Money < _item.BuyPrice)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }
            SoundManager.Instance.PlaySound("Buy");
            Stats.Money -= _item.BuyPrice;
            _item.Amount++;
        }

        private Coroutine _holdBuy;
        IEnumerator HoldBuy()
        {
            Buy();
            yield return new WaitForSeconds(0.25f);
            while (true)
            {
                Buy();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void OnButtonDown()
        {
            if (_holdBuy != null) StopCoroutine(_holdBuy);
            _holdBuy = StartCoroutine(HoldBuy());
        }

        public void OnButtonUp()
        {
            StopCoroutine(_holdBuy);
            _holdBuy = null;
        }
    }
}