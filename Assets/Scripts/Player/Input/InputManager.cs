using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputAction playerInputActions;

    public event Action OnJump; 
    private void Awake()
    {
        playerInputActions = new PlayerInputAction();
    }
    private void OnEnable()
    {
        playerInputActions.Controls.Enable();
    }
    private void OnDisable()
    {
        playerInputActions.Controls.Disable();
    }

    public Vector2 GetNormalizedInputDirection()
    {
        Vector2 inputDirection = playerInputActions.Controls.Move.ReadValue<Vector2>();
        inputDirection.Normalize();
        return inputDirection;           
    }

    public Vector2 GetMouseDelta()
    {
        return playerInputActions.Controls.MousePosition.ReadValue<Vector2>();
    }

    public bool PlayerJumped()
    {
        OnJump?.Invoke();
        return playerInputActions.Controls.Jump.triggered;
    }


}
