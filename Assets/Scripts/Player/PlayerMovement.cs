using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform cameraTransform;
    InputManager inputManager;
    Rigidbody rb;
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        inputManager.OnJump += Jump;
    }
    private void OnDisable()
    {
        inputManager.OnJump -= Jump;
    }
    private void FixedUpdate()
    {
        Vector2 inputDirection = inputManager.GetNormalizedInputDirection();
        Vector3 moveDir = cameraTransform.forward * inputDirection.y + cameraTransform.right * inputDirection.x;
        moveDir.y = 0;
        rb.AddForce(moveDir * walkSpeed, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        if (IsGrounded())
        {

        }
    }

    private bool IsGrounded()
    {
        return true;
    }
}
