using UnityEngine;

namespace Core
{
    [CreateAssetMenu]
    public class InventoryItem : ScriptableObject
    {
        public string Name;
        public float BuyPrice, SellPrice;
        
        private int _amount;
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnAmountChanged?.Invoke();
            }
        }
        public delegate void AmountChanged();
        public AmountChanged OnAmountChanged;

        public Sprite ItemSprite;
    }
}
