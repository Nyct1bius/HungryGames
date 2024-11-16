using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class PlayerCameraManager : MonoBehaviour
{
    private float _sensibility;
    private GameObject _playerRef;
    private Transform _playerVisual;

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
        Vector3 cameraPosOffset = new Vector3(_playerVisual.position.x, _playerVisual.position.y + 3.5f, _playerVisual.position.z);
        transform.position = cameraPosOffset;
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
