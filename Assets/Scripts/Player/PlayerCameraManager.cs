using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class PlayerCameraManager : MonoBehaviour
{
    private float _sensibility;
    private GameObject _playerRef;
    private Transform _playerHeadRef;
    private Transform playerBody;
    private PlayerVisual _playerVisual;

    private GunManager _gunManager;

    private InputManager _inputManager;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Vector3 cameraPosOffset = _playerHeadRef.position;
        transform.position = cameraPosOffset;
        Vector2 mousePos = _inputManager.GetMouseDelta();
        float mouseX = mousePos.x * Time.deltaTime * _sensibility;
        float mouseY = mousePos.y * Time.deltaTime * _sensibility;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -55, 55);

        //Rotate Camera and orietation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        _playerVisual.AimDirection(-xRotation);
        playerBody.Rotate(Vector3.up * mouseX);

    }

    public void SetupCameraVariables(GameObject playerRef, Transform playerVisual,Transform playerBodyRef, InputManager inputManager, float sensibility, 
                                     RectTransform crosshair, GunManager gunManager, CinemachineVirtualCamera vCamera,PlayerVisual animationManager)
    {
        _playerRef = playerRef;
        _playerHeadRef = playerVisual;
        playerBody = playerBodyRef;
        _inputManager = inputManager;
        _sensibility = sensibility * 10;
        _gunManager = gunManager;
        _playerVisual = animationManager;
        vCamera.Follow = transform;
        vCamera.LookAt = transform;
        _gunManager.SetupVariables(inputManager, crosshair);
    }
}
