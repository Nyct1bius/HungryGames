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
           // gameObject.layer = playerLocalLayer;
            currentHealth = maxHealth;
            healthBar.SetMaxHealthServerRpc(currentHealth);
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
        currentHealth -= damage;
        healthBar.SetHealthServerRpc(currentHealth);
        Debug.Log(currentHealth);
    }

    public void Debuff(float damageMultiplierDebuff, float speedMultiplierDebuff, float armorDebuff)
    {
        
    }

}
