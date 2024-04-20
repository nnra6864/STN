using UnityEngine;

namespace Core
{
    public class CameraControls : MonoBehaviour
    {
        Vector3 _previousMousePosition;
        private float _currentZoom = -30;
        [SerializeField] private float _sensitivity, _zoomSensitivity;
        [SerializeField] private float _zoomMin, _zoomMax;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2)) _previousMousePosition = Input.mousePosition;
            Move();
            Zoom();
        }

        void Move()
        {
            Vector3 rotation = new();
            if (Input.GetKey(KeyCode.Mouse2))
            {
                rotation = (_previousMousePosition - Input.mousePosition) * _sensitivity;
                _previousMousePosition = Input.mousePosition;
            }

            transform.position = new();
            transform.Rotate(Vector3.right, rotation.y);
            transform.Rotate(Vector3.up, -rotation.x, Space.World);
            transform.Translate(0, 0, _currentZoom);
        }

        void Zoom()
        {
            if (NnUtils.IsPointerOverUIElement()) return;
            _currentZoom += (Input.GetAxisRaw("Mouse ScrollWheel")) * _zoomSensitivity;
            _currentZoom = _currentZoom < _zoomMin ? _zoomMin : _currentZoom > _zoomMax ? _zoomMax : _currentZoom;
        }
    }
}