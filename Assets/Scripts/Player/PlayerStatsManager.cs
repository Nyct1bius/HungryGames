using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour, IBuffable
{
    [SerializeField] public HealthBar healthBar;
    [SerializeField] public int maxHealth;
    [SerializeField] public int maxSpeed;
    [SerializeField] private LayerMask playerLocalLayer;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Guns guns;
    [SerializeField] private GameObject playerMesh;
    private int currentHealth;


    private bool isDead;

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            // gameObject.layer = playerLocalLayer;
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);
        }
        else
        {

            healthBar.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        MatchManager.localInstance.OnFinishedMatch += FinishedMatch;
    }
    private void OnDisable()
    {
        MatchManager.localInstance.OnFinishedMatch -= FinishedMatch;
    }

    private void FinishedMatch()
    {
        playerMovement.enabled = false;
        inputManager.enabled = false;
    }

    public void Buff(float damageMultiplierBuff, float speedMultiplierBuff, float armorBuff)
    {

    }

    public void Damage(int damage)
    {
        if (!IsOwner) return;
        if (!isDead)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            Debug.Log(currentHealth);
            if (currentHealth <= 0)
            {
                Death();
            }
        }

    }
    private void Death()
    {
        isDead = true;
        playerVisual.PlayDeathAnim();
        playerMovement.enabled = false;
        inputManager.enabled = false;
        MatchManager.localInstance.PlayerDied(gameObject, playerMesh, OwnerClientId);
    }
    public void Revive()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        isDead = false;
        playerMovement.enabled = true;
        inputManager.enabled = true;
        playerVisual.PlayReviveAnim();

    }
    public void Debuff(float damageMultiplierDebuff, float speedMultiplierDebuff, float armorDebuff)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        DespawnTrail despawnTrail = collision.gameObject.GetComponent<DespawnTrail>();
        if (despawnTrail != null)
        {
            int currentBulletIndex = despawnTrail.bulletIndex;
            Damage(guns.types[currentBulletIndex].damage);
        }
    }
}
