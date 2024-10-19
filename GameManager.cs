using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour {
    public GameObject floatingText;
    public Transform spawnPointsParent;
    public GameObject dummy;
    public GameObject practiceModeIndicator;
    public GameObject menu;
    public Button concedeButton;
    public Button resumeButton;
    private WaitingForPlayerScreen wfpScreen;
    public Button mainMenuButton;
    public bool isPracticeMode;
    public Text gameFinishText;
    public bool isGameFinished;
    public bool isMenuOpen
    {
        get
        {
            return menu.activeSelf;
        }
    }

    public static GameManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    private static GameManager _instance;

    private void Awake()
    {
        spawnPointsParent.gameObject.SetActive(false);
        isPracticeMode = false;
        menu.gameObject.SetActive(false);
        wfpScreen = FindObjectOfType<WaitingForPlayerScreen>();
    }
    private void FixedUpdate()
    {
        practiceModeIndicator.SetActive(isPracticeMode);
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && resumeButton.interactable)
        {
            menu.SetActive(!menu.activeSelf);
            
        }

        if (HUDManager.instance.player != null && wfpScreen.canvas.enabled == false)
        {
            concedeButton.interactable = !isGameFinished && HUDManager.instance.player.health > 0;
        }
        else
        {
            concedeButton.interactable = false;
        }
        
        mainMenuButton.interactable = isGameFinished || isPracticeMode || wfpScreen.canvas.enabled;
        resumeButton.interactable = !isGameFinished;




        if (!isPracticeMode) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnDummy();
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            HUDManager.instance.player.health += 10;
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            HUDManager.instance.player.health -= 10;
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach(Transform t in HUDManager.instance.hand.transform)
            {
                if(t.GetComponent<AutoattackCard>() == null && t.GetComponent<Card>() != null)
                {
                    Destroy(t.gameObject);
                }


            }
            HUDManager.instance.player.magic.DrawCard();
            HUDManager.instance.player.magic.DrawCard();
            HUDManager.instance.player.magic.DrawCard();
            HUDManager.instance.player.magic.DrawCard();
            HUDManager.instance.player.magic.DrawCard(); // lol
            //to my defensse, it's probably faster than a for-loop
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            foreach(Transform t in HUDManager.instance.hand.transform)
            {
                t.GetComponent<Card>().Unlock();
            }
        }
    }

    public void onPlayerDeath(Player deadPlayer)
    {
        menu.SetActive(true);
        isGameFinished = true;
        if (deadPlayer.isLocalPlayer)
        {
            gameFinishText.text = "패배!";
        }
        else
        {
            gameFinishText.text = "승리!";
        }

    }

    public void Concede()
    {
        HUDManager.instance.player.health = 0;
        menu.SetActive(false);
    }

    public void Resume()
    {
        menu.SetActive(false);
    }
    public void MainMenu()
    {

        SceneManager.LoadScene("Main Menu");
        FindObjectOfType<NetworkManager>().StopClient();
        FindObjectOfType<NetworkManager>().StopServer();
    }
    public void SpawnDummy()
    {
        GameObject dumdum = Instantiate(dummy, GetBestSpawnPoint() + Random.insideUnitSphere * 2f, Quaternion.identity);
        NetworkServer.Spawn(dumdum);
    }

    public Vector3 GetBestSpawnPoint()
    {
        Vector3[] points = new Vector3[spawnPointsParent.childCount];
        float[] scores = new float[spawnPointsParent.childCount];
        Player[] players = FindObjectsOfType<Player>();

        for (int i = 0; i < spawnPointsParent.childCount; i++)
        {
            points[i] = spawnPointsParent.GetChild(i).position;
            scores[i] = 0f;
            foreach(Player player in players)
            {
                scores[i] += (player.transform.position - points[i]).magnitude;
            }
        }

        return points[scores.ToList().IndexOf(scores.Max())];
        

    }

    public void SpawnFloatingText(Vector3 position, string text, Color textColour)
    {
        if (isServer)
        {
            RpcSpawnFloatingText(position, text, textColour);
        }
        else
        {
            CmdSpawnFloatingText(position, text, textColour);
        }
    }


    [Command]
    public void CmdSpawnFloatingText(Vector3 position, string text, Color textColour)
    {
        RpcSpawnFloatingText(position, text, textColour);
    }

    [ClientRpc]
    public void RpcSpawnFloatingText(Vector3 position, string text, Color textColour)
    {
        FloatingText ft = Instantiate(floatingText, position, Quaternion.identity).GetComponent<FloatingText>();
        ft.text = text;
        ft.GetComponent<TextMesh>().color = textColour;

    }

    public void ClientSpawnFloatingText(Vector3 position, string text, Color textColour)
    {
        FloatingText ft = Instantiate(floatingText, position, Quaternion.identity).GetComponent<FloatingText>();
        ft.text = text;
        ft.GetComponent<TextMesh>().color = textColour;

    }
}
