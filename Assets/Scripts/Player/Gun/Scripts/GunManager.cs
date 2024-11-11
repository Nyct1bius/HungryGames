using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
            Debug.Log(currentAmmo);
            canShoot = false;
            //guns.types[currentBulletIndex].shootingSystem.Play();
            SpawnTrailServerRpc();

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

    private Vector3 GetDirection()
    {
        Vector3 direction = Camera.main.transform.forward;
        if (guns.types[currentBulletIndex].addBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-guns.types[currentBulletIndex].bulletSpreadVariance.x, -guns.types[currentBulletIndex].bulletSpreadVariance.x),
                Random.Range(-guns.types[currentBulletIndex].bulletSpreadVariance.y, -guns.types[currentBulletIndex].bulletSpreadVariance.y),
                Random.Range(-guns.types[currentBulletIndex].bulletSpreadVariance.z, -guns.types[currentBulletIndex].bulletSpreadVariance.z)
                );

            direction.Normalize();
        }
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 0.3)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hit.point;
        spawnBulletHit = hit;
        SpawnBulletImpactServerRpc();
        DealsDamageServerRpc();
        DestroyTrailServerRpc();
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
    private void SpawnTrailServerRpc()
    {
        if (Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
        {
            bulletTrail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            bulletTrail.GetComponent<NetworkObject>().Spawn(true);
            StartCoroutine(SpawnTrail(bulletTrail, hit));
            target = hit.collider.gameObject;
            Debug.Log(target);

        }
    }

    [ServerRpc]
    private void SpawnBulletImpactServerRpc()
    {
        bulletHit = Instantiate(guns.types[currentBulletIndex].ImpactParticleSystem, spawnBulletHit.point, Quaternion.LookRotation(spawnBulletHit.normal));
        bulletHit.GetComponent<NetworkObject>().Spawn(true);
    }


    [ServerRpc]
    private void DestroyTrailServerRpc()
    {
        bulletTrail.GetComponent<NetworkObject>().Despawn();
        Destroy(bulletTrail.gameObject, bulletTrail.time);
    }
    #endregion
}
