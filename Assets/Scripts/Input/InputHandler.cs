using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour
{
    private PlayerInput inputActions;

    public static event Action OnJump;
    public static event Action OnCover;
    public static event Action OnPause;

    private void Awake()
    {
        inputActions = new PlayerInput();

        inputActions.Gameplay.Jump.performed += ctx => OnJump?.Invoke();
        inputActions.Gameplay.Cover.performed += ctx => OnCover?.Invoke();
        inputActions.Gameplay.Pause.performed += ctx => OnPause?.Invoke();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }
}