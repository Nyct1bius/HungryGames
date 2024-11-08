using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffObject : MonoBehaviour
{
    [SerializeField]
    bool isDamageBuff, isSpeedBuff;

    [SerializeField]
    GameObject player;

    private float enabledTimer = 20f;

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
        enabledTimer = 20f;
    }

    void OnTriggerEnter(Collider other)
    {   
        PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();
        if (playerStats != null)
        {
            if (isDamageBuff)
            {
                playerStats.GetComponent<PlayerStatsManager>().Buff(1.2f, 1, 1);
            }
            if (isSpeedBuff)
            {
                playerStats.GetComponent<PlayerStatsManager>().Buff(1, 1.25f, 1);
            }
            else
            {
                playerStats.GetComponent<PlayerStatsManager>().Buff(1, 1, 1.1f);
            }
        }
    }
}
