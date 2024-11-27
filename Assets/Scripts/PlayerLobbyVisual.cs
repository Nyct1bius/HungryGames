using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobbyVisual : NetworkBehaviour
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
        if(!IsHost)
        {
            head.SetActive(true);
            SkinnedMeshRenderer headeRender = head.GetComponent<SkinnedMeshRenderer>();
            legs.material = alternativeLegMat;
            torso.material = alternativeTorsoMat;
            headeRender.material = alternativeHeadMat;
        }

    }
}
