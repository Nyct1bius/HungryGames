using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
    [Header("Meshes")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject legs;

    [Header("Alternative Materials")]
    [SerializeField] private Material alternativeHeadMat;
    [SerializeField] private Material alternativeTorsoMat;
    [SerializeField] private Material alternativeLegMat;

    [Header("Components")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LayerMask noVisualizeByCamera;
    [Header("Audios")]
    [SerializeField] private AudioClip[] walkAudios;
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
            SkinnedMeshRenderer headeRender = head.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer legsRender = legs.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer torsoRender = torso.GetComponent<SkinnedMeshRenderer>();
            head.layer = noVisualizeByCamera;
            legs.layer = noVisualizeByCamera;   
            torso.layer = noVisualizeByCamera;
            legsRender.material = alternativeLegMat;
            torsoRender.material = alternativeTorsoMat;
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
    public void PlayDeathAnim()
    {
        animator.SetBool("IsDead",true);
        animator.SetTrigger("Died");
    }
    public void PlayReviveAnim()
    {
        animator.SetBool("IsDead", false);
    }
    public void AimDirection(float mouseY)
    {
        animator.SetFloat("MousePosY", mouseY);
    }
          
    private void Walk()
    {
        if (!IsOwner) return;
        if (inputManager.IsPaused()) return;
        animator.SetBool("IsWalking", true);
    }
    private void StopWalk()
    {
        if (!IsOwner) return;
        if (inputManager.IsPaused()) return;
        animator.SetBool("IsWalking", false);
        StopRun();
    }
    private void Run()
    {
        if (!IsOwner) return;
        if (inputManager.IsPaused()) return;
        animator.SetBool("IsRunning", true);
    }

    private void StopRun()
    {
        if (!IsOwner) return;
        if (inputManager.IsPaused()) return;
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

    [ServerRpc(RequireOwnership = false)]
    public void PlayWalkSoundServerRpc()
    {
        PlayWalkSoundClientRpc();
    }
    [ClientRpc]
    private void PlayWalkSoundClientRpc()
    {
        int randomNunber = UnityEngine.Random.Range(0, walkAudios.Length);
        AudioSource.PlayClipAtPoint(walkAudios[randomNunber], transform.position, 2f);
    }
}
