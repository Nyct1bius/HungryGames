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
    private int currentHealth;

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            // gameObject.layer = playerLocalLayer;
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);
            StartCoroutine(WaitToMove());
        }
        else
        {

            healthBar.gameObject.SetActive(false);
        }

    }
    IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(0.1f);
        MatchManager.localInstance.PlayerToSpawnLocation(this.transform);
    }
    public void Buff(float damageMultiplierBuff, float speedMultiplierBuff, float armorBuff)
    {

    }

    public void Damage(int damage)
    {
        if (!IsOwner) return;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Death();
        }
    }
    private void Death()
    {
        playerMovement.enabled = false;
        inputManager.enabled = false;
        playerVisual.PlayDeathAnim();
    }
    public void Debuff(float damageMultiplierDebuff, float speedMultiplierDebuff, float armorDebuff)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        DespawnTrail despawnTrail = collision.gameObject.GetComponent<DespawnTrail>();
        if(despawnTrail != null)
        {
            Damage(50);
        }
    }
}
