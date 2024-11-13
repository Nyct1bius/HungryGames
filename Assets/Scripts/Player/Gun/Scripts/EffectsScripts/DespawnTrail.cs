using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DespawnTrail : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        StartCoroutine(waitToDestory());
    }

    IEnumerator waitToDestory()
    {
        yield return new WaitForSeconds(2);
        DestroyTrailServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyTrailServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}

