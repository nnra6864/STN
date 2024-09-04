using System.Collections;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Institutions
{
    public class SellItem : MonoBehaviour
    {
        private InventoryItem _item;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Button _sellButton, _sellAllButton;
        [SerializeField] private TMP_Text _itemName, _itemPrice, _itemsLeft;
        
        public void UpdateInfo(InventoryItem item)
        {
            _item = item;
            _itemImage.sprite = item.ItemSprite;
            _itemName.text = item.Name;
            _itemPrice.text = $"${item.SellPrice}";
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
            var colors = _sellButton.colors;
            if (_item.Amount > 0)
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
            _sellAllButton.colors = _sellButton.colors = colors;
        }
        
        public void Sell()
        {
            if (_item.Amount < 1)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }
            SoundManager.Instance.PlaySound("Sell");
            _item.Amount--;
            Stats.Money += _item.SellPrice;
        }
        
        private Coroutine _holdSell;
        IEnumerator HoldSell()
        {
            Sell();
            yield return new WaitForSeconds(0.25f);
            while (true)
            {
                Sell();
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void SellAll()
        {
            if (_item.Amount < 1)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }
            SoundManager.Instance.PlaySound("Sell");
            Stats.Money += _item.SellPrice * _item.Amount;
            _item.Amount = 0;
        }

        public void OnButtonDown()
        {
            if (_holdSell != null) StopCoroutine(_holdSell);
            _holdSell = StartCoroutine(HoldSell());
        }

        public void OnButtonUp()
        {
            StopCoroutine(_holdSell);
            _holdSell = null;
        }
    }
}