using UnityEngine;
using UnityEngine.InputSystem;

namespace TPST
{
    public class InputReader : MonoBehaviour, Controls.IPlayerActions
    {
        private Controls _controls;

        public Vector2 MoveInput { get; private set; }
        public bool IsMoving => MoveInput != Vector2.zero;
        public Vector2 LookInput { get; private set; }

        private void Awake()
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
    }
}
