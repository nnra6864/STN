using Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Institutions
{
    public class Workshop : MonoBehaviour
    {
        [SerializeField] private float _workerPrice;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Button _buyButton;

        private void Awake()
        {
            _price.text = $"${_workerPrice}";
            Stats.OnMoneyChanged += UpdateButton;
        }

        private void OnEnable()
        {
            UpdateButton();
        }

        private void UpdateButton()
        {
            var colors = _buyButton.colors;
            if (Stats.Money >= _workerPrice)
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

        public void BuyWorker()
        {
            if (Stats.Money < _workerPrice)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }
            
            SoundManager.Instance.PlaySound("BuyWorker");
            Stats.Money -= _workerPrice;
            Hotbar.WorkersCount++;
        }
    }
}