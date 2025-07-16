using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public event EventHandler OnJumpStarted;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnJumpPerformed;
    public event EventHandler OnFormChanged;
    public event EventHandler OnSprintStarted;
    public event EventHandler OnSprintCanceled;
    public event EventHandler OnInteract;
    public event EventHandler OnAttack;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.started += Jump_started;
        playerInputActions.Player.Jump.canceled += Jump_canceled;
        playerInputActions.Player.Jump.performed += Jump_performed;

        playerInputActions.Player.ChangeForm.performed += ChangeForm_performed;

        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(InputAction.CallbackContext obj)
    {
        OnSprintCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_started(InputAction.CallbackContext obj)
    {
        OnSprintStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ChangeForm_performed(InputAction.CallbackContext obj)
    {
        OnFormChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        OnJumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        OnJumpStarted?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized() 
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
