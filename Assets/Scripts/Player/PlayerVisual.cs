using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
    [Header("Meshes")]
    [SerializeField] private GameObject head;
    [SerializeField] private SkinnedMeshRenderer torso;
    [SerializeField] private SkinnedMeshRenderer legs;

    [Header("Alternative Materials")]
    [SerializeField] private Material alternativeHeadMat;
    [SerializeField] private Material alternativeTorsoMat;
    [SerializeField] private Material alternativeLegMat;

    [Header("Components")]
    [SerializeField] private InputManager inputManager;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        animator = GetComponent<Animator>();
        if (IsOwner)
        {
            head.SetActive(false);
        }
        else
        {
            head.SetActive(true);
          
        }
        if (!IsHost)
        {
            SkinnedMeshRenderer headeRender = head.GetComponent<SkinnedMeshRenderer>();
            legs.material = alternativeLegMat;
            torso.material = alternativeTorsoMat;
            headeRender.material = alternativeHeadMat;
        }

    }
    private void OnEnable()
    {
        inputManager.OnWalk += Walk;
        inputManager.OnStopWalk += StopWalk;
        inputManager.OnRun += Run;
        inputManager.OnStopRun += StopRun;
    }
    private void OnDisable()
    {
        inputManager.OnWalk -= Walk;
        inputManager.OnStopWalk -= StopWalk;
        inputManager.OnRun -= Run;
        inputManager.OnStopRun -= StopRun;
    }
    private void Walk()
    {
        animator.SetBool("IsWalking", true);
    }
    private void StopWalk()
    {
        animator.SetBool("IsWalking", false);
    }
    private void Run()
    {
        animator.SetBool("IsRunning", true);
    }

    private void StopRun()
    {
        animator.SetBool("IsRunning", false);
    }


}
