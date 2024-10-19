using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class WaitingForPlayerScreen : MonoBehaviour {

    public Canvas canvas;
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    void Update () {

        canvas.enabled = !GameManager.instance.isGameFinished && !GameManager.instance.isPracticeMode && networkManager.numPlayers < 2 && networkManager.numPlayers != 0 && networkManager.IsClientConnected();
        
	}
}
