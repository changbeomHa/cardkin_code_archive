using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUDManager : MonoBehaviour {
    public static HUDManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<HUDManager>();
            }
            return _instance;
        }
    }
    private static HUDManager _instance;
    public Canvas canvas;
    [Header("Pre-configurations")]
    public Player player;
    public Image crosshairBackground;
    public Image crosshairCurrent;
    public Image healthbarBackground;
    public Image healthbarDelta;
    public Image healthbarCurrent;
    public Image healthbarBox;
    public Text attackName;
    public Outline attackNameOutline;
    public Color attackNameDisabled;
    public Color attackNameEnabled;
    public GameObject durationIndicatorPrefab;
    public RectTransform durationIndicatorPosition;
    public GameObject hand;
    public RectTransform handRectTransform;
    public Image dimBackdrop;
    public GameObject aimPoint;
    public Renderer aimPointRenderer;

    // Use this for initialization
    [Header("Settings")]
    public float healthbarDeltaSpeed = 0.15f;
	
	// Update is called once per frame
	void Update () {
        canvas.enabled = player != null;

        if(player != null)
        {
            if(player.magic.selectedCard != null && player.magic.isAiming)
            {
                aimPoint.transform.parent = null;
                aimPoint.transform.position = player.magic.selectedCard.aimPointPosition;
                aimPoint.SetActive(player.magic.selectedCard.aimPointType != Card.AimPointType.None);
                aimPoint.transform.localScale = new Vector3(1, 1, 1) * player.magic.selectedCard.aimPointRadius * 2f;
                if (player.magic.selectedCard.isDisabled)
                {
                    aimPointRenderer.material.color = new Color(2, 0, 0, .5f);
                }
                else
                {
                    aimPointRenderer.material.color = new Color(0, 1, 0, .5f);
                }
            } else
            {
                aimPoint.SetActive(false);
            }

            crosshairCurrent.enabled = player.magic.isAiming && !player.magic.isHandOpen;
            crosshairBackground.enabled = !player.magic.isHandOpen;
            attackName.color = player.magic.isAiming ? attackNameEnabled : attackNameDisabled;
            attackName.enabled = !player.magic.isHandOpen;
            attackNameOutline.enabled = player.magic.isAiming;
            float fill = player.health / player.maxHealth;
            healthbarCurrent.fillAmount = fill;
            healthbarDelta.fillAmount = Mathf.MoveTowards(healthbarDelta.fillAmount, fill, healthbarDeltaSpeed * Time.deltaTime);
            if(player.magic.selectedCard != null && !player.magic.selectedCard.isDisabled)
            {
                Card selected = player.magic.selectedCard;
                crosshairCurrent.fillAmount = 1f - selected.currentCooldown/selected.cooldownTime;
                attackName.text = player.magic.selectedCard.ToString();
            }
            else
            {
                crosshairCurrent.fillAmount = 0f;
                attackName.text = "";
            }

            

            if (player.magic.isHandOpen)
            {
                
                dimBackdrop.enabled = true;

            }
            else
            {
                
                dimBackdrop.enabled = false;
            }




        }

	}

    public void CreateDurationIndicator(float duration)
    {
        Instantiate(durationIndicatorPrefab, durationIndicatorPosition).GetComponent<DurationIndicator>().duration = duration;
    }
}
