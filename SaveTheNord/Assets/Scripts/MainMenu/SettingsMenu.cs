using UnityEngine;

namespace MainMenu
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _videoSettings;
        [SerializeField] private GameObject _audioSettings;
        [SerializeField] private GameObject _gameplaySettings;
        public void OpenVideoSettings()
        {
            _audioSettings.SetActive(false);
            _gameplaySettings.SetActive(false);
            _videoSettings.SetActive(true);
        }
        
        public void OpenAudioSettings()
        {
            _videoSettings.SetActive(false);
            _gameplaySettings.SetActive(false);
            _audioSettings.SetActive(true);
        }
        
        public void OpenGameplaySettings()
        {
            _videoSettings.SetActive(false);
            _audioSettings.SetActive(false);
            _gameplaySettings.SetActive(true);
        }
    }
}
