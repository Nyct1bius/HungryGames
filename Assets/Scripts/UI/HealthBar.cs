using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float smoothSpeed = 5f; // Velocidade da animação, ajustável no Inspector
    private float targetHealth; // Armazena o valor de destino da barra de vida

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        targetHealth = health;
    }

    // Define o valor alvo da barra de vida
    public void SetHealth(float health)
    {
        targetHealth = health;
    }

    void Update()
    {
        // Interpola suavemente o valor atual até o valor alvo
        slider.value = Mathf.Lerp(slider.value, targetHealth, smoothSpeed * Time.deltaTime);
    }
}