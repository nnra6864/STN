using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NnUtils.Scripts.Color;

namespace MainMenu
{
    public class GameplaySettings : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _transitionTime, _hoverColor, _selectColor, _groundColor, _waterColor;
        [SerializeField] private Toggle _showPlanets, _showFog;
        [SerializeField] private Material _groundTilesMaterial, _waterTilesMaterial;

        private void OnEnable()
        {
            _transitionTime.SetTextWithoutNotify($"{PlayerPrefs.GetFloat("TileTransitionTime", 0.25f)}");
            _hoverColor.SetTextWithoutNotify(PlayerPrefs.GetString("TileHoverHex", "EBCB8BFF"));
            _selectColor.SetTextWithoutNotify(PlayerPrefs.GetString("TileSelectHex", "88C0D0FF"));
            _groundColor.SetTextWithoutNotify(PlayerPrefs.GetString("GroundColor", "417B38FF"));
            _waterColor.SetTextWithoutNotify(PlayerPrefs.GetString("WaterColor", "5E81ACFF"));
            _showPlanets.SetIsOnWithoutNotify(PlayerPrefs.GetInt("ShowPlanets", 1) == 1);
            _showFog.SetIsOnWithoutNotify(PlayerPrefs.GetInt("ShowPollutionFog", 1) == 1);
        }

        public void ChangeTransitionTime(string input)
        {
            PlayerPrefs.SetFloat("TileTransitionTime", float.Parse(input));
            _transitionTime.SetTextWithoutNotify($"{PlayerPrefs.GetFloat("TileTransitionTime", 0.25f)}");
        }

        public void ChangeHoverColor(string input)
        {
            var color = ColorUtility.ToHtmlStringRGBA(HexToRgba(input, new (235, 203, 139, 255)));
            PlayerPrefs.SetString("TileHoverHex", color);
            _hoverColor.SetTextWithoutNotify(color);
        }
        
        public void ChangeSelectColor(string input)
        {
            var color = ColorUtility.ToHtmlStringRGBA(HexToRgba(input, new (136, 192, 208, 255)));
            PlayerPrefs.SetString("TileSelectHex", color);
            _selectColor.SetTextWithoutNotify(color);
        }

        public void ToggleShowPlanets(bool input)
        {
            PlayerPrefs.SetInt("ShowPlanets", input ? 1 : 0);
        }
        
        public void ChangeGroundColor(string input)
        {
            var color = ColorUtility.ToHtmlStringRGBA(HexToRgba(input, new (65, 123, 56, 255)));
            _groundTilesMaterial.color = HexToRgba(color, new (65, 123, 56, 255));
            PlayerPrefs.SetString("GroundColor", color);
            _groundColor.SetTextWithoutNotify(color);
        }
        
        public void ChangeWaterColor(string input)
        {
            var color = ColorUtility.ToHtmlStringRGBA(HexToRgba(input, new (94, 129, 172, 255)));
            _waterTilesMaterial.color = HexToRgba(color, new (94, 129, 172, 255));
            PlayerPrefs.SetString("WaterColor", color);
            _waterColor.SetTextWithoutNotify(color);
        }
        
        public void ToggleShowPollutionFog(bool input)
        {
            PlayerPrefs.SetInt("ShowPollutionFog", input ? 1 : 0);
        }
    }
}
