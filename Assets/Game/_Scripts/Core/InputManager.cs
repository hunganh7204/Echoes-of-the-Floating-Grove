using System;
using UnityEngine;
using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerControls playerControls;

    public float HorizontalMovement => (DialogueManager.IsChatting || Time.timeScale == 0f) ? 0f : playerControls.Player.Move.ReadValue<float>();

    public bool JumpPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.Jump.WasPressedThisFrame();

    public bool InteractPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.Interact.WasPressedThisFrame();

    public bool AttackPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.Attack.WasPressedThisFrame();
    public bool GravityPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.Gravity.WasPressedThisFrame();
    public bool RecordEchoPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.RecordEcho.WasPressedThisFrame();
    public bool ReplayEchoPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.ReplayEcho.WasPressedThisFrame();
    public bool CancelEchoPressed => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.CancelEcho.WasPressedThisFrame();
    public bool SprintHeld => !DialogueManager.IsChatting && Time.timeScale > 0f && playerControls.Player.Sprint.IsPressed();
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

    public string GetBindingName(string actionName, int bindingIndex = 0)
    {
        InputAction action = playerControls.asset.FindAction(actionName);
        if (action != null)
        {
            return action.GetBindingDisplayString(bindingIndex);
        }
        return "Trống";
    }

    private InputAction GetConflictingAction(InputAction currentAction, string newBindingPath)
    {
        foreach (var action in playerControls.asset)
        {
            if (action == currentAction) continue;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].effectivePath == newBindingPath)
                {
                    return action; 
                }
            }
        }
        return null;
    }

    public void RebindAction(string actionName, Action onRebindComplete, int bindingIndex = 0)
    {
        InputAction actionToRebind = playerControls.asset.FindAction(actionName);
        if (actionToRebind == null) return;

        string oldBindingPath = actionToRebind.bindings[bindingIndex].effectivePath;

        actionToRebind.Disable();

        var operation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                string newBindingPath = actionToRebind.bindings[bindingIndex].effectivePath;

                InputAction conflictingAction = GetConflictingAction(actionToRebind, newBindingPath);
                if (conflictingAction != null)
                {
                    int conflictIndex = -1;
                    for (int i = 0; i < conflictingAction.bindings.Count; i++)
                    {
                        if (conflictingAction.bindings[i].effectivePath == newBindingPath)
                        {
                            conflictIndex = i;
                            break;
                        }
                    }

                    if (conflictIndex != -1)
                    {
                        conflictingAction.ApplyBindingOverride(conflictIndex, oldBindingPath);
                    }
                }

                op.Dispose();
                actionToRebind.Enable();

                SaveBindingsToData();
                onRebindComplete?.Invoke();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                actionToRebind.Enable();
                onRebindComplete?.Invoke();
            })
            .Start();
    }

    public void ResetBindingsToDefault()
    {
        playerControls.asset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("InputBindings");
        PlayerPrefs.Save();
    }

    public void SaveBindingsToData()
    {
        string overrides = playerControls.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("InputBindings", overrides);
        PlayerPrefs.Save();

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
