using UnityEngine;
using UnityEngine.InputSystem;

namespace BlindfoldProtocol.Runtime
{
    public sealed class GameCameraController : MonoBehaviour
    {
        private float _panSpeed = 20f;
        private float _zoomStep = 1.2f;
        private float _minZoom = 3f;
        private float _maxZoom = 30f;
        private Camera _targetCamera;

        public void Configure(float panSpeed, float zoomStep, float minZoom, float maxZoom)
        {
            _panSpeed = panSpeed;
            _zoomStep = zoomStep;
            _minZoom = minZoom;
            _maxZoom = maxZoom;

            if (_targetCamera == null)
            {
                _targetCamera = GetComponent<Camera>();
            }
        }

        private void Awake()
        {
            _targetCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (_targetCamera == null)
            {
                return;
            }

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            var horizontal = 0f;
            var vertical = 0f;

            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    horizontal -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    horizontal += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    vertical -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    vertical += 1f;
                }
            }

            var move = new Vector2(horizontal, vertical);
            if (move.sqrMagnitude > 1f)
            {
                move.Normalize();
            }

            var panStep = _panSpeed * Time.unscaledDeltaTime;
            var position = transform.position;
            position.x += move.x * panStep;
            position.z += move.y * panStep;
            transform.position = position;

            if (mouse == null)
            {
                return;
            }

            var scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) <= Mathf.Epsilon)
            {
                return;
            }

            _targetCamera.orthographicSize = Mathf.Clamp(
                _targetCamera.orthographicSize - (scroll * _zoomStep * 0.01f),
                _minZoom,
                _maxZoom);
        }
    }
}
