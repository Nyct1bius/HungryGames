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
        if (!IsOwner) return;
        animator.SetBool("IsWalking", true);
    }
    private void StopWalk()
    {
        if (!IsOwner) return;
        animator.SetBool("IsWalking", false);
        StopRun();
    }
    private void Run()
    {
        if (!IsOwner) return;
        animator.SetBool("IsRunning", true);
    }

    private void StopRun()
    {
        if (!IsOwner) return;
        animator.SetBool("IsRunning", false);
    }

    public void PlayShootAnimation()
    {
        animator.SetTrigger("IsShooting");
    }
    public void PlayReloadAnimation()
    {
        animator.SetTrigger("IsReloading");
    }

}
