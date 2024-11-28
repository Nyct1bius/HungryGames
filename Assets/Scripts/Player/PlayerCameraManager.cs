using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerCameraManager : MonoBehaviour
{
    private float _sensibility;
    private GameObject _playerRef;
    private Transform _playerHeadRef;
    private Transform playerBody;
    private PlayerVisual _playerVisual;
    private Transform _alternativeHead;

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
        Vector2 mousePos = _inputManager.GetMouseDelta();
        float mouseX = mousePos.x * Time.deltaTime * _sensibility;
        float mouseY = mousePos.y * Time.deltaTime * _sensibility;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -55, 70);

        //Rotate Camera and orietation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        _playerVisual.AimDirection(-xRotation);
        playerBody.Rotate(Vector3.up * mouseX);

        if(xRotation > 10)
        {
  
            Vector3 cameraPosOffset = new Vector3(_alternativeHead.position.x, _alternativeHead.position.y + 0.3f, _alternativeHead.position.z);
            transform.position = cameraPosOffset;
        }
        else
        {
            Vector3 cameraPosOffset = new Vector3(_playerHeadRef.position.x, _playerHeadRef.position.y + 0.3f, _playerHeadRef.position.z);
            transform.position = cameraPosOffset;
        }

    }

    public void SetupCameraVariables(GameObject playerRef, Transform playerVisual,Transform alternativeHead, Transform playerBodyRef, InputManager inputManager, float sensibility, 
                                     RectTransform crosshair, GunManager gunManager, CinemachineVirtualCamera vCamera,PlayerVisual animationManager)
    {
        _alternativeHead = alternativeHead;
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
