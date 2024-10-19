using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {

    public InputField addressText;
    public Canvas lobbyCanvas;
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        addressText.text = PlayerPrefs.GetString("Address", "127.0.0.1");
    }

    private void FixedUpdate()
    {
        lobbyCanvas.gameObject.SetActive(!networkManager.IsClientConnected() && SceneManager.GetActiveScene().name == "Battleground");

    }

    public void Host()
    {
        
        networkManager.networkPort = 7766;
        networkManager.maxConnections = 2;
        networkManager.StartHost();
        GameManager.instance.isPracticeMode = false;

    }

    public void Connect()
    {
        PlayerPrefs.SetString("Address", addressText.text);
        networkManager.networkAddress = addressText.text;
        networkManager.networkPort = 7766;
        networkManager.maxConnections = 2;
        networkManager.StartClient();
        GameManager.instance.isPracticeMode = false;

    }

    public void Practice()
    {
        ConnectionConfig cc = new ConnectionConfig();
        networkManager.StartHost(cc, 1);
        GameManager.instance.isPracticeMode = true;
    }

}
