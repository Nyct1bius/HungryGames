using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Guns guns;
    private RectTransform crosshair;
    public int currentBulletIndex;

    private Animator anim;
    private float lastShootTime;
    private InputManager _inputManager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentBulletIndex = 0;
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
        if (lastShootTime + guns.types[currentBulletIndex].fireRate < Time.time)
        {
            
            guns.types[currentBulletIndex].shootingSystem.Play();
            Vector3 direction = GetDirection();

            if(Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
            {
                TrailRenderer trail = Instantiate(guns.types[currentBulletIndex].bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
                lastShootTime = Time.deltaTime;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
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
   
}
