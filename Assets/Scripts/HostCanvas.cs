using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostCanvas : MonoBehaviour
{
 
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Load", LoadSceneMode.Single);
    }
    public void Client()
    {
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
