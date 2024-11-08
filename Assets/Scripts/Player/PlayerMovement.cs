using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement variables")]
    [SerializeField] private float speed;
    private float currentSpeed;


    [Header("Camera variables")]
    [SerializeField] private GameObject mainCameraPrefab;
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private GameObject fpsCameraPrefab;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private CinemachineVirtualCamera vCamera;
    [SerializeField] private GunManager gunManager;
    [Range(0, 10)]
    [SerializeField] private float sensibility;
    private AudioListener listerner;
    //Components
    InputManager inputManager;
    Rigidbody rb;
    private GameObject fpsCameraRef;
    private GameObject mainCameraRef;


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

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetupCameraVariablesIsOwner();
        }
        else
        {
            vCamera.Priority = -10;
        }
       
    }
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
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
        if(!IsOwner)
        {
            return;
        }

        HandleGravity();
        Movement();
    }
    private void Movement()
    {
        Vector2 inputDirection = inputManager.GetNormalizedInputDirection();
        Vector3 moveDir = mainCameraRef.transform.forward * inputDirection.y + mainCameraRef.transform.right * inputDirection.x;
        moveDir.y = 0;
        rb.AddForce(moveDir * currentSpeed, ForceMode.VelocityChange);
    }
    private void Jump()
    {
        if (!IsOwner)
        {
            return;
        }

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
        if (!IsOwner)
        {
            return;
        }

        currentSpeed *= 2;
    }

    private void StopRun()
    {
        if (!IsOwner)
        {
            return;
        }

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

    private void SetupCameraVariablesIsOwner()
    {
        mainCameraRef = Instantiate(mainCameraPrefab);
        listerner = mainCameraRef.GetComponent<AudioListener>();
        listerner.enabled = true;
        fpsCameraRef = Instantiate(fpsCameraPrefab, cameraOffset.position, Quaternion.identity);
        vCamera.Priority = 10;
        PlayerCameraManager cameraManager = fpsCameraRef.GetComponent<PlayerCameraManager>();
        cameraManager.SetupCameraVariables(this.gameObject, playerVisual, inputManager, sensibility, crosshair, gunManager, vCamera);
        SpawnCamerasServerRpc();
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (GameObject camera in cameras)
        {
            if (camera != mainCameraRef)
            {
                camera.SetActive(false);
            }
        }
    }

    [ServerRpc]
    private void SpawnCamerasServerRpc()
    {
        NetworkObject nObjectCam1 = fpsCameraRef.GetComponent<NetworkObject>();
        nObjectCam1.Spawn();
        NetworkObject nObjectCam2 = mainCameraRef.GetComponent<NetworkObject>();
        nObjectCam2.Spawn();
    }
}
