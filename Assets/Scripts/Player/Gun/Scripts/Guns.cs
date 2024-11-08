using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Guns : ScriptableObject
{
    public List<GunTypes> types;
}
[Serializable]
public class GunTypes
{
    [Header("Info")]
    public string name;
    public int ID;
    [Header("Shooting")]
    public bool addBulletSpread = true;
    public Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    public float fireRate = 0.5f;
    public float maxDistance = 300f;
    public int damage;

    [Header("Effects")]
    public ParticleSystem shootingSystem;
    public ParticleSystem ImpactParticleSystem;
    public TrailRenderer bulletTrail;

    [Header("Gun capacity/reloading")]
    public int maxAmmo;
    public float reloadTime;
}
