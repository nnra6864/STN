using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel, _exitPanel;
        public static GameObject PauseMenuObject, PausePanel, ExitPanel;

        private void Awake()
        {
            PauseMenuObject = gameObject;
            PausePanel = _pausePanel;
            ExitPanel = _exitPanel;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            SoundManager.Instance.PlaySound("Select");
            SoundManager.Instance.StopSound("GameLoop");
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            SoundManager.Instance.PlaySound("Select");
            SoundManager.Instance.PlaySound("GameLoop");
            Time.timeScale = 1;
        }

        public void Disable()
        {
            Stats.SelectedInfo = null;
        }

        public static void EscPauseMenu()
        {
            if (ExitPanel.activeSelf)
            {
                ExitPanel.SetActive(false);
                PausePanel.SetActive(true);
                return;
            }
            Stats.SelectedInfo = null;
        }
        
        public void ExitToMainMenu()
        {
            SoundManager.Instance.PlaySound("Select");
            SceneManager.LoadScene(0);
        }

        public void ExitToDesktop()
        {
            SoundManager.Instance.PlaySound("Select");
            Application.Quit();
        }

        public void Restart()
        {
            SoundManager.Instance.PlaySound("Select");
            SceneManager.LoadScene(1);
        }
    }
}
