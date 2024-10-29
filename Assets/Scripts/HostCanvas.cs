using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostCanvas : MonoBehaviour
{
 
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void Server()
    {
        NetworkManager.Singleton.StartServer();
    }
}
