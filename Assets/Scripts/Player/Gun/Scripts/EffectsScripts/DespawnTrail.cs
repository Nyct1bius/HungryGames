using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DespawnTrail : NetworkBehaviour
{
    public int bulletIndex;
    [SerializeField] private GameObject bulletVisual;
    public override void OnNetworkSpawn()
    {
        StartCoroutine(waitToDestory());
    }
    IEnumerator waitToDestory()
    {
        yield return new WaitForSeconds(1.2f);
        DestroyTrailServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DisableBulletServerRpc()
    {
        bulletVisual.SetActive(false);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyTrailServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this);
    }
}

