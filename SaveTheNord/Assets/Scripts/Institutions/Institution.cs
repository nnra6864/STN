using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Institutions
{
    public class Institution : MonoBehaviour, IInteract
    {
        [System.Serializable]
        public struct CraftRecipe
        {
            public List<InventoryItem> Ingredients;
            public InventoryItem Product;
            public float Price;
            public float AirContamination;
            public float WaterContamination;
        }
        
        [SerializeField] private GameObject _ui;
        [SerializeField] private GameObject _infoUI;
        [SerializeField] private string _selectSoundName;
        
        public void MouseEnter()
        {
            
        }

        public void MouseLeave()
        {
            
        }

        public void Click()
        {
            ToggleUI();
        }

        public void RightClick()
        {
            ToggleInfo();
        }

        public void ToggleUI()
        {
            _toggleUIRoutine ??= StartCoroutine(ToggleUi());
        }
        
        public void ToggleInfo()
        {
            _toggleInfoUIRoutine ??= StartCoroutine(ToggleInfoUi());
        }

        private Coroutine _toggleUIRoutine;
        private IEnumerator ToggleUi()
        {
            SoundManager.Instance.PlaySound(_selectSoundName);
            if (_ui.activeSelf) Stats.SelectedInfo = null;
            else
            {
                _ui.SetActive(true);
                Stats.SelectedInfo = _ui;
            }
            yield return new WaitForSeconds(0.5f);
            _toggleUIRoutine = null;
        }
        
        private Coroutine _toggleInfoUIRoutine;
        private IEnumerator ToggleInfoUi()
        {
            SoundManager.Instance.PlaySound(_selectSoundName);
            if (_infoUI.activeSelf) Stats.SelectedInfo = null;
            else
            {
                _infoUI.SetActive(true);
                Stats.SelectedInfo = _infoUI;
            }
            yield return new WaitForSeconds(0.5f);
            _toggleInfoUIRoutine = null;
        }
    }
}