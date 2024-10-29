using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;
    private Vector3 offset;
    private void Start()
    {
        offset = transform.position;
    }
    void Update()
    {
        transform.position = cameraPos.position + offset;
    }
}
