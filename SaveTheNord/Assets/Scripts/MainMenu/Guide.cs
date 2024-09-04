using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class Guide : MonoBehaviour
    {
        [SerializeField] private Image _aboutImage, _institutionsImage, _controlsImage, _creditsImage;
        [SerializeField] private GameObject _about, _institutions, _controls, _credits;
        [SerializeField] private Toggle _sandboxMode;

        private void Awake()
        {
            _sandboxMode.SetIsOnWithoutNotify(PlayerPrefs.GetInt("IsSandbox", 0) == 1);
        }

        public void EnableAbout()
        {
            _about.SetActive(true);
            _institutions.SetActive(false);
            _controls.SetActive(false);
            _credits.SetActive(false);
            _aboutImage.color = new Color32(0, 0, 0, 150);
            _institutionsImage.color = new Color32(0, 0, 0, 0);
            _controlsImage.color = new Color32(0, 0, 0, 0);
            _creditsImage.color = new Color32(0, 0, 0, 0);
        }
        public void EnableInstitutions()
        {
            _about.SetActive(false);
            _institutions.SetActive(true);
            _controls.SetActive(false);
            _credits.SetActive(false);
            _aboutImage.color = new Color32(0, 0, 0, 0);
            _institutionsImage.color = new Color32(0, 0, 0, 150);
            _controlsImage.color = new Color32(0, 0, 0, 0);
            _creditsImage.color = new Color32(0, 0, 0, 0);
        }
        public void EnableControls()
        {
            _about.SetActive(false);
            _institutions.SetActive(false);
            _controls.SetActive(true);
            _credits.SetActive(false);
            _aboutImage.color = new Color32(0, 0, 0, 0);
            _institutionsImage.color = new Color32(0, 0, 0, 0);
            _controlsImage.color = new Color32(0, 0, 0, 150);
            _creditsImage.color = new Color32(0, 0, 0, 0);
        }
        public void EnableCredits()
        {
            _about.SetActive(false);
            _institutions.SetActive(false);
            _controls.SetActive(false);
            _credits.SetActive(true);
            _aboutImage.color = new Color32(0, 0, 0, 0);
            _institutionsImage.color = new Color32(0, 0, 0, 0);
            _controlsImage.color = new Color32(0, 0, 0, 0);
            _creditsImage.color = new Color32(0, 0, 0, 150);
        }
        
        public void ToggleIsSandbox(bool input)
        {
            SoundManager.Instance.PlaySound("Select");
            PlayerPrefs.SetInt("IsSandbox", input ? 1 : 0);
        }
    }
}
