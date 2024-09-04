using System.Collections;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static NnUtils.Scripts.Color;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private VolumeProfile _volumeProfile;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _settings, _guideButton;
        [SerializeField] private GameObject _settingsScaler, _guideScaler;
        [SerializeField] private Material _groundMaterial, _waterMaterial;
        static MenuButton _selectedButton;

        private void Start()
        {
            if (_volumeProfile.TryGet<MotionBlur>(out var mb))
                mb.intensity.value = PlayerPrefs.GetInt("MotionBlurStrength", 6) / 10f;
            _audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
            _audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
            _audioMixer.SetFloat("SfxVolume", PlayerPrefs.GetFloat("SfxVolume"));
            _groundMaterial.color = HexToRgba(PlayerPrefs.GetString("GroundColor", "417B38FF"), new (65, 123, 56, 255));
            _waterMaterial.color = HexToRgba(PlayerPrefs.GetString("WaterColor", "5E81ACFF"), new (94, 129, 172, 255));
            SoundManager.Instance.StopSound("GameLoop");
            SoundManager.Instance.PlaySound("MenuLoop");
            ShowPlanets();
            SoundManager.Instance.StopSound("PlanetMoveTowards");
            SoundManager.Instance.StopSound("Select");
        }

        private void Update()
        {
            Raycast();
            if (Input.GetKeyDown(KeyCode.Mouse0)) 
                if (_selectedButton != null) _selectedButton.Click();
            if (Input.GetKeyUp(KeyCode.Escape) && _settingsScaler.activeSelf)
                CloseSettings();
            else if (Input.GetKeyUp(KeyCode.Escape) && _guideScaler.activeSelf)
                CloseGuide();
        }

        void Raycast()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 200))
            {
                var obj = hit.transform.gameObject.GetComponent<MenuButton>();
                if (obj == _selectedButton) return; 
                if (_selectedButton != null) _selectedButton.MouseLeave();
                if (obj == null) return;
                _selectedButton = obj;
                _selectedButton.MouseEnter();
            }
            else if (_selectedButton != null)
            {
                _selectedButton.MouseLeave();
                _selectedButton = null;
            }
        }
        
        public void Play()
        {
            SoundManager.Instance.StopSound("MenuLoop");
            SoundManager.Instance.PlaySound("Select");
            SceneManager.LoadScene(1);
        }

        public void OpenSettings()
        {
            SoundManager.Instance.PlaySound("Select");
            HidePlanets();
            StartCoroutine(HideUI(_guideButton));
            StartCoroutine(ShowUI(_settingsScaler));
        }

        public void CloseSettings()
        {
            SoundManager.Instance.PlaySound("Select");
            StartCoroutine(HideUI(_settingsScaler));
            StartCoroutine(ShowUI(_guideButton));
            ShowPlanets();
        }
        
        public void OpenGuide()
        {
            SoundManager.Instance.PlaySound("Select");
            HidePlanets();
            StartCoroutine(HideUI(_guideButton));
            StartCoroutine(ShowUI(_guideScaler));
        }

        public void CloseGuide()
        {
            SoundManager.Instance.PlaySound("Select");
            StartCoroutine(HideUI(_guideScaler));
            StartCoroutine(ShowUI(_guideButton));
            ShowPlanets();
        }

        public void Quit()
        {
            SoundManager.Instance.PlaySound("Select");
            Application.Quit();
        }

        void ShowPlanets()
        {
            foreach (Transform planet in transform)
            {
                var button = planet.GetComponent<MenuButton>();
                button.StartCoroutine(button.Show());
            }
        }
        
        void HidePlanets()
        {
            foreach (Transform planet in transform)
            {
                var button = planet.GetComponent<MenuButton>();
                button.StartCoroutine(button.Hide());
            }
        }

        IEnumerator ShowUI(GameObject ui)
        {
            float lerpPosition = 0;
            ui.SetActive(true);
            ui.transform.localScale = Vector3.zero;
            
            while (lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPosition, 0.5f, easingType: Easings.Types.QuadInOut);
                ui.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
        }
        
        IEnumerator HideUI(GameObject ui)
        {
            float lerpPosition = 0;
            ui.transform.localScale = Vector3.one;
            
            while (lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPosition, 0.5f, easingType: Easings.Types.QuadInOut);
                ui.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            ui.SetActive(false);
        }
    }
}
