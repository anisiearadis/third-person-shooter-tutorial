using UnityEngine;

namespace TPST
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float moveSpeed;

        [Header("Player Rotation")]
        [SerializeField] private float playerRotationSmoothTime = 0.12f;

        [Header("Camera Rotation")]
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float minClamp = -30f;
        [SerializeField] private float maxClamp = 70f;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraRotationSmoothTime = 0.12f;
        [SerializeField] private float cameraDistance = 5f;

        private Transform _cameraTransform;
        private CharacterController _controller;
        private InputReader _input;

        private float _currentPlayerRotationVelocity;

        private float _currentSpeed;
        private float _speedVelocity;

        private Vector2 _currentRotation;
        private Vector3 _targetRotation;
        private Vector3 _currentCameraRotationVelocity;

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputReader>();
        }

        private void Update()
        {
            PlayerRotationUpdate();
            PlayerMovementUpdate();
        }

        private void LateUpdate()
        {
            CameraRotationUpdate();
        }

        private void PlayerRotationUpdate()
        {
            if (!_input.IsMoving) return;

            float rotation = Mathf.Atan2(
                _input.MoveInput.x,
                _input.MoveInput.y
            ) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                rotation,
                ref _currentPlayerRotationVelocity,
                playerRotationSmoothTime
            );
        }

        private void PlayerMovementUpdate()
        {
            float targetSpeed = _input.MoveInput.magnitude * moveSpeed;
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.1f);
            
            _controller.Move(transform.forward * (_currentSpeed * Time.deltaTime));
        }

        private void CameraRotationUpdate()
        {
            _currentRotation.y += _input.LookInput.x * sensitivity;
            _currentRotation.x -= _input.LookInput.y * sensitivity;

            _currentRotation.x = Mathf.Clamp(_currentRotation.x, minClamp, maxClamp);

            _targetRotation = Vector3.SmoothDamp(
                _targetRotation,
                _currentRotation,
                ref _currentCameraRotationVelocity,
                cameraRotationSmoothTime
            );
            _cameraTransform.transform.eulerAngles = _targetRotation;

            _cameraTransform.position = cameraTarget.transform.position - _cameraTransform.forward * cameraDistance;
        }
    }
}