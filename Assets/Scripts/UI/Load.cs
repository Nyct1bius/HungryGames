using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class Load : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EsperaSegundos());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CarregarCena()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("TesteM");
        while (!asyncOperation.isDone)
        {
            //Debug.Log("Carregando: " + (asyncOperation.progress * 100f) + "%");
            //this.barraProgresso.value = asyncOperation.progress;
            yield return null;
        }
    }

    private IEnumerator EsperaSegundos()
    {
        yield return new WaitForSeconds(5);
 
       NetworkManager.Singleton.SceneManager.LoadScene("Lobby",LoadSceneMode.Single);
    }
}
