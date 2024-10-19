using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
public abstract class Card : MonoBehaviour, IPointerClickHandler {

    private Minigame currentMinigameInstance;


    [Header("Card Settings")]
    public bool isDisabled = false;
    public float cooldownTime = 0.25f;
    public AimPointType aimPointType = AimPointType.None;
    public float aimPointDistance = 30f;
    public float aimPointRadius = 1f;
    public GameObject minigamePrefab;
    public enum AimPointType
    {
        None, MidAir, OnGround
    }
    [Header("Attributes")]

    public Player owner;
    public bool isUnlocked = false;
    private bool wasUnlocked = false;

    protected Text cardTitleText;
    protected Text cardDescriptionText;
    protected Text cardAAChargeText;
    protected Text cardNameText;
    protected GameObject unlockedIndicator;
    protected GameObject lockedIndicator;
    protected GameObject selectedIndicator;
    protected Canvas canvas;
    protected int AAChargeGranted = 0;
    public float currentCooldown = 0f;
    public Vector3 aimPointPosition
    {
        get
        {
            switch (aimPointType)
            {
                case AimPointType.None:
                    return owner.magic.GetAirAimPoint(aimPointDistance);
                case AimPointType.MidAir:
                    return owner.magic.GetAirAimPoint(aimPointDistance);
                case AimPointType.OnGround:
                    return owner.magic.GetGroundAimPoint(aimPointDistance);
                   
            }
            return owner.magic.GetAirAimPoint(aimPointDistance);
        }
    }


    void Awake () {
        // Following code looks fucking ridiculous but it's for the best, trust me.
        
        try
        {
            unlockedIndicator = transform.Find("Unlocked Indicator").gameObject;
            lockedIndicator = transform.Find("Locked Indicator").gameObject;
            
        }
        catch { }

        try
        {
            cardTitleText = transform.Find("Card Title").gameObject.GetComponent<Text>();
        }
        catch { }

        try
        {
            cardNameText = transform.Find("Card Name").GetComponent<Text>();
        }
        catch { }
        
        canvas = transform.parent.parent.gameObject.GetComponent<Canvas>();
        selectedIndicator = transform.Find("Selected Indicator").gameObject;

        try
        {
            cardAAChargeText = transform.Find("Card AA Charge").GetComponent<Text>();
            AAChargeGranted = int.Parse(cardAAChargeText.text);
        }
        catch
        {
            AAChargeGranted = 0;
        }
        
	}

    private void LateUpdate()
    {
        if(owner == null)
        {
            owner = HUDManager.instance.player;
            return;
        }
        selectedIndicator.SetActive(owner.magic.selectedCard == this);
        if(cardNameText != null)
        {
            cardNameText.text = ToString();
        }
        
        if (currentCooldown > 0)
        {
            currentCooldown = Mathf.MoveTowards(currentCooldown, 0f, Time.deltaTime);

        }

        if (isUnlocked && !wasUnlocked)
        {
            wasUnlocked = true;
            if (unlockedIndicator != null)
            {
                UISFXManager.instance.Play("Unlock");
                unlockedIndicator.SetActive(true);
            }
            if (lockedIndicator != null)
            {
                lockedIndicator.SetActive(false);
            }

        }
        else if (!isUnlocked && wasUnlocked)
        {
            wasUnlocked = false;
            if (unlockedIndicator != null)
            {
                unlockedIndicator.SetActive(false);
            }
            if (lockedIndicator != null)
            {
                lockedIndicator.SetActive(true);
            }

        }

        if(currentMinigameInstance != null)
        {
            currentMinigameInstance.canvas.enabled = owner.magic.isHandOpen;
            currentMinigameInstance.remainingTime = Mathf.MoveTowards(currentMinigameInstance.remainingTime, 0, Time.deltaTime);
            currentMinigameInstance.remainingTimeText.text = ((int)currentMinigameInstance.remainingTime).ToString();
            if(currentMinigameInstance.remainingTime == 0)
            {
                currentMinigameInstance.remainingTime = 180;
                currentMinigameInstance.Fail();
            }
        }
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            owner.magic.selectedCard = this;
            UISFXManager.instance.Play("Slide Card");
        }
        else
        {
            if(minigamePrefab != null)
            {
                InstantiateMinigame(minigamePrefab);
                UISFXManager.instance.Play("Selection");
            }
            else
            {
                Unlock();
            }
            
        }
    }
    public void Unlock()
    {
        isUnlocked = true;
        owner.magic.autoattackCharge += AAChargeGranted;
    }

    protected void InstantiateMinigame(GameObject original)
    {
        if (currentMinigameInstance != null) return;

        Minigame minigame = Instantiate(original).GetComponent<Minigame>();
        minigame.SetCard(this);
        currentMinigameInstance = minigame;

    }

    public abstract void Use();
    public abstract override string ToString();
}
