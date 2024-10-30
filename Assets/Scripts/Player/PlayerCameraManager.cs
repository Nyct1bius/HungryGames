using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class PlayerCameraManager : NetworkBehaviour
{
    private float _sensibility;
    private GameObject _playerRef;
    private Transform _playerVisual;

    private GunManager _gunManager;
    private CinemachineVirtualCamera fpsCamera;
 

    private InputManager _inputManager;
    private float xRotation;
    private float yRotation;
    private Vector3 _cameraOffset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!IsOwner) return;
        transform.position = new Vector3(_playerRef.transform.position.x, _playerRef.transform.position.y + 0.7f,  _playerRef.transform.position.z);
        Vector2 mousePos = _inputManager.GetMouseDelta();
        float mouseX = mousePos.x * Time.deltaTime * _sensibility;
        float mouseY = mousePos.y * Time.deltaTime * _sensibility;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -75, 75);

        //Rotate Camera and orietation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        _playerVisual.Rotate(Vector3.up * mouseX);
    }

    public void SetupCameraVariables(GameObject playerRef, Transform playerVisual, InputManager inputManager, float sensibility, 
                                     RectTransform crosshair, GunManager gunManager, CinemachineVirtualCamera vCamera)
    {
        _playerRef = playerRef;
        _playerVisual = playerVisual;
        _inputManager = inputManager;
        _sensibility = sensibility * 10;
        _gunManager = gunManager;
        vCamera.Follow = transform;
        vCamera.LookAt = transform;
        _gunManager.SetupVariables(inputManager, crosshair);
    }

}
