using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NnUtils.Scripts;
using Plants;
using UnityEngine;
using static NnUtils.Scripts.Color;
using Color = UnityEngine.Color;

namespace Core
{
    public class Stats : MonoBehaviour
    {
        public delegate void StatChanged();

        public static bool IsSandbox;
        [SerializeField] Nord _nord;
        [SerializeField] private GameObject _planets, _fog;
        [SerializeField] private Win _win;
        [SerializeField] private PlantNumbers _vegetationGoals;
        public static bool HasSavedTheNord;
        public static Nord NordScript;
        public static PlantNumbers VegetationGoals;
        public static StatChanged OnNordExploded;
        [SerializeField] private Material _fogMaterial;
        [SerializeField] private Camera _camera;

        #region Tiles
        public static List<GameObject> GroundTiles = new();
        public static List<GameObject> WaterTiles = new();
        public static List<Renderer> WaterTileRenderers = new();
        #endregion

        #region Selected Objects
        static GameObject _selectedObject;
        public static GameObject SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (NordScript.IsDestroyed) return;
                if (_selectedObject == value) return;
                _selectedObject = value;
                SelectedInteract = value == null ? null : value.GetComponent<IInteract>();
                OnSelectedObjectChanged?.Invoke();
            }
        }
        public delegate void SelectedObjectChanged();
        public static event SelectedObjectChanged OnSelectedObjectChanged;
        public static IInteract SelectedInteract;
        static GameObject _selectedInfo;
        public static GameObject SelectedInfo
        {
            get => _selectedInfo;
            set
            {
                if (NordScript.IsDestroyed) return;
                if (_selectedInfo == value) return;
                if (_selectedInfo != null)
                    _selectedInfo.SetActive(false);

                _selectedInfo = value;
                if (_selectedInfo != null)
                    _selectedInfo.SetActive(true);
            }
        }

        private static Tile _selectedTile;

        public static Tile SelectedTile
        {
            get => _selectedTile;
            set
            {
                if (NordScript.IsDestroyed) return;
                if (_selectedTile == value) return;
                if (_selectedTile != null)
                    _selectedTile.DeSelectTile();
                _selectedTile = value;
            }
        }
        #endregion

        #region Time
        static float _timerTime;
        public static float TimerTime { get => _timerTime; set { _timerTime = value; OnTimerChanged?.Invoke(); } }
        public static event StatChanged OnTimerChanged;
        #endregion
        
        #region Pollution
        private static float _pollutionLevel;
        public static float TotalPollution, AveragePollution, HighestPollution;
        private static int _totalPollutionChanges;
        public static float PollutionLevel
        {
            get => _pollutionLevel;
            set
            {
                _pollutionLevel = value;
                HighestPollution = HighestPollution < value ? value : HighestPollution;
                TotalPollution += value;
                _totalPollutionChanges++;
                AveragePollution = TotalPollution / _totalPollutionChanges;
            }
        }
        
        [Tooltip("Idle Air Pollution Increase Per Second")]
        [SerializeField] private float _idleAirPollutionIncrease;
        public static StatChanged OnAirPollutionLevelChanged;
        private static float _airPollutionLevel;
        public static float TotalAirPollution, AverageAirPollution, HighestAirPollution;
        private static int _totalAirPollutionChanges;
        public static float AirPollutionLevel
        {
            get => _airPollutionLevel;
            set
            {
                _airPollutionLevel = Mathf.Clamp(value, 0, 100);
                HighestAirPollution = HighestAirPollution < value ? value : HighestAirPollution;
                TotalAirPollution += value;
                _totalAirPollutionChanges++;
                AverageAirPollution = TotalAirPollution / _totalAirPollutionChanges;
                PollutionLevel = (AirPollutionLevel + WaterPollutionLevel) / 2f;
                OnAirPollutionLevelChanged?.Invoke();
            }
        }

        [Tooltip("Idle Water Pollution Increase Per Second")]
        [SerializeField] private float _idleWaterPollutionIncrease;
        public static StatChanged OnWaterPollutionLevelChanged;
        private static float _waterPollutionLevel;
        public static float TotalWaterPollution, AverageWaterPollution, HighestWaterPollution;
        private static int _totalWaterPollutionChanges;
        public static float WaterPollutionLevel
        {
            get => _waterPollutionLevel;
            set
            {
                _waterPollutionLevel = Mathf.Clamp(value, 0, 100);
                HighestWaterPollution = HighestWaterPollution < value ? value : HighestWaterPollution;
                TotalWaterPollution += value;
                _totalWaterPollutionChanges++;
                AverageWaterPollution = TotalWaterPollution / _totalWaterPollutionChanges;
                PollutionLevel = (AirPollutionLevel + WaterPollutionLevel) / 2f;
                OnWaterPollutionLevelChanged?.Invoke();
            }
        }
        #endregion
        
        #region Money
        private static float _money;
        public static float Money
        {
            get => _money;
            set
            {
                if (IsSandbox) return;
                _money = value;
                OnMoneyChanged?.Invoke();
            }
        }
        public static event StatChanged OnMoneyChanged;
        #endregion
        
        #region Inventory
        [SerializeField] private List<InventoryItem> _resetItemAmounts;
        #endregion

        #region PlantNumbers
        public static Dictionary<Plant.PlantNames, PlantNumberItem> PlantNumbers;
        public static int BushesDestroyed, BushesPlanted;
        public static int TreesDestroyed, TreesPlanted;
        public static int LiliesDestroyed, LiliesPlanted;
        #endregion

        #region Settings
        public static float ColorUpdateSpeed;
        public static Color32 TileHoverColor;
        public static Color32 TileSelectColor = new(163, 190, 140, 255);
        #endregion

        private void Awake()
        {
            SoundManager.Instance.PlaySound("GameLoop");
            ResetStats();
        }

        private void Update()
        {
            if (HasSavedTheNord || _nord.IsDestroyed) return;
            TimerTime += Time.deltaTime;
            Raycast();
            UpdatePollution();
            if (AirPollutionLevel >= 100 || WaterPollutionLevel >= 100) _nord.ExplodeNord();
            else if (AirPollutionLevel <= 0.01f && WaterPollutionLevel <= 0.01f && HasEnoughPlants())
            {
                HasSavedTheNord = true;
                SelectedTile = null;
                StartCoroutine(ClearUpEffects());
            }
        }

        private void Raycast()
        {
            if (Misc.IsPointerOverUI)
            {
                if (SelectedObject != null) SelectedInteract?.MouseLeave();
                SelectedObject = null;
                return;
            }
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100))
            {
                var obj = hit.transform.gameObject;
                if (obj == SelectedObject) return;
                if (SelectedObject != obj) SelectedInteract?.MouseLeave();
                SelectedObject = obj;
                SelectedInteract?.MouseEnter();
            }
            else if (SelectedObject != null)
            {
                SelectedInteract?.MouseLeave();
                SelectedObject = null;
            }
        }

        private static readonly int _glossMapScale = Shader.PropertyToID("_Smoothness");
        private void UpdatePollution()
        {
            WaterPollutionLevel += _idleWaterPollutionIncrease * Time.deltaTime;
            AirPollutionLevel += _idleAirPollutionIncrease * Time.deltaTime;
            PollutionLevel = (AirPollutionLevel + WaterPollutionLevel) / 2f;
            var a = Mathf.Lerp(0, 0.05f, AirPollutionLevel / 100f);
            _fogMaterial.color = new Color(_fogMaterial.color.r, _fogMaterial.color.g, _fogMaterial.color.b, a);
            foreach (var rend in WaterTileRenderers)
            {
                rend.material.SetFloat(_glossMapScale, Mathf.Lerp(1, 0, WaterPollutionLevel / 100f));
            }
        }

        IEnumerator ClearUpEffects()
        {
            float lerpPos = 0;
            float fogA = _fogMaterial.color.a;
            while (lerpPos < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPos, easingType: Easings.Type.SineInOut);
                var a = Mathf.Lerp(fogA, 0, t);
                _fogMaterial.color = new Color(_fogMaterial.color.r, _fogMaterial.color.g, _fogMaterial.color.b, a);
                foreach (var rend in WaterTileRenderers)
                    rend.material.SetFloat(_glossMapScale, Mathf.Lerp(0, 1, t));
                yield return null;
            }
            _win.SavedTheNord();
        }

        bool HasEnoughPlants() =>
            PlantNumbers.All(pn => pn.Value.Amount >= pn.Value.Target);

        private void ResetStats()
        {
            IsSandbox = PlayerPrefs.GetInt("IsSandbox", 0) == 1;
            _fog.SetActive(PlayerPrefs.GetInt("ShowPollutionFog", 1) == 1);
            if (PlayerPrefs.GetInt("ShowPlanets", 1) == 0) Destroy(_planets);
            HasSavedTheNord = false;
            _nord.IsDestroyed = false;
            NordScript = _nord;
            VegetationGoals = _vegetationGoals;
            ColorUpdateSpeed = PlayerPrefs.GetFloat("TileTransitionTime", 0.25f);
            TileHoverColor =
                HexToRgba(PlayerPrefs.GetString("TileHoverHex", "EBCB8BFF"), new(235, 203, 139, 255));
            TileSelectColor =
                HexToRgba(PlayerPrefs.GetString("TileSelectHex", "88C0D0FF"), new(136, 192, 208, 255));
            TimerTime = 0;
            GroundTiles.Clear();
            WaterTiles.Clear();
            WaterTileRenderers.Clear();
            _totalPollutionChanges = _totalAirPollutionChanges = _totalWaterPollutionChanges = 0;
            TotalPollution = TotalAirPollution = TotalWaterPollution = 0;
            AveragePollution = HighestPollution = PollutionLevel = 0;
            AverageAirPollution = HighestAirPollution = AirPollutionLevel = Random.Range(10, 30);
            AverageWaterPollution = HighestWaterPollution = WaterPollutionLevel = Random.Range(10, 30);
            PollutionLevel = (AirPollutionLevel + WaterPollutionLevel) / 2f;
            OnMoneyChanged = null;
            _money = IsSandbox ? 69420 : Random.Range(0, 25);
            SelectedInfo = SelectedObject = null;
            OnNordExploded = null;
            
            foreach (var item in _resetItemAmounts)
            {
                item.OnAmountChanged = null;
                item.Amount = IsSandbox ? 69420 : 0;
            }
            
            PlantNumbers = new()
            {
                {Plant.PlantNames.NoakBush, new (0, 36)},
                {Plant.PlantNames.NoakLily, new (0, 8)},
                {Plant.PlantNames.NoakTree, new (0, 32)},
                {Plant.PlantNames.NirchBush, new (0, 24)},
                {Plant.PlantNames.NirchLily, new (0, 6)},
                {Plant.PlantNames.NirchTree, new (0, 16)},
                {Plant.PlantNames.AnaciaBush, new (0, 12)},
                {Plant.PlantNames.AnaciaLily, new (0, 4)},
                {Plant.PlantNames.AnaciaTree, new (0, 8)},
                {Plant.PlantNames.NordBush, new (0, 6)},
                {Plant.PlantNames.NordLily, new (0, 2)},
                {Plant.PlantNames.NordTree, new (0, 4)}
            };
            BushesDestroyed = BushesPlanted = TreesDestroyed = TreesPlanted = LiliesDestroyed = LiliesPlanted = 0;
        }
    }
}