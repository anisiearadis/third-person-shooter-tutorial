using UnityEngine;

namespace TPST
{
    public enum MovementStateType
    {
        TargetingMovement,
        FreeLookMovement,
    }

    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float moveSpeed;

        [Header("Player Rotation")]
        [SerializeField] private MovementStateType movementStateType = MovementStateType.TargetingMovement;
        [SerializeField] private float playerRotationSmoothTime = 0.05f;

        [Header("Camera Rotation")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float minClamp = -30f;
        [SerializeField] private float maxClamp = 70f;
        [SerializeField] private float cameraTargetRotationSmoothTime = 0.05f;

        private Transform _cameraTransform;
        private CharacterController _controller;
        private InputReader _input;

        private float _currentPlayerRotationVelocity;

        private float _currentSpeed;
        private float _speedVelocity;

        private Vector2 _currentCameraTargetRotation;
        private Vector3 _cameraTargetRotation;
        private Vector3 _currentCameraTargetRotationVelocity;

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputReader>();
        }

        private void Update()
        {
            if (movementStateType == MovementStateType.TargetingMovement)
            {
                TargetingMovementUpdate();
            }

            if (movementStateType == MovementStateType.FreeLookMovement)
            {
                FreeLookMovementUpdate();
            }
        }

        private void LateUpdate()
        {
            CameraRotationUpdate();
        }

        private void TargetingMovementUpdate()
        {
            if (!_input.IsMoving) return;

            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(_cameraTransform.forward);

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetRotation.eulerAngles.y,
                ref _currentPlayerRotationVelocity,
                playerRotationSmoothTime
            );

            // Movement
            Vector3 movement = _cameraTransform.right * _input.MoveInput.x + _cameraTransform.forward * _input.MoveInput.y;
            movement.y = 0;

            _controller.Move(movement * (moveSpeed * Time.deltaTime));
        }

        private void FreeLookMovementUpdate()
        {
            if (!_input.IsMoving) return;

            // Rotation
            Quaternion targetRotation = Quaternion.Euler(
                0f,
                Mathf.Atan2(_input.MoveInput.x, _input.MoveInput.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y,
                0f
            );

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetRotation.eulerAngles.y,
                ref _currentPlayerRotationVelocity,
                playerRotationSmoothTime
            );

            // Movement
            float targetSpeed = _input.MoveInput.magnitude * moveSpeed;
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.1f);

            _controller.Move(transform.forward * (_currentSpeed * Time.deltaTime));
        }

        private void CameraRotationUpdate()
        {
            _currentCameraTargetRotation.y += _input.LookInput.x * sensitivity;
            _currentCameraTargetRotation.x -= _input.LookInput.y * sensitivity;
            _currentCameraTargetRotation.x = Mathf.Clamp(_currentCameraTargetRotation.x, minClamp, maxClamp);

            _cameraTargetRotation = Vector3.SmoothDamp(
                _cameraTargetRotation,
                _currentCameraTargetRotation,
                ref _currentCameraTargetRotationVelocity,
                cameraTargetRotationSmoothTime
            );

            cameraTarget.transform.eulerAngles = _cameraTargetRotation;
        }
    }
}