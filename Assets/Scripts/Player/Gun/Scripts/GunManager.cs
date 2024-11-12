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
    [SerializeField] private Guns guns;
    private bool canShoot = true;
    private int currentAmmo;
    private RectTransform crosshair;
    public int currentBulletIndex;

    private Animator anim;
    private InputManager _inputManager;
    Ray mouseWorldPos;
    RaycastHit spawnBulletHit;
    TrailRenderer bulletTrail;
    ParticleSystem bulletHit;
    private GameObject target;
    RaycastHit hitBullet;
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
    }
    private void Update()
    {
        if (!IsOwner) return;
        Debug.DrawLine(mouseWorldPos.origin, spawnBulletHit.point, Color.green);
    }
    private void OnDisable()
    {
        _inputManager.OnShoot -= Shoot;
    }

    private void Shoot()
    {
        if(!IsOwner) return;
        mouseWorldPos = Camera.main.ScreenPointToRay(crosshair.position);
        CheckMag();
        if (canShoot && currentAmmo > 0)
        {
            currentAmmo--;
            canShoot = false;
            if (Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
            {
                bulletTrail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
                SpawnTrailServerRpc(hit.point);
                spawnBulletHit = hit;
                target = hit.collider.gameObject;
                StartCoroutine(SpawnTrail(bulletTrail, hit));
            }
            //guns.types[currentBulletIndex].shootingSystem.Play();
            Debug.Log(currentAmmo);

            StartCoroutine(FireRateDelay(guns.types[currentBulletIndex].fireRate));
        }

        
    }
    private void CheckMag()
    {
        if (currentAmmo == 0)
        {
            StartCoroutine(Reload(guns.types[currentBulletIndex].reloadTime));
        }
    }
    IEnumerator Reload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = guns.types[currentBulletIndex].maxAmmo;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit endPosition)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 2)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        if (IsOwner)
        {
            trail.transform.position = endPosition.point;
            SpawnBulletImpactServerRpc();
            DealsDamageServerRpc();
            DestroyTrailServerRpc(trail.time);
        }
    }
    IEnumerator FireRateDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    #region ClientCalls
    [ServerRpc]
    private void DealsDamageServerRpc()
    {
        PlayerStatsManager stats = target.GetComponent<PlayerStatsManager>();
        if (stats != null && target != NetworkManager.LocalClient.PlayerObject)
        {
            stats.Damage(guns.types[currentBulletIndex].damage);
        }
    }

    [ServerRpc]
    private void SpawnTrailServerRpc(Vector3 pointToGo)
    {
        bulletTrail.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    private void SpawnBulletImpactServerRpc()
    {
        bulletHit = Instantiate(guns.types[currentBulletIndex].ImpactParticleSystem, spawnBulletHit.point, Quaternion.LookRotation(spawnBulletHit.normal));
        bulletHit.GetComponent<NetworkObject>().Spawn();
    }


    [ServerRpc]
    private void DestroyTrailServerRpc(float time)
    {
        bulletTrail.GetComponent<NetworkObject>().Despawn();
        Destroy(bulletTrail.gameObject, time);
    }
    #endregion
}
