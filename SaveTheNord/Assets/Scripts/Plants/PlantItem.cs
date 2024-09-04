using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plants
{
    public class PlantItem : MonoBehaviour
    {
        private TileUI.Seed _seed;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Button _plantButton;
        [SerializeField] private TMP_Text _itemName;
        private string _plantSoundName;

        public void UpdateInfo(TileUI.Seed seed)
        {
            _seed = seed;
            _itemImage.sprite = seed.Item.ItemSprite;
            UpdateAmount();
            seed.Item.OnAmountChanged += UpdateAmount;
            _plantSoundName = seed.PlantSoundName;
        }

        private void UpdateAmount()
        {
            _itemName.text = $"{_seed.Item.name} Left: {_seed.Item.Amount}";
            UpdateButtonColors();
        }
        
        void UpdateButtonColors()
        {
            var colors = _plantButton.colors;
            if (_seed.Item.Amount > 0)
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
            _plantButton.colors = colors;
        }

        public void PlantSeeds()
        {
            if (_seed.Item.Amount < 1)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }

            SoundManager.Instance.PlaySound(_plantSoundName);
            _seed.Item.Amount--;
            var plant = Instantiate(_seed.PlantPrefab, Stats.SelectedTile.transform).GetComponent<Plant>();
            Stats.SelectedTile.IsUsed = true;
            Stats.SelectedTile = null;
            switch (plant.PlantType)
            {
                case Plant.PlantTypes.Bush: Stats.BushesPlanted++; break;
                case Plant.PlantTypes.Tree: Stats.TreesPlanted++; break;
                case Plant.PlantTypes.Lily: Stats.LiliesPlanted++; break;
            }
        }
    }
}
