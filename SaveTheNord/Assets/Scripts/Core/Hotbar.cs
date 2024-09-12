using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class Hotbar : MonoBehaviour
    {
        [System.Serializable]
        public enum Tools
        {
            Hand, //H1
            Shears, //S2
            Axe, //A3
            Workers, //W4
            Fertilizer, //F5
            WaterPurifier //P6
        }
        public delegate void ValueChanged();
        public static ValueChanged OnSelectedToolChanged;
        private static Tools _selectedTool = 0;
        public static Tools SelectedTool
        {
            get => _selectedTool;
            set
            {
                _selectedTool = value;
                OnSelectedToolChanged?.Invoke();
            }
        }
        [SerializeField] private InventoryItem _fertilizerItem, _waterPurifierItem;
        private static InventoryItem _fertilizer, _waterPurifier;
        public static int FertilizerCount
        {
            get => _fertilizer.Amount;
            set => _fertilizer.Amount = value;
        }
        public static int WaterPurifierCount
        {
            get => _waterPurifier.Amount;
            set => _waterPurifier.Amount = value;
        }

        private static int _workersCount;
        public static int WorkersCount
        {
            get => _workersCount;
            set
            {
                _workersCount = value;
                OnWorkersCountChanged?.Invoke();
            }
        }
        public static ValueChanged OnWorkersCountChanged;
        [SerializeField] private TMP_Text _fertilizerAmount, _waterPurifierAmount, _workersAmount;
        
        private void Awake()
        {
            OnSelectedToolChanged += SelectHotbarSlot;
            SelectedTool = Tools.Hand;
            SoundManager.Instance.StopSound("Select");
            _fertilizer = _fertilizerItem;
            _fertilizerAmount.text = $"{_fertilizer.Amount}";
            _fertilizer.OnAmountChanged += UpdateAmount;
            _waterPurifier = _waterPurifierItem;
            _waterPurifierAmount.text = $"{_waterPurifier.Amount}";
            _waterPurifier.OnAmountChanged += UpdateAmount;
            OnWorkersCountChanged += UpdateAmount;
            WorkersCount = Stats.IsSandbox ? 999999 : 0;
        }

        private void OnDestroy()
        {
            OnSelectedToolChanged -= SelectHotbarSlot;
        }

        void UpdateAmount()
        {
            _fertilizerAmount.text = $"{_fertilizer.Amount}";
            _waterPurifierAmount.text = $"{_waterPurifier.Amount}";
            _workersAmount.text = $"{WorkersCount}";
        }

        void SelectHotbarSlot()
        {
            int slot = 0;
            string selectSound = "Select";
            switch (SelectedTool)
            {
                case Tools.Hand:
                    slot = 0;
                    selectSound = "Select";
                    break;
                case Tools.Shears:
                    slot = 1;
                    selectSound = "Shears";
                    break;
                case Tools.Axe:
                    slot = 2;
                    selectSound = "AxeHit";
                    break;
                case Tools.Workers:
                    slot = 3;
                    selectSound = "WorkshopSelect";
                    break;
                case Tools.Fertilizer:
                    slot = 4;
                    selectSound = "Fertilize";
                    break;
                case Tools.WaterPurifier:
                    slot = 5;
                    selectSound = "PurifyWater";
                    break;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Image>().color = i == slot
                    ? new Color32(0, 0, 0, 200)
                    : new Color32(0, 0, 0, 100);
            }
            
            SoundManager.Instance.PlaySound(selectSound);
        }

        public void ChangeSelectedTool(int index)
        {
            if (index is < 0 or > 5) return;
            SelectedTool = (Tools)index;
        }
    }
}