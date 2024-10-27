using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlayUI : MonoBehaviour
{

    public GameObject _blueVictory;
    public GameObject _orengeVictory;
    public GameObject _empate;

    public GameObject _menuScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            _menuScreen.SetActive(true);
        }
    }

    public void Titulo()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void FimDaPartida()
    {

    }
}
