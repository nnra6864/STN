using System.Collections.Generic;
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
        private readonly List<Resolution> _filteredResolutions = new();

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
            int resolutionIndex = 0;
            List<string> options = new();
            Resolution[] resolutions = Screen.resolutions;

            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                if (resolutions[i].refreshRate == Screen.currentResolution.refreshRate || resolutions[i].refreshRate == Screen.currentResolution.refreshRate - 1)
                {
                    _filteredResolutions.Add(resolutions[i]);
                }
            }

            for (int i = 0; i < _filteredResolutions.Count; i++) 
            {
                string option = _filteredResolutions[i].width + " x " + _filteredResolutions[i].height;
                options.Add(option);

                if (_filteredResolutions[i].width == Screen.currentResolution.width && _filteredResolutions[i].height == Screen.currentResolution.height)
                {
                    resolutionIndex = i;
                }
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
