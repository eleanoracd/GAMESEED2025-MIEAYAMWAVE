using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public static InputHandler Instance;

    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool PausePressed { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Jump.performed += OnJump;
        inputActions.Gameplay.Jump.canceled += OnJumpCancel;
        inputActions.Gameplay.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Jump.performed -= OnJump;
        inputActions.Gameplay.Jump.canceled -= OnJumpCancel;
        inputActions.Gameplay.Jump.performed -= OnPause;
        inputActions.Gameplay.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = true;
        JumpHeld = true;
    }

    private void OnJumpCancel(InputAction.CallbackContext context) => JumpHeld = false;
    private void OnPause(InputAction.CallbackContext context) => PausePressed = true;

    public void ResetFlags()
    {
        JumpPressed = false;
        PausePressed = false;
    }
}
