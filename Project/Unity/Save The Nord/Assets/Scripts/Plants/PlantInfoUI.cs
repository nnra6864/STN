using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Plants
{
    public class PlantInfoUI : MonoBehaviour
    {
        [SerializeField] private Plant _plantScript;
        [SerializeField] private Slider _healthSlider, _maturitySlider;
        [SerializeField] private TMP_Text _health, _maturity, _fertilized, _airPollutionReduction, _waterPollutionReduction;

        private void Awake()
        {
            _healthSlider.maxValue = _plantScript.StartingHealth;
            UpdateHealth();
            UpdateMaturity();
            UpdateIsFertilized();
            _airPollutionReduction.text = $"Air Pollution Reduction: {_plantScript.AirPollutionReduction}";
            _waterPollutionReduction.text = $"Water Pollution Reduction: {_plantScript.WaterPollutionReduction}";
            _plantScript.OnHealthChanged += UpdateHealth;
            _plantScript.OnMaturityChanged += UpdateMaturity;
            _plantScript.OnIsFertilizedChanged += UpdateIsFertilized;
        }

        void UpdateHealth()
        {
            _health.text = $"{_plantScript.Health}/{_plantScript.StartingHealth}";
            _healthSlider.value = _plantScript.Health;
        }
        void UpdateMaturity()
        {
            _maturity.text = $"{(int)(_plantScript.Maturity * 100)}%";
            _maturitySlider.value = _plantScript.Maturity * 100;
        }
        void UpdateIsFertilized()
        {
            _fertilized.color = _plantScript.IsFertilized
                ? new Color32(163, 190, 140, 255)
                : new Color32(191, 97, 106, 255);
        }
    }
}
