using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu
{
    public class MenuButton : MonoBehaviour, IPlanetButton
    {
        [SerializeField] private float _lerpLength;
        public Vector3 StartingPosition;
        [SerializeField] private Vector3 _moveAmount;
        private Vector3 _targetPosition;
        public Quaternion StartingRotation;
        [SerializeField] private Vector3 _rotationAmount;
        private Quaternion _targetRotation;
        public Vector3 StartingScale;
        [SerializeField] private Vector3 _scaleAmount;
        private Vector3 _targetScale;
        private float _lerpPosition;
        [SerializeField] private UnityEvent _onClick;

        private void Awake()
        {
            _lerpPosition = 0;
            StartingPosition = transform.position;
            _targetPosition = StartingPosition + _moveAmount;
            StartingRotation = transform.rotation;
            _targetRotation = Quaternion.Euler(StartingRotation.eulerAngles + _rotationAmount);
            StartingScale = transform.localScale;
            _targetScale = StartingScale + _scaleAmount;
        }

        public void MouseEnter()
        {
            if (_moveAway != null) StopCoroutine(_moveAway);
            _moveTowards = StartCoroutine(MoveTowards());
        }

        public void MouseLeave()
        {
            if (_moveTowards != null) StopCoroutine(_moveTowards);
            _moveAway = StartCoroutine(MoveAway());
        }

        public void Click()
        {
            _onClick?.Invoke();
        }

        private Coroutine _moveTowards;
        private Coroutine _moveAway;

        private IEnumerator MoveTowards()
        {
            SoundManager.Instance.PlaySound("PlanetMoveTowards");
            while (_lerpPosition < 1)
            {
                _lerpPosition += Time.deltaTime / _lerpLength;
                _lerpPosition = Mathf.Clamp01(_lerpPosition);
                float t = NnUtils.EaseInOutQuad(_lerpPosition);
                transform.position = Vector3.Lerp(StartingPosition, _targetPosition, t);
                transform.rotation = Quaternion.Lerp(StartingRotation, _targetRotation, t);
                yield return new WaitForEndOfFrame();
            }
        }
        
        private IEnumerator MoveAway()
        {
            SoundManager.Instance.PlaySound("PlanetMoveAway");
            while (_lerpPosition > 0)
            {
                _lerpPosition -= Time.deltaTime / _lerpLength;
                _lerpPosition = Mathf.Clamp01(_lerpPosition);
                float t = NnUtils.EaseInOutQuad(_lerpPosition);
                transform.position = Vector3.Lerp(StartingPosition, _targetPosition, t);
                transform.rotation = Quaternion.Lerp(StartingRotation, _targetRotation, t);
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator Hide()
        {
            SoundManager.Instance.PlaySound("PlanetMoveAway");
            GetComponent<SphereCollider>().enabled = false;
            float hideLerpPosition = 0;
            while (hideLerpPosition < 1)
            {
                hideLerpPosition += Time.deltaTime / _lerpLength;
                hideLerpPosition = Mathf.Clamp01(hideLerpPosition);
                float t = NnUtils.EaseInOutQuad(hideLerpPosition);
                transform.localScale = Vector3.Lerp(StartingScale, Vector3.zero, t);
                yield return new WaitForEndOfFrame();
            }
        }
        
        public IEnumerator Show()
        {
            SoundManager.Instance.PlaySound("PlanetMoveTowards");
            float hideLerpPosition = 0;
            while (hideLerpPosition < 1)
            {
                hideLerpPosition += Time.deltaTime / _lerpLength;
                hideLerpPosition = Mathf.Clamp01(hideLerpPosition);
                float t = NnUtils.EaseInOutQuad(hideLerpPosition);
                transform.localScale = Vector3.Lerp(Vector3.zero, StartingScale, t);
                yield return new WaitForEndOfFrame();
            }
            GetComponent<SphereCollider>().enabled = true;
        }
    }
}
