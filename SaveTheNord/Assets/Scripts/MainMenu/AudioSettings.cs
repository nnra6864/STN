using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MainMenu
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _master, _music, _sfx;
        private void OnEnable()
        {
            _audioMixer.GetFloat("MasterVolume", out var master);
            _master.SetValueWithoutNotify(master);
            _audioMixer.GetFloat("MusicVolume", out var music);
            _music.SetValueWithoutNotify(music);
            _audioMixer.GetFloat("SfxVolume", out var sfx);
            _sfx.SetValueWithoutNotify(sfx);

        }

        public void ChangeMasterVolume(float value)
        {
            PlayerPrefs.SetFloat("MasterVolume", value);
            _audioMixer.SetFloat("MasterVolume", value);
        }
        
        public void ChangeMusicVolume(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);    
            _audioMixer.SetFloat("MusicVolume", value);    
        }
        
        public void ChangeSfxVolume(float value)
        {
            PlayerPrefs.SetFloat("SfxVolume", value);
            _audioMixer.SetFloat("SfxVolume", value);
        }
    }
}
