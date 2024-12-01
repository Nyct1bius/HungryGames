using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebuffObject : NetworkBehaviour
{
    private float enabledTimer = 15f;

    void Update()
    {
        enabledTimer -= Time.deltaTime;

        if (enabledTimer <= 0)
        {
            ActiveBuffServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ActiveBuffServerRpc()
    {
        ActiveClientRpc();
    }
    [ClientRpc]
    private void ActiveClientRpc()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        enabledTimer = 15f;
    }
    void OnTriggerEnter(Collider other)
    {
        PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();
        if (playerStats != null)
        {
            playerStats.GetComponent<PlayerStatsManager>().Debuff(0, 0.80f, 0);

            ActiveBuffServerRpc();
        }
    }
}
