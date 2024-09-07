using System;
using System.Collections;
using Core;
using NnUtils.Scripts;
using UnityEngine;

namespace Plants
{
    public class PlantNumbers : NnBehaviour
    {
        private bool _isHidden = true;
        private float _lerpPos;
        private Vector2 _hiddenPosition, _shownPosition;
        private Quaternion _hiddenRotation, _shownRotation;
        private RectTransform _rect;
        
        [SerializeField] private RectTransform _toggleButtonArrow;
        [SerializeField] private GameObject _prefab, _content;
        [SerializeField] private float _transitionTime = 0.75f;
        [SerializeField] private Easings.Type _transitionEasing = Easings.Type.ExpoOut;
        
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _hiddenPosition = _isHidden
                ? _rect.anchoredPosition
                : _rect.anchoredPosition - new Vector2(_rect.sizeDelta.x / 2, 0);
            _shownPosition = _hiddenPosition + new Vector2(_rect.sizeDelta.x, 0);
            _hiddenRotation = Quaternion.Euler(new(0, 0, 90));
            _shownRotation = Quaternion.Euler(new(0, 0, -90));
            foreach (var pn in Stats.PlantNumbers)
            {
                var obj = Instantiate(_prefab, _content.transform);
                obj.GetComponent<PlantNumber>().UpdateInfo(pn.Key);
            }
        }

        public void ToggleUI() => StartNullRoutine(ref _toggleRoutine, ToggleRoutine());
        private Coroutine _toggleRoutine;
        private IEnumerator ToggleRoutine()
        {
            _isHidden = !_isHidden;
            SoundManager.Instance.PlaySound("Select");

            Func<float> lerp = _isHidden 
                ? () => Misc.ReverseLerpPos(ref _lerpPos, _transitionTime, _transitionEasing)
                : () => Misc.UpdateLerpPos(ref _lerpPos, _transitionTime, _transitionEasing); 
            
            while (_isHidden ? _lerpPos > 0 : _lerpPos < 1)
            {
                var t = lerp();
                _rect.anchoredPosition = Vector2.Lerp(_hiddenPosition, _shownPosition, t);
                _toggleButtonArrow.rotation = Quaternion.Lerp(_hiddenRotation, _shownRotation, t);
                yield return null;
            }
            
            _toggleRoutine = null;
        }
    }
}
