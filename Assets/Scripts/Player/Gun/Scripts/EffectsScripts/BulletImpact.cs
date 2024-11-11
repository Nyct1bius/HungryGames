using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletImpact : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        ParticleSystem pSystem = GetComponent<ParticleSystem>();
        float lifetime = pSystem.main.duration + 0.5f;
        StartCoroutine(DespawnBulletImpact(lifetime));
    }
    IEnumerator DespawnBulletImpact(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); 
        DestroyBulletImpactServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyBulletImpactServerRpc()
    {
        NetworkObject impactNetworkObject = gameObject.GetComponent<NetworkObject>();
        impactNetworkObject.Despawn();  // Spawn bullet across the network
        Destroy(gameObject);
    }

}
