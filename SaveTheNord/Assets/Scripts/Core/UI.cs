using System.Collections;
using NnUtils.Scripts;
using Plants;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Core
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private GameObject _uiScaler;
        [SerializeField] private Drops _drops;
        public static Drops Drops;
        [SerializeField] private TMP_Text _pollution, _airPollution, _waterPollution, _timer, _money;
        [SerializeField] private Slider _pollutionSlider, _airPollutionSlider, _waterPollutionSlider;

        private void Awake()
        {
            Drops = _drops;
            Stats.OnAirPollutionLevelChanged += UpdateAirPollution;
            Stats.OnWaterPollutionLevelChanged += UpdateWaterPollution;
            UpdateTime();
            Stats.OnTimerChanged += UpdateTime;
            _money.text = $"${Stats.Money}";
            Stats.OnMoneyChanged += () => { _money.text = $"${Stats.Money}"; };
            Stats.OnNordExploded += () => { StartCoroutine(HideUIFailed()); };
        }

        void UpdateTime()
        {
            int seconds = ((int)Stats.TimerTime % 60);
            int minutes = ((int)Stats.TimerTime / 60);
            _timer.text = $"Time: {minutes:00}:{seconds:00}";
        }
        
        void UpdateAirPollution()
        {
            _pollution.text = $"{(int)Stats.PollutionLevel}%";
            _pollutionSlider.value = Stats.PollutionLevel;
            _airPollution.text = $"{(int)Stats.AirPollutionLevel}%";
            _airPollutionSlider.value = Stats.AirPollutionLevel;
        }

        void UpdateWaterPollution()
        {
            _pollution.text = $"{(int)Stats.PollutionLevel}%";
            _pollutionSlider.value = Stats.PollutionLevel;
            _waterPollution.text = $"{(int)Stats.WaterPollutionLevel}%";
            _waterPollutionSlider.value = Stats.WaterPollutionLevel;
        }

        public IEnumerator HideUIFailed()
        {
            float lerpPosition = 0;
            _uiScaler.transform.localScale = Vector3.one;
            
            while (lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPosition, 0.5f, easingType: Easings.Type.QuadInOut);
                _uiScaler.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            
            gameObject.SetActive(false);
        }
        public IEnumerator HideUISaved()
        {
            float lerpPosition = 0;
            _uiScaler.transform.localScale = Vector3.one;
            
            while (lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPosition, 0.5f, easingType: Easings.Type.QuadInOut);
                _uiScaler.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, t);
                yield return new WaitForEndOfFrame();
            }
            gameObject.SetActive(false);
        }
    }
}