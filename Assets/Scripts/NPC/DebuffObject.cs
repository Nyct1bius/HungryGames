using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffObject : MonoBehaviour
{
    private float enabledTimer = 15f;

    void Update()
    {
        enabledTimer -= Time.deltaTime;

        if (enabledTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        enabledTimer = 15f;
    }
    private void OnDisable()
    {
        enabledTimer = 15f;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();
        if (playerStats != null)
        {
            playerStats.GetComponent<PlayerStatsManager>().Debuff(0, 0.80f, 0);

            gameObject.SetActive(false);
        }
    }
}
