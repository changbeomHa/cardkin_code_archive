using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
public class MainMenuManager : MonoBehaviour {
    public Color defaultTextColour;
    public Color hoverTextColour;
    public LayerMask whatIsText;
    public Animator doorAnimator;
    private TextMesh hoveringText;
    public CinemachineVirtualCamera enteringTheDoorCamera;
    private Camera cam;
    public GameObject enterThePlayCurtain;
    private void Start()
    {
        cam = Camera.main;
    }
    private void FixedUpdate()
    {
        Vector2 mousePosition;
        mousePosition = Input.mousePosition;
        Vector3 viewport = new Vector3(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        RaycastHit hitInfo;
        if(Physics.Raycast(cam.ViewportPointToRay(viewport), out hitInfo, 10, whatIsText))
        {
            if(hoveringText == null || hoveringText.gameObject != hitInfo.collider.gameObject)
            {
                UISFXManager.instance.Play("Tick");
            }
            if (hoveringText != null && hoveringText.gameObject != hitInfo.collider.gameObject)
            {
                
                hoveringText.color = defaultTextColour;
            }
            hoveringText = hitInfo.collider.GetComponent<TextMesh>();
            hoveringText.color = hoverTextColour;


        } else if (hoveringText != null)
        {
            hoveringText.color = defaultTextColour;
            hoveringText = null;
        }
        
    }

    private void Update()
    {
        if (hoveringText == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            UISFXManager.instance.Play("Selection");
            switch (hoveringText.text)
            {
                case "덱 수정":
                    SceneManager.LoadScene("Deck Maker");
                    
                    break;
                case "플레이":
                    doorAnimator.SetTrigger("DoorATrigger");
                    enteringTheDoorCamera.enabled = true;
                    StartCoroutine("StartPlayingRoutine");
                    enterThePlayCurtain.SetActive(true);
                    UISFXManager.instance.Play("Door Open");
                    break;
                case "설정":
                    SceneManager.LoadScene("Options");
                    break;
            }
        }
    }
    IEnumerator StartPlayingRoutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Battleground");
    }
}
