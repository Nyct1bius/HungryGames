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

    public event Action OnWalk,OnStopWalk, OnJump,OnRun,OnStopRun, OnShoot,OnReload; 
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
        playerInputActions.Controls.Shoot.performed += PlayerShoot;
        playerInputActions.Controls.Reload.performed += PlayerReload;
    }
    private void OnDisable()
    {
        playerInputActions.Controls.Disable();
        playerInputActions.Controls.Move.performed -= PlayerMove;
        playerInputActions.Controls.Move.canceled -= PlayerMove;
        playerInputActions.Controls.Jump.performed -= PlayerJumped;
        playerInputActions.Controls.Run.performed -= PlayerRun;
        playerInputActions.Controls.Run.canceled -= PlayerStopRun;
        playerInputActions.Controls.Shoot.performed -= PlayerShoot;
        playerInputActions.Controls.Reload.performed -= PlayerReload;
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
    private void PlayerShoot(InputAction.CallbackContext context)
    {
        OnShoot?.Invoke();
    }


}
