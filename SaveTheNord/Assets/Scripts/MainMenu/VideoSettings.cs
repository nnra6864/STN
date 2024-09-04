using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace MainMenu
{
    public class VideoSettings : MonoBehaviour
    {
        [SerializeField] private VolumeProfile _volumeProfile;
        [SerializeField] private TMP_Dropdown _resolutionsDropdown, _fullscreenModeDropdown;
        [SerializeField] private Slider _motionBlurStrengthSlider;
        private List<Resolution> _filteredResolutions = new();

        private void OnEnable()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            _resolutionsDropdown.ClearOptions();
            _filteredResolutions.Clear();
            PopulateResolutionsDropdown();
            _fullscreenModeDropdown.SetValueWithoutNotify(
                Screen.fullScreenMode == FullScreenMode.Windowed
                    ? 0 : Screen.fullScreenMode == FullScreenMode.FullScreenWindow
                        ? 1 : 2);
            _motionBlurStrengthSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("MotionBlurStrength", 6));
        }
        
        void PopulateResolutionsDropdown()
        {
            var resolutionIndex = 0;
            List<string> options = new();
            
            //Will leave it for now but causes no res to appear on linux
            //_filteredResolutions = Screen.resolutions
            //    .Where(r => 
            //        Mathf.Approximately((float)r.refreshRateRatio.value, (float)Screen.currentResolution.refreshRateRatio.value) ||
            //        Mathf.Approximately((float)r.refreshRateRatio.value, (float)Screen.currentResolution.refreshRateRatio.value - 1))
            //    .Reverse().ToList();

            _filteredResolutions = Screen.resolutions.Reverse().ToList();
            
            for (int i = 0; i < _filteredResolutions.Count; i++) 
            {
                var option = $"{_filteredResolutions[i].width}x{_filteredResolutions[i].height}";
                options.Add(option);

                if (_filteredResolutions[i].width == Screen.currentResolution.width &&
                    _filteredResolutions[i].height == Screen.currentResolution.height)
                    resolutionIndex = i;
            }
            
            _resolutionsDropdown.AddOptions(options);
            _resolutionsDropdown.SetValueWithoutNotify(resolutionIndex);
            _resolutionsDropdown.RefreshShownValue();
        }
        
        public void SetResolution(int index)
        {
            Resolution resolution = _filteredResolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }

        public void SetFullscreenMode(int index)
        {
            switch (index)
            {
                case 0: Screen.fullScreenMode = FullScreenMode.Windowed;
                    return;
                case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    return;
                case 2: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    return;
            }
        }

        public void SetMotionBlur(float value)
        {
            if (_volumeProfile.TryGet<MotionBlur>(out var mb))
                mb.intensity.value = value / 10f;
            PlayerPrefs.SetInt("MotionBlurStrength", (int)value);
        }
    }
}
