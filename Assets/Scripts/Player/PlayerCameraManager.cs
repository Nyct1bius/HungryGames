using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraManager : MonoBehaviour
{
    private float sensibility;
    [SerializeField] private Transform playerVisual;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    private CinemachinePOV povCamera;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sensibility = GetComponentInParent<PlayerMovement>().sensibility;
        povCamera = fpsCamera.GetCinemachineComponent<CinemachinePOV>();
        povCamera.m_VerticalAxis.m_MaxSpeed = sensibility;
        povCamera.m_HorizontalAxis.m_MaxSpeed = sensibility;
    }
    private void FixedUpdate()
    {
        //Vector2 mousePos = inputManager.GetMouseDelta();
        //float mouseX = mousePos.x * Time.deltaTime * sensibility;
        //float mouseY = mousePos.y * Time.deltaTime * sensibility;

        //yRotation += mouseX;
        //xRotation -= mouseY;

        //xRotation = Mathf.Clamp(xRotation, -90, 90);

        ////Rotate Camera and orietation
        //transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerVisual.rotation = Quaternion.Euler(new Vector3(0, povCamera.m_HorizontalAxis.Value,0));
    }
}
