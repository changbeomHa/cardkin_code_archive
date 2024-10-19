using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CleanupManager : NetworkBehaviour {

    private NetworkManager networkManager;
    public Transform hand;
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    void Update () {
        if (!networkManager.IsClientConnected() || (GameManager.instance != null && GameManager.instance.isGameFinished))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            foreach(Transform t in hand)
            {
                Destroy(t.gameObject);
            }
        }
	}
}
