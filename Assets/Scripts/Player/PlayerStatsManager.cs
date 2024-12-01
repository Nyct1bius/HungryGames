using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour, IBuffable
{
    public HealthBar healthBar;
    public int maxHealth;
    [SerializeField] private float maxSpeed;
    [HideInInspector] public float currentSpeed;
    [SerializeField] private float maxRunningSpeed;
    [HideInInspector] public float curentRunnigSpeed;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Guns guns;
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private LayerMask bulletLayer;


    private int currentHealth;
    private bool isDead;

    public override void OnNetworkSpawn()
    {
        currentSpeed = maxSpeed;
        curentRunnigSpeed = maxRunningSpeed;
        if (IsOwner)
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(currentHealth);
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        playerMovement.enabled = false;
        inputManager.enabled = false;
    }
    private void Update()
    {
        if (inputManager.IsPaused())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerMovement.enabled = false;
             MatchManager.localInstance.pauseMenuUI.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerMovement.enabled = true;
            MatchManager.localInstance.pauseMenuUI.SetActive(false);
        }
    }
    private void OnEnable()
    {
        MatchManager.localInstance.OnFinishedMatch += FinishedMatch;
        MatchManager.localInstance.OnStartMatch += MatchStart;
    }
    private void OnDisable()
    {
        MatchManager.localInstance.OnFinishedMatch -= FinishedMatch;
        MatchManager.localInstance.OnStartMatch -= MatchStart;
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
    private void MatchStart()
    {
        playerMovement.enabled = true;
        inputManager.enabled = true;
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
        currentSpeed = maxSpeed * speedMultiplierDebuff;
        curentRunnigSpeed = maxRunningSpeed * speedMultiplierDebuff;
    }
  
}
