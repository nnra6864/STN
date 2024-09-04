using System.Collections;
using Core;
using UnityEngine;

namespace Plants
{
    public class PlantNumbers : MonoBehaviour
    {
        [SerializeField] private bool _isHidden;
        [SerializeField] private RectTransform _toggleButtonArrow;
        private float _lerpPosition;
        private Vector2 _hiddenPosition, _shownPosition;
        private Quaternion _hiddenRotation, _shownRotation;
        private RectTransform _rect;
        [SerializeField] private GameObject _prefab, _content;
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
            if (_toggleCoroutine != null) StopCoroutine(_toggleCoroutine);
            _toggleCoroutine = StartCoroutine(_isHidden ? Show() : Hide());
            _isHidden = !_isHidden;
        }

        private Coroutine _toggleCoroutine;
        IEnumerator Show()
        {
            SoundManager.Instance.PlaySound("Select");
            while (_lerpPosition < 1)
            {
                _lerpPosition += Time.deltaTime / 0.25f;
                _lerpPosition = Mathf.Clamp01(_lerpPosition);
                var t = NnUtils.EaseInOut(_lerpPosition);
                _rect.anchoredPosition = Vector2.Lerp(_hiddenPosition, _shownPosition, t);
                _toggleButtonArrow.rotation = Quaternion.Lerp(_hiddenRotation, _shownRotation, t);
                yield return null;
            }
        }

        IEnumerator Hide()
        {
            SoundManager.Instance.PlaySound("Select");
            while (_lerpPosition > 0)
            {
                _lerpPosition -= Time.deltaTime / 0.25f;
                _lerpPosition = Mathf.Clamp01(_lerpPosition);
                var t = NnUtils.EaseInOut(_lerpPosition);
                _rect.anchoredPosition = Vector2.Lerp(_hiddenPosition, _shownPosition, t);
                _toggleButtonArrow.rotation = Quaternion.Lerp(_hiddenRotation, _shownRotation, t);
                yield return null;
            }
        }
    }
}
