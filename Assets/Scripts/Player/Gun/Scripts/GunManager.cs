using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : NetworkBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private LayerMask mask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Guns guns;
    [SerializeField] private PlayerVisual playerAnimations;
    private bool canShoot = true;
    private int currentAmmo;
    private RectTransform crosshair;
    private bool canReload = true;
    public int currentBulletIndex;

    private Animator anim;
    private InputManager _inputManager;
    Ray mouseWorldPos;
    RaycastHit spawnBulletHit;
    private GameObject localTarget;
    private ulong localTargetID;
    TrailRenderer localTrail;
    public override void OnNetworkSpawn()
    {
        currentBulletIndex = 0;
        currentAmmo = guns.types[currentBulletIndex].maxAmmo;
    }
    public void SetupVariables(InputManager inputManager, RectTransform crosshair)
    {
        _inputManager = inputManager;
        this.crosshair = crosshair;
        _inputManager.OnShoot += Shoot;
        _inputManager.OnReload += CallReloadRoutine;
    }
    private void Update()
    {
        if (!IsOwner) return;
        Debug.DrawLine(mouseWorldPos.origin, spawnBulletHit.point, Color.green);
    }
    private void OnDisable()
    {
        _inputManager.OnShoot -= Shoot;
        _inputManager.OnShoot -= CallReloadRoutine;
    }
    #region Shoot Logic
    private void Shoot()
    {
        if (!IsOwner) return;
        mouseWorldPos = Camera.main.ScreenPointToRay(crosshair.position);
        if (canShoot && currentAmmo > 0)
        {
            currentAmmo--;
            canShoot = false;
            playerAnimations.PlayShootAnimation();
            if (Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
            {
                SpawnTrailServerRpc(hit.point);
                StartCoroutine(WaitToDisplayBulletHit(hit));
            }
            else
            {
                SpawnTrailServerRpc(hit.point);
            }
            //guns.types[currentBulletIndex].shootingSystem.Play();
            Debug.Log(currentAmmo);

            StartCoroutine(FireRateDelay(guns.types[currentBulletIndex].fireRate));
        }

        CheckMag();
    }
    IEnumerator FireRateDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }
    private void CheckMag()
    {
        if (currentAmmo == 0)
        {
            StartCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
        }
    }
    private void CallReloadRoutine()
    {
        if (canReload)
        {
            StopCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
            currentAmmo = 0;
            StartCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
        }
    }
    IEnumerator Reload(float reloadTime)
    {
        Debug.Log("Reloading");
        canReload = false;
        playerAnimations.PlayReloadAnimation();
        yield return new WaitForSeconds(reloadTime);
        canReload = true;
        currentAmmo = guns.types[currentBulletIndex].maxAmmo;
    }
    #endregion
    #region Move Bullet To Target
    private IEnumerator MoveTrailToHitServer(TrailRenderer trail, Vector3 endPos)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPos, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = endPos;
    }
    private IEnumerator WaitToDisplayBulletHit(RaycastHit hit)
    {
        yield return new WaitForSeconds(0.15f);
        SpawnBulletImpactServerRpc(hit.point, hit.normal);      
    }
    #endregion
    #region ClientCalls


    [ServerRpc]
    private void SpawnTrailServerRpc(Vector3 endPos)
    {
        TrailRenderer bulletTrail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
        bulletTrail.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(MoveTrailToHitServer(bulletTrail, endPos));
    }

    [ServerRpc]
    private void SpawnBulletImpactServerRpc(Vector3 spawnPos,Vector3 normal)
    {
        ParticleSystem bulletHit = Instantiate(guns.types[currentBulletIndex].ImpactParticleSystem, spawnPos, Quaternion.LookRotation(normal));
        bulletHit.GetComponent<NetworkObject>().Spawn();
    }
    #endregion
}
