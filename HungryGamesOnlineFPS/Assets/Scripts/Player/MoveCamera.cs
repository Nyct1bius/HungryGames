using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraraTransform;
    private void FixedUpdate()
    {
        transform.position = cameraraTransform.position;
    }
}
