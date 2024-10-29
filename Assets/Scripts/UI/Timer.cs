using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

    public float TimeLeft;
    public bool TimerOn = false;

    public GamePlayUI gamePlayUI;

    public TextMeshProUGUI TimerText;
    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerOn)
        {
            if(TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                UpDateTimer(TimeLeft);
            }
            else
            {
                Debug.Log("Time is UP");
                TimeLeft = 0;
                TimerOn = false;
                gamePlayUI.FimDaPartida(true, true);
            }
        }
    }

    void UpDateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.Floor(currentTime / 60);
        float seconds = Mathf.Floor(currentTime % 60);

        TimerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
