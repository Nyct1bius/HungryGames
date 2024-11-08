using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
    [Header("Meshes")]
    [SerializeField] private GameObject head;
    [SerializeField] private SkinnedMeshRenderer torso;
    [SerializeField] private SkinnedMeshRenderer legs;

    [Header("Alternative Materials")]
    [SerializeField] private Material alternativeHeadMat;
    [SerializeField] private Material alternativeTorsoMat;
    [SerializeField] private Material alternativeLegMat;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            head.SetActive(false);
        }
        else
        {
            head.SetActive(true);
          
        }
        if (!IsHost)
        {
            SkinnedMeshRenderer headeRender = head.GetComponent<SkinnedMeshRenderer>();
            legs.material = alternativeLegMat;
            torso.material = alternativeTorsoMat;
            headeRender.material = alternativeHeadMat;
        }




    }

}
