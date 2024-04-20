using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Institutions
{
    public class Store : MonoBehaviour
    {
        [SerializeField] private Image _buyButtonImage, _sellButtonImage;
        [SerializeField] private GameObject _buyPanel, _sellPanel;
        [SerializeField] private GameObject _buyScrollContent, _sellScrollContent;
        [SerializeField] private List<InventoryItem> _buyItems, _sellItems;
        [SerializeField] private GameObject _buyItemPrefab, _sellItemPrefab;

        private void Awake()
        {
            foreach (var item in _buyItems)
            {
                var buyItem = Instantiate(_buyItemPrefab, _buyScrollContent.transform);
                buyItem.GetComponent<BuyItem>().UpdateInfo(item);
            }
            foreach (var item in _sellItems)
            {
                var sellItem = Instantiate(_sellItemPrefab, _sellScrollContent.transform);
                sellItem.GetComponent<SellItem>().UpdateInfo(item);
            }
        }

        public void EnableBuy()
        {
            if (_buyPanel.activeSelf) return;
            _sellPanel.SetActive(false);
            _buyPanel.SetActive(true);
            _buyButtonImage.color = new Color32(0, 0, 0, 100);
            _sellButtonImage.color = new Color32(0, 0, 0, 0);
        }
        
        public void EnableSell()
        {
            if (_sellPanel.activeSelf) return;
            _sellPanel.SetActive(true);
            _buyPanel.SetActive(false);
            _buyButtonImage.color = new Color32(0, 0, 0, 0);
            _sellButtonImage.color = new Color32(0, 0, 0, 100);
        }
    }
}