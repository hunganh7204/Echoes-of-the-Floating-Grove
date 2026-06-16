using System;
using UnityEngine;
using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerControls playerControls;

    public float HorizontalMovement => DialogueManager.IsChatting ? 0f : playerControls.Player.Move.ReadValue<float>();

    public bool JumpPressed => !DialogueManager.IsChatting && playerControls.Player.Jump.WasPressedThisFrame();
    public bool InteractPressed => !DialogueManager.IsChatting && playerControls.Player.Interact.WasPressedThisFrame();
    public bool AttackPressed => !DialogueManager.IsChatting && playerControls.Player.Attack.WasPressedThisFrame();
    public bool GravityPressed => !DialogueManager.IsChatting && playerControls.Player.Gravity.WasPressedThisFrame();
    public bool RecordEchoPressed => !DialogueManager.IsChatting && playerControls.Player.RecordEcho.WasPressedThisFrame();
    public bool ReplayEchoPressed => !DialogueManager.IsChatting && playerControls.Player.ReplayEcho.WasPressedThisFrame();
    public bool CancelEchoPressed => !DialogueManager.IsChatting && playerControls.Player.CancelEcho.WasPressedThisFrame();
    public bool SprintHeld => !DialogueManager.IsChatting && playerControls.Player.Sprint.IsPressed();

    public bool PausePressed => playerControls.Player.Pause.WasPressedThisFrame();

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
            LoadBindings();
        }
        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

    // ==========================================
    // 1. HÀM TRẢ VỀ TÊN PHÍM HIỂN THỊ CHO UI
    // ==========================================
    public string GetBindingName(string actionName)
    {
        InputAction action = playerControls.asset.FindAction(actionName);
        if (action != null)
        {
            // Lấy tên hiển thị của phím ở vị trí index 0
            return action.GetBindingDisplayString(0);
        }
        return "Trống";
    }

    // ==========================================
    // 2. HÀM KIỂM TRA TRÙNG PHÍM
    // ==========================================
    private InputAction GetConflictingAction(InputAction currentAction, string newBindingPath)
    {
        foreach (var action in playerControls.asset)
        {
            if (action == currentAction) continue;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].effectivePath == newBindingPath)
                {
                    return action; // Trả về chính xác hành động đang chiếm phím này
                }
            }
        }
        return null;
    }

    // ==========================================
    // 3. CẬP NHẬT LẠI HÀM ĐỔI PHÍM (REBIND)
    // ==========================================
    public void RebindAction(string actionName, Action onRebindComplete)
    {
        InputAction actionToRebind = playerControls.asset.FindAction(actionName);
        if (actionToRebind == null) return;

        actionToRebind.Disable();

        // Lưu lại phím cũ để hoán đổi
        string oldBindingPath = actionToRebind.bindings[0].effectivePath;

        var rebindOperation = actionToRebind.PerformInteractiveRebinding(0)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                string newBindingPath = actionToRebind.bindings[0].effectivePath;

                // KIỂM TRA VÀ HOÁN ĐỔI NẾU TRÙNG LẶP
                InputAction conflictingAction = GetConflictingAction(actionToRebind, newBindingPath);
                if (conflictingAction != null)
                {
                    Debug.Log($"Hệ thống: Trùng phím! Đang hoán đổi phím sang {conflictingAction.name}");
                    // Gán phím cũ của hành động hiện tại cho hành động bị trùng
                    conflictingAction.ApplyBindingOverride(0, oldBindingPath);
                }

                operation.Dispose();
                actionToRebind.Enable();

                // Chỉ báo là đã xong, để UI tự quét lại toàn bộ
                onRebindComplete?.Invoke();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                actionToRebind.Enable();
                onRebindComplete?.Invoke();
            })
            .Start();
    }

    public void SaveBindingsToData()
    {
        string overrides = playerControls.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("InputBindings", overrides);
        PlayerPrefs.Save();

        Debug.Log("Hệ thống: Đã lưu cấu hình phím bấm!");
    }

    public void LoadBindings()
    {
        if (PlayerPrefs.HasKey("InputBindings"))
        {
            string overrides = PlayerPrefs.GetString("InputBindings");
            playerControls.asset.LoadBindingOverridesFromJson(overrides);
        }
    }
}
