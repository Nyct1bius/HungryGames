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

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        targetHealth = health;
        HealthAnim();
    }

    // Define o valor alvo da barra de vida
    public void SetHealth(float health)
    {
        targetHealth = health;
        HealthAnim();
    }
    void HealthAnim()
    {
        // Interpola suavemente o valor atual até o valor alvo
        slider.value = targetHealth;
        //slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);
    }

    void Update()
    {
        // Interpola suavemente o valor atual até o valor alvo
       // slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);
    }
}