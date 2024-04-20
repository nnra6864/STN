using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Institutions
{
    public class Ingredient : MonoBehaviour
    {
        private InventoryItem _item;
        [SerializeField] private Image _itemImage, _backgroundImage;
        [SerializeField] private TMP_Text _itemName, _itemsLeft;
        
        public void UpdateInfo(InventoryItem item)
        {
            _item = item;
            _itemImage.sprite = item.ItemSprite;
            _itemName.text = item.Name;
            UpdateAmount();
            _item.OnAmountChanged += UpdateAmount;
        }

        void UpdateAmount()
        {
            _itemsLeft.text = $"{_item.Name} Left: {_item.Amount}";
            _backgroundImage.color = _item.Amount > 0
                ? new Color32(163, 190, 140, 100)
                : new Color32(191, 97, 106, 100);
        }
    }
}
