using System.Collections;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu
{
    public class MenuButton : NnBehaviour, IPlanetButton
    {
        private Vector3 _startPos;
        private Vector3 _targetPos;

        private Quaternion _startRot;
        private Quaternion _targetRot;
        
        [SerializeField] private UnityEvent _onClick;
        
        [SerializeField] private float _moveTime = 1;
        [SerializeField] private Easings.Type _moveEasing = Easings.Type.ExpoInOut;
        
        [SerializeField] private Vector3 _moveAmount;
        [SerializeField] private Vector3 _rotationAmount;
        [SerializeField] private Vector3 _scaleAmount;

        private void Awake()
        {
            _startPos = transform.position;
            _targetPos = _startPos + _moveAmount;
            _startRot = transform.rotation;
            _targetRot = Quaternion.Euler(_startRot.eulerAngles + _rotationAmount);
        }

        public void MouseEnter()
        {
            if (_moveAwayRoutine != null) StopCoroutine(_moveAwayRoutine);
            _moveTowardsRoutine = StartCoroutine(MoveTowardsRoutine());
        }

        public void MouseLeave()
        {
            if (_moveTowardsRoutine != null) StopCoroutine(_moveTowardsRoutine);
            _moveAwayRoutine = StartCoroutine(MoveAwayRoutine());
        }

        public void Click() => _onClick?.Invoke();


        private Coroutine _moveTowardsRoutine;
        private IEnumerator MoveTowardsRoutine()
        {
            SoundManager.Instance.PlaySound("PlanetMoveTowards");
            
            var startPos = transform.localPosition;
            var targetPos = _targetPos;
            var startRot = transform.localRotation;
            var targetRot = _targetRot;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, _moveTime, _moveEasing);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
                yield return new WaitForEndOfFrame();
            }
        }
        
        private Coroutine _moveAwayRoutine;
        private IEnumerator MoveAwayRoutine()
        {
            SoundManager.Instance.PlaySound("PlanetMoveAway");

            var startPos = transform.localPosition;
            var targetPos = _startPos;
            var startRot = transform.localRotation;
            var targetRot = _startRot;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, _moveTime, _moveEasing);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
                yield return new WaitForEndOfFrame();
            }
        }

        public void Hide()
        {
            StopRoutine(ref _showRoutine);
            RestartRoutine(ref _hideRoutine, HideRoutine());
        }

        private Coroutine _hideRoutine;
        private IEnumerator HideRoutine()
        {
            SoundManager.Instance.PlaySound("PlanetMoveAway");
            GetComponent<SphereCollider>().enabled = false;
            
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, 0.5f, easingType: Easings.Type.QuadInOut);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            
            _hideRoutine = null;
        }
        
        public void Show()
        {
            StopRoutine(ref _hideRoutine);
            RestartRoutine(ref _showRoutine, ShowRoutine());
        }
        
        private Coroutine _showRoutine;
        private IEnumerator ShowRoutine()
        {
            SoundManager.Instance.PlaySound("PlanetMoveTowards");
            
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, 0.5f, easingType: Easings.Type.QuadInOut);
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return new WaitForEndOfFrame();
            }
            
            GetComponent<SphereCollider>().enabled = true;
            _showRoutine = null;
        }
    }
}
