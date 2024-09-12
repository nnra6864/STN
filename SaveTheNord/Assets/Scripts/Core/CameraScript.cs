using System.Collections;
using NnUtils.Scripts;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Should be attached to the camera origin that's a parent of the actual camera.
    /// <br/>
    /// Origin object should be placed at the center of the current planet.
    /// </summary>
    public class CameraScript : NnBehaviour
    {
        //Current rotation has to be stored and can't be taken from the origin rot
        //because quaternions are between -180 and 180
        private Vector2 _currentRot;
        private Vector2 _totalRot;
        
        //Needed for the UI checks
        private bool _isRotating;
        
        private float _zoom;
        
        [Header("Components")]
        [SerializeField] private Transform _origin;
        [SerializeField] private Transform _camera;
        
        [Header("Values")]
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _zoomSensitivity = 30;
        [SerializeField] private Vector2 _zoomMinMax = new(-50, -15);

        [Header("Tweening")]
        [SerializeField] private float _rotateTime = 1;
        [SerializeField] private Easings.Type _rotateEasing = Easings.Type.ExpoOut;
        [SerializeField] private float _zoomTime = 1;
        [SerializeField] private Easings.Type _zoomEasing = Easings.Type.ExpoOut;

        private void Reset()
        {
            _origin = transform;
            _camera = transform.GetChild(0);
        }

        private void Start()
        {
            //Centering the zoom
            _zoom = Mathf.LerpUnclamped(_zoomMinMax.x, _zoomMinMax.y, 0.5f);
            
            //Calling RotateRoutine and ZoomRoutine to align current values with actual values
            RestartRoutine(ref _rotateRoutine, RotateRoutine());
            RestartRoutine(ref _zoomRoutine, ZoomRoutine());
        }
        
        private void Update()
        {
            Rotate();
            Zoom();
        }

        private void Rotate()
        {
            if (!Misc.IsPointerOverUI && Input.GetKeyDown(KeyCode.Mouse2)) _isRotating = true;
            if (Input.GetKeyUp(KeyCode.Mouse2)) _isRotating = false;
            if (!_isRotating) return;
            
            //Get mouse delta and return if 0
            Vector2 positionDelta = new(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            if (positionDelta == Vector2.zero) return;
            
            //Add to the _totalRot and restart the RotateRoutine()
            _totalRot += positionDelta * _sensitivity;
            RestartRoutine(ref _rotateRoutine, RotateRoutine());
        }

        private Coroutine _rotateRoutine;
        private IEnumerator RotateRoutine()
        {
            var startRot = _currentRot;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, _rotateTime, _rotateEasing);
                _currentRot = Vector2.LerpUnclamped(startRot, _totalRot, t);
                _origin.localRotation = Quaternion.Euler(_currentRot);
                yield return null;
            }

            _rotateRoutine = null;
        }

        private void Zoom()
        {
            //Get mouse scroll delta and return if 0
            var scrollDelta = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scrollDelta == 0) return;
            
            //Add to _zoom, clamp to _zoomMinMax and restart the ZoomRoutine()
            _zoom += scrollDelta * _zoomSensitivity;
            _zoom = Mathf.Clamp(_zoom, _zoomMinMax.x, _zoomMinMax.y);
            RestartRoutine(ref _zoomRoutine, ZoomRoutine());
        }

        private Coroutine _zoomRoutine;
        private IEnumerator ZoomRoutine()
        {
            var startZoom = _camera.localPosition.z;
            
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var camPos = _camera.localPosition;
                var t = Misc.Tween(ref lerpPos, _zoomTime, _zoomEasing);
                camPos.z = Mathf.LerpUnclamped(startZoom, _zoom, t);
                _camera.localPosition = camPos;
                yield return null;
            }

            _zoomRoutine = null;
        }
    }
}
