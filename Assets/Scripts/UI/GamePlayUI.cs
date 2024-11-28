using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlayUI : MonoBehaviour
{
    public Timer timer;
    public GameObject _blueVictory;
    public GameObject _orengeVictory;
    public GameObject _empate;

    public GameObject _menuScreen;

    public TextMeshProUGUI _placar1;
    public TextMeshProUGUI _placar2;

    public int _score = 0;
    public int _score2 = 0;

    public bool _isPause = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p") || Input.GetKeyDown(KeyCode.Escape) && !_isPause)
        {
            _menuScreen.SetActive(true);
            _isPause = true;
        }
        else if(_isPause && Input.GetKeyDown("p") || Input.GetKeyDown(KeyCode.Escape))
        {
            _menuScreen.SetActive(false);
            _isPause = false;
        }
    }

    public void Titulo()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void FimDaPartida(bool host, bool empate)
    {
        timer.TimerOn = false;

        if(empate)
        {
            _empate.SetActive(true);
        }
        else
        {
            if (host)
            {
                _blueVictory.SetActive(true); // ficar ativa por 5 segundos e sair do jogo
            }
            else
            {
                _orengeVictory.SetActive(true);
            }
        }
        // parar timer
    }

    public void PlacarChange(bool host)
    {
        if(host)
        {
            _score++;
            _placar1.text = _score.ToString();
        }
        else
        {
            _score2++;
            _placar2.text = _score2.ToString(); 
        }
    }
}
