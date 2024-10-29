using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : NetworkBehaviour
{
    PlayerInputAction playerInputActions;

    public event Action OnJump,OnRun,OnStopRun, OnShoot; 
    private void Awake()
    {
        playerInputActions = new PlayerInputAction();
    }
    private void OnEnable()
    {
        playerInputActions.Controls.Enable();
        playerInputActions.Controls.Jump.performed += PlayerJumped;
        playerInputActions.Controls.Run.performed += PlayerRun;
        playerInputActions.Controls.Run.canceled += PlayerStopRun;
        playerInputActions.Controls.Shoot.performed += PlayerShoot;
    }
    private void OnDisable()
    {
        playerInputActions.Controls.Disable();
        playerInputActions.Controls.Jump.performed -= PlayerJumped;
        playerInputActions.Controls.Run.performed -= PlayerRun;
        playerInputActions.Controls.Run.canceled -= PlayerStopRun;
        playerInputActions.Controls.Shoot.performed -= PlayerShoot;
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

    private void PlayerJumped(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }
    private void PlayerRun(InputAction.CallbackContext context)
    {
        OnRun?.Invoke();
    }
    private void PlayerStopRun(InputAction.CallbackContext context)
    {
        OnStopRun?.Invoke();
    }
    private void PlayerShoot(InputAction.CallbackContext context)
    {
        OnShoot?.Invoke();
    }

    IEnumerator ShootLoop()
    {
        float inputValue = playerInputActions.Controls.Shoot.ReadValue<float>();
        while (inputValue != 0)
        {
            OnShoot?.Invoke();
            yield return new WaitForSeconds(0.01f);
        }

    }

}
