using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DespawnTrail : NetworkBehaviour
{
    public int bulletIndex;
    [SerializeField] private Guns guns;
    [SerializeField] private GameObject bulletVisual;

    private void Start()
    {

        StartCoroutine(delayToDestroyBullet());
    }
    IEnumerator delayToDestroyBullet()
    {
        yield return new WaitForSeconds(1.5f);
        DestroyTrailServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyTrailServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        PlayerStatsManager playerStats = collision.gameObject.GetComponent<PlayerStatsManager>();
        if (playerStats != null)
        {
            playerStats.Damage(guns.types[bulletIndex].damage);
        }

        StartCoroutine(delayToDestroyBullet());
    }
}

