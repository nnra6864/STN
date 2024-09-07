using System.Collections;
using Core;
using NnUtils.Scripts;
using UnityEngine;

namespace Plants
{
    public class PlantNumbers : NnBehaviour
    {
        private bool _isHidden = true;
        private float _lerpPosition;
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

        public void ToggleUI()
        {
            StartNullRoutine(ref _toggleRoutine, _isHidden ? ShowRoutine() : HideRoutine());
            _isHidden = !_isHidden;
        }

        private Coroutine _toggleRoutine;

        private IEnumerator ShowRoutine()
        {
            SoundManager.Instance.PlaySound("Select");
            
            while (_lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref _lerpPosition, _transitionTime, _transitionEasing);
                _rect.anchoredPosition = Vector2.Lerp(_hiddenPosition, _shownPosition, t);
                _toggleButtonArrow.rotation = Quaternion.Lerp(_hiddenRotation, _shownRotation, t);
                yield return null;
            }

            _toggleRoutine = null;
        }

        private IEnumerator HideRoutine()
        {
            SoundManager.Instance.PlaySound("Select");
            
            while (_lerpPosition > 0)
            {
                var t = Misc.ReverseLerpPos(ref _lerpPosition, _transitionTime, _transitionEasing);
                _rect.anchoredPosition = Vector2.Lerp(_hiddenPosition, _shownPosition, t);
                _toggleButtonArrow.rotation = Quaternion.Lerp(_hiddenRotation, _shownRotation, t);
                yield return null;
            }

            _toggleRoutine = null;
        }
    }
}
