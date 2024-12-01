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
    private bool shooting;
    private bool isPaused;
    public event Action OnWalk, OnStopWalk, OnJump, OnRun, OnStopRun, OnReload;
    private void Awake()
    {
        playerInputActions = new PlayerInputAction();
    }
    private void OnEnable()
    {
        playerInputActions.Controls.Enable();
        playerInputActions.Controls.Move.performed += PlayerMove;
        playerInputActions.Controls.Move.canceled += PlayerStopMoving;
        playerInputActions.Controls.Jump.performed += PlayerJumped;
        playerInputActions.Controls.Run.performed += PlayerRun;
        playerInputActions.Controls.Run.canceled += PlayerStopRun;
        playerInputActions.Controls.Reload.performed += PlayerReload;
        playerInputActions.Controls.Shoot.performed += ShootPerformed;
        playerInputActions.Controls.Shoot.canceled += ShootCanceled;
        playerInputActions.Controls.Paused.performed += PausePerformed;
    }
    private void OnDisable()
    {
        playerInputActions.Controls.Disable();
        playerInputActions.Controls.Move.performed -= PlayerMove;
        playerInputActions.Controls.Move.canceled -= PlayerMove;
        playerInputActions.Controls.Jump.performed -= PlayerJumped;
        playerInputActions.Controls.Run.performed -= PlayerRun;
        playerInputActions.Controls.Run.canceled -= PlayerStopRun;
        playerInputActions.Controls.Reload.performed -= PlayerReload;
        playerInputActions.Controls.Shoot.performed -= ShootPerformed;
        playerInputActions.Controls.Shoot.canceled -= ShootCanceled;
        playerInputActions.Controls.Paused.performed -= PausePerformed;
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
    private void PlayerMove(InputAction.CallbackContext context)
    {
        OnWalk?.Invoke();
    }
    private void PlayerReload(InputAction.CallbackContext context)
    {
        OnReload?.Invoke();
    }
    private void PlayerStopMoving(InputAction.CallbackContext context)
    {
        OnStopWalk?.Invoke();
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
    public bool PlayerShoot()
    {
        return shooting;
    }
    private void ShootPerformed(InputAction.CallbackContext context)
    {
        shooting = true;
    }
    private void ShootCanceled(InputAction.CallbackContext context)
    {
        shooting = false;
    }
    public bool IsPaused()
    {
        return isPaused;
    }
    private void PausePerformed(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
    }
}
