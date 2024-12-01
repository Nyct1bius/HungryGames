using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostCanvas : MonoBehaviour
{
    [SerializeField] private string inputClientText;
    [SerializeField] private string inputHostText;
    [SerializeField] private TMP_Text reactionClientTextBox;
    [SerializeField] private TMP_Text reactionHostTextBox;

    public void GrabFromInputFieldClient(string input)
    {
        inputClientText = input;
        DisplayReactionToInputClient();
    }

    private void DisplayReactionToInputClient()
    {
        reactionClientTextBox.text = inputClientText;
    }
    public void GrabFromInputFieldHost(string input)
    {
        inputHostText = input;
        DisplayReactionToInputHost();
    }

    private void DisplayReactionToInputHost()
    {
        reactionHostTextBox.text = inputHostText;
    }

    public void Host()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = inputHostText;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Load", LoadSceneMode.Single);
    }
    public void Client()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = inputClientText;
        NetworkManager.Singleton.StartClient();
    }
    public void Server()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
