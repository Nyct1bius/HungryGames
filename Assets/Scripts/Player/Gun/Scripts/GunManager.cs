using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
    [SerializeField] private InputManager inputManager;
    [SerializeField] private TextMeshProUGUI bulletCounterUI;
    [SerializeField] private TextMeshProUGUI gunNameUI;
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
        _inputManager.OnReload += CallReloadRoutine;
    }
    private void OnDisable()
    {
        _inputManager.OnReload -= CallReloadRoutine;
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (inputManager.PlayerShoot() && canShoot)
        {
            Shoot();
            Debug.Log("Fire");
        }
    }
 
    #region Shoot Logic

    private void Shoot()
    {
        mouseWorldPos = Camera.main.ScreenPointToRay(crosshair.position);
        if (currentAmmo > 0)
        {
            currentAmmo--;
            canShoot = false;
            if (Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
            {
                playerAnimations.PlayShootAnimation();
                SpawnTrailServerRpc(hit.point);
                StartCoroutine(WaitToDisplayBulletHit(hit));
            }
            else
            {
                SpawnTrailServerRpc(mouseWorldPos.direction);
            }
            //guns.types[currentBulletIndex].shootingSystem.Play();
            Debug.Log(currentAmmo);
            bulletCounterUI.text = currentAmmo.ToString();
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
            if (canReload)
            {
                playerAnimations.PlayReloadAnimation();
                StopCoroutine(FireRateDelay(guns.types[currentBulletIndex].fireRate));
                canShoot = false;
                StartCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));

            }
         
        }
    }
    private void CallReloadRoutine()
    {
        if (canReload)
        {
            StopCoroutine(FireRateDelay(guns.types[currentBulletIndex].fireRate));
            StopCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
            playerAnimations.PlayReloadAnimation();
            canShoot = false;
            currentAmmo = 0;
            StartCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
        }
    }
    IEnumerator Reload(float reloadTime)
    {
        Debug.Log("Reloading");
        canReload = false;
        yield return new WaitForSeconds(reloadTime);
        canReload = true; 
        canShoot = true;
        currentAmmo = guns.types[currentBulletIndex].maxAmmo;
        bulletCounterUI.text = currentAmmo.ToString();
    }
    #endregion
    #region Move Bullet To Target
    private IEnumerator MoveTrailToHitServer(GameObject trail, Vector3 endPos)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 0.5f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPos, time);
            time += Time.deltaTime/0.1f;
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
        GameObject bulletTrail = Instantiate(guns.types[currentBulletIndex].bulletVFX, bulletSpawnPoint.position, Quaternion.identity);
        bulletTrail.GetComponent<DespawnTrail>().bulletIndex = currentBulletIndex;
        bulletTrail.GetComponent<NetworkObject>().Spawn(true);
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
