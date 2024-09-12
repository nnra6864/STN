using System;
using System.Text.RegularExpressions;
using Core;
using TMPro;
using UnityEngine;

namespace Plants
{
    public class PlantNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name, _amount;
        private PlantNumberItem _plantNumberStruct;
        
        public void UpdateInfo(Plant.PlantNames plantName)
        {
            _name.text = Regex.Replace(Enum.GetName(typeof(Plant.PlantNames), plantName)!, "([a-z])([A-Z])", "$1 $2");
            _plantNumberStruct = Stats.PlantNumbers[plantName];
            _amount.text = $"{_plantNumberStruct.Amount}/{_plantNumberStruct.Target}";
            _plantNumberStruct.OnAmountChanged += () =>
            {
                _amount.text = $"{_plantNumberStruct.Amount}/{_plantNumberStruct.Target}";
            };
        }
    }
}
