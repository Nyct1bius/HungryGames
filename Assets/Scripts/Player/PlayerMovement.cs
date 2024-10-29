using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement variables")]
    [SerializeField] private float speed;
    private float currentSpeed;


    [Header("Camera variables")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private GameObject fpsCamera;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private GunManager gunManager;
    [Range(0, 10)]
    [SerializeField] private float sensibility;
    //Components
    InputManager inputManager;
    Rigidbody rb;
    private GameObject cameraRef;


    [Header("Jump variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float detectionSphereRadius;
    [Range (0, -5)]
    [SerializeField] private float gravity;
    private float groundedGravity = -0.05f;

    [Header("Crosshair variables")]
    [SerializeField] private RectTransform crosshair;
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        cameraRef = Instantiate(fpsCamera, cameraOffset.position, Quaternion.identity);
        PlayerCameraManager cameraManager = cameraRef.GetComponent<PlayerCameraManager>();
        cameraManager.SetupCameraVariables(this.gameObject, playerVisual, inputManager, sensibility, crosshair,gunManager, vCamera);

    }
    private void Start()
    {
        currentSpeed = speed;
    }
    private void OnEnable()
    {
        inputManager.OnJump += Jump;
        inputManager.OnRun += Run;
        inputManager.OnStopRun += StopRun;
    }
    private void OnDisable()
    {
        inputManager.OnJump -= Jump;
        inputManager.OnRun -= Run;
        inputManager.OnStopRun -= StopRun;
    }
    private void FixedUpdate()
    {
        HandleGravity();
        Movement();
    }
    private void Movement()
    {
        Vector2 inputDirection = inputManager.GetNormalizedInputDirection();
        Vector3 moveDir = cameraTransform.forward * inputDirection.y + cameraTransform.right * inputDirection.x;
        moveDir.y = 0;
        rb.AddForce(moveDir * currentSpeed, ForceMode.VelocityChange);
    }
    private void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce * 10, ForceMode.Impulse);
        }
       
    }
    private void HandleGravity()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * groundedGravity, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.VelocityChange);
        }
    }

    private void Run()
    {
        currentSpeed *= 2;
    }

    private void StopRun()
    {
        currentSpeed = speed;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, detectionSphereRadius, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, detectionSphereRadius);
    }
}
