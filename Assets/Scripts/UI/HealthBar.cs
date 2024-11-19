using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    public Slider slider;
    public float smoothSpeed = 5f; // Velocidade da animação, ajustável no Inspector
    private float targetHealth; // Armazena o valor de destino da barra de vida

    [ServerRpc]
    public void SetMaxHealthServerRpc(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        targetHealth = health;
        HealthAnimClientRpc();
    }

    // Define o valor alvo da barra de vida
    [ServerRpc]
    public void SetHealthServerRpc(float health)
    {
        targetHealth = health;
        HealthAnimClientRpc();
    }
    [ClientRpc]
    void HealthAnimClientRpc()
    {
        // Interpola suavemente o valor atual até o valor alvo
        slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);
    }

    void Update()
    {
        // Interpola suavemente o valor atual até o valor alvo
       // slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);
    }
}