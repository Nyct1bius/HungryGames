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
    private float lastShootTime;
    private InputManager _inputManager;
    Ray mouseWorldPos;
    RaycastHit spawnBulletHit;
    TrailRenderer bulletTrail;
    ParticleSystem bulletHit;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentBulletIndex = 0;
    }
    private void Start()
    {
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
        if (!IsOwner) return;
        mouseWorldPos = Camera.main.ScreenPointToRay(crosshair.position);
        if (canShoot && currentAmmo >= 0)
        {
            Debug.Log(currentAmmo);
            currentAmmo--;
            canShoot = false;
            guns.types[currentBulletIndex].shootingSystem.Play();
            SpawnTrailServerRpc();
            StartCoroutine(FireRateDelay(guns.types[currentBulletIndex].fireRate));
        }
     
        CheckMag();
    }
    private void CheckMag()
    {
        if(currentAmmo <= 0)
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

        while(time < 0.3)
        {
            trail.transform.position = Vector3.Lerp(startPosition,hit.point,time);
            time += Time.deltaTime/ trail.time;
            yield return null;
        }
      
        trail.transform.position = hit.point;
        spawnBulletHit = hit;
        SpawnBulletImpactServerRpc();
        DestroyTrailServerRpc();
    }
    IEnumerator FireRateDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    [ServerRpc]
    private void SpawnTrailServerRpc()
    {
        if (Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
        {
            bulletTrail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            NetworkObject networkObject = bulletTrail.GetComponent<NetworkObject>();
            networkObject.Spawn();  // Spawn bullet across the network
            StartCoroutine(SpawnTrail(bulletTrail, hit));
        }
        

    }
    
    [ServerRpc]
    private void SpawnBulletImpactServerRpc()
    {
        bulletHit = Instantiate(guns.types[currentBulletIndex].ImpactParticleSystem, spawnBulletHit.point, Quaternion.LookRotation(spawnBulletHit.normal));
        NetworkObject networkObject = bulletHit.GetComponent<NetworkObject>();
        networkObject.Spawn();  // Spawn bullet across the network
    }

    [ServerRpc]
    private void DestroyTrailServerRpc()
    {
        NetworkObject trailNetworkObject = bulletTrail.GetComponent<NetworkObject>();
        trailNetworkObject.Despawn();  // Spawn bullet across the network
        Destroy(bulletTrail.gameObject, bulletTrail.time);
    }
}
