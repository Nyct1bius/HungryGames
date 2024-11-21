using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestesUI : MonoBehaviour
{

    public HealthBar healthBar;
    public GamePlayUI gamePlayUI;

    public int _playerMaxHealth = 100;
    public int _playerCurrentHealth = 100;

    public int _playerDeath = 0;

    void Start()
    {
        healthBar.SetMaxHealth(_playerMaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(_playerCurrentHealth);

        if (_playerDeath >= 3 )
        {
            gamePlayUI.FimDaPartida(true, false);
        }
    }

    public void Dano()
    {
        if(_playerDeath < 3)
        {
            if (_playerCurrentHealth > 0)
            {
                _playerCurrentHealth -= 10;
            }
            else
            {
                _playerDeath++;
                _playerCurrentHealth = _playerMaxHealth;
                gamePlayUI.PlacarChange(true);
            }
        }
        else
        {
            gamePlayUI.FimDaPartida(true, false);
        }
        
    }

}
