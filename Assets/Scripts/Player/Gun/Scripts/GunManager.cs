using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
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
        mouseWorldPos = Camera.main.ScreenPointToRay(crosshair.position);
        if (canShoot && currentAmmo >= 0)
        {
            Debug.Log(currentAmmo);
            currentAmmo--;
            canShoot = false;
            guns.types[currentBulletIndex].shootingSystem.Play();
            if(Physics.Raycast(mouseWorldPos, out RaycastHit hit, float.MaxValue, mask))
            {
                TrailRenderer trail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
            }
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
        Instantiate(guns.types[currentBulletIndex].ImpactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
    IEnumerator FireRateDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }
   
}
