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
    private int currentHealth;

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            transform.position = new Vector3(10, 10, 10);
            // gameObject.layer = playerLocalLayer;
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);
        }
        else
        {

            healthBar.gameObject.SetActive(false);
        }

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
            Damage(5);
        }
    }
}
