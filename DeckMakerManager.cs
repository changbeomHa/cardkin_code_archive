using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeckMakerManager : MonoBehaviour {

	
	public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        UISFXManager.instance.Play("Selection");
    }
}
