using UnityEngine;
using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerControls playerControls;

        public float HorizontalMovement => playerControls.Player.Move.ReadValue<float>();

        public bool JumpPressed => playerControls.Player.Jump.WasPressedThisFrame();
        public bool InteractPressed => playerControls.Player.Interact.WasPressedThisFrame();
        public bool AttackPressed => playerControls.Player.Attack.WasPressedThisFrame();
        public bool GravityPressed => playerControls.Player.Gravity.WasPressedThisFrame();
        public bool RecordEchoPressed => playerControls.Player.RecordEcho.WasPressedThisFrame();
        public bool ReplayEchoPressed => playerControls.Player.ReplayEcho.WasPressedThisFrame();
        public bool CancelEchoPressed => playerControls.Player.CancelEcho.WasPressedThisFrame();
        public bool SprintHeld => playerControls.Player.Sprint.IsPressed();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerControls = new PlayerControls();
        }
        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
