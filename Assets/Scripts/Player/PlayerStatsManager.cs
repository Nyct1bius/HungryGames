using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour,IBuffable
{
    [SerializeField] public HealthBar healthBar;
    [SerializeField] public int maxHealth;
    [SerializeField] public int maxSpeed;
    private int currentHealth;

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

    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealthServerRpc(currentHealth);
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
      
    }

}
