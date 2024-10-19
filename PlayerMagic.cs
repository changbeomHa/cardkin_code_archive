using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerMagic : NetworkBehaviour {

    private NetworkManager networkManager;

    public Transform magicPivot;
    public int autoattackCharge;
    public List<GameObject> deck;
    public Card selectedCard;
    public List<GameObject> modelsToHideWhenAiming;

    public bool isHandOpen = false;
    [Header("Internal")]
    public GameObject testCard;
    public GameObject autoattackCard;
    public bool isAiming;
    
    private Player player;
    private Animator animator;
    private GameObject playerModel;
    private int lockUnlockCycle = 0;
    void Start() {
        networkManager = FindObjectOfType<NetworkManager>();
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        playerModel = transform.Find("Magician").gameObject;
        if (isLocalPlayer)
        {
            AddCardToHand(autoattackCard);
            StartCoroutine(DrawCardRoutine());
            StartCoroutine(AutoattackChargeRoutine());
        }
        

    }

    IEnumerator AutoattackChargeRoutine()
    {
        while (!player.isDead)
        {
            if(autoattackCharge < 5)
            {
                autoattackCharge++;
            }
            yield return new WaitForSeconds(1.3f);
        }
    }

    IEnumerator DrawCardRoutine()
    {
        while (!player.isDead)
        {
            if((HUDManager.instance.hand.transform.childCount < 6) && ((GameManager.instance != null)))
            {
                DrawCard();
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    void Update() {
        if (!isLocalPlayer) { return; }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isHandOpen = !isHandOpen;
        }
        if (selectedCard != null && !selectedCard.isUnlocked)
        {
            selectedCard = null;
        }

        if (Input.GetButton("Fire1") && !(Input.GetKey(KeyCode.LeftShift)) && Input.GetButton("Fire2") && !isHandOpen)
        {
            if (selectedCard != null && selectedCard.currentCooldown == 0 && !selectedCard.isDisabled && selectedCard.isUnlocked)
            {
                selectedCard.currentCooldown = selectedCard.cooldownTime;
                
                selectedCard.Use();
                animator.SetTrigger("Attack");
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (selectedCard == null)
            {
                foreach (Transform t in HUDManager.instance.hand.transform)
                {
                    Card card = t.GetComponent<Card>();
                    if (card.isUnlocked)
                    {
                        selectedCard = card;
                    }

                }
            }
        }

        if (!isAiming && selectedCard != null)
        {
            selectedCard.currentCooldown = 0.1f;
        }
        if (Input.GetButton("Fire2") && !player.magic.isHandOpen && !Input.GetKey(KeyCode.LeftShift))
        {
            player.magic.isAiming = true;
        }
        else
        {
            player.magic.isAiming = false;
        }

        foreach(GameObject model in modelsToHideWhenAiming)
        {
            model.SetActive(!isAiming);
        }

        animator.SetBool("Aim", player.magic.isAiming);
        
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ScrollThroughCards(1);
            UISFXManager.instance.Play("Slide Card");
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ScrollThroughCards(-1);
            UISFXManager.instance.Play("Slide Card");
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(0).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(1).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(2).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(3).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(4).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            try
            {
                UISFXManager.instance.Play("Slide Card");
                Card card = HUDManager.instance.hand.transform.GetChild(5).GetComponent<Card>();
                if (card.isUnlocked) selectedCard = card;
            }
            catch { }
        }


    }

    public void ScrollThroughCards(int scrollAmount)
    {
        int current;
        for (int i = 0; i < HUDManager.instance.hand.transform.childCount; i++)
        {
            if (HUDManager.instance.hand.transform.GetChild(i) == selectedCard.transform)
            {
                current = i;
                i += scrollAmount;
                if (i > HUDManager.instance.hand.transform.childCount - 1)
                {
                    i = 0;
                }
                if (i < 0)
                {
                    i = HUDManager.instance.hand.transform.childCount - 1;
                }
                while (current != i && !HUDManager.instance.hand.transform.GetChild(i).GetComponent<Card>().isUnlocked)
                {
                    i += scrollAmount;
                    if (i > HUDManager.instance.hand.transform.childCount - 1)
                    {
                        i = 0;
                    }
                    if (i < 0)
                    {
                        i = HUDManager.instance.hand.transform.childCount - 1;
                    }
                }
                selectedCard = HUDManager.instance.hand.transform.GetChild(i).GetComponent<Card>();
                break;
            }

        }
    }

    public void DrawCard()
    {
        AddCardToHand(deck[Random.Range(0, deck.Count)]);
        UISFXManager.instance.Play("Draw Card");
    }

    public void AddCardToHand(GameObject cardPrefab)
    {
        lockUnlockCycle++;
        Card card = Instantiate(cardPrefab, HUDManager.instance.hand.transform).GetComponent<Card>();

        card.owner = player;
        if (lockUnlockCycle == 3)
        {
            card.Unlock();
            lockUnlockCycle = 0;
        }


    }

    public Vector3 GetAirAimPoint(float maxDistance)
    {
        return player.cameraRig.GetAirAimPoint(maxDistance);

    }
    public Vector3 GetGroundAimPoint(float maxDistance)
    {
        return player.cameraRig.GetGroundAimPoint(maxDistance);
    }



    public void SpawnMagic(string magicName, Vector3 position, Quaternion rotation)
    {
        CmdSpawnMagic(magicName, 8f, position, rotation);
    }

    public void SpawnMagic(string magicName, float lifetime, Vector3 position, Quaternion rotation)
    {
        CmdSpawnMagic(magicName, lifetime, position, rotation);
    }

    public void SpawnMagicAtPlayer(string magicName, float lifetime)
    {
        CmdSpawnMagic(magicName, lifetime, magicPivot.position, magicPivot.rotation);
    }

    public void SpawnMagicAtPlayer(string magicName)
    {
        CmdSpawnMagic(magicName, 8f, magicPivot.position, magicPivot.rotation);
        
    }





    [Command]
    public void CmdSpawnMagic(string magicName, float lifetime, Vector3 position, Quaternion rotation)
    {

        Magic newMagic = Instantiate(Resources.Load<GameObject>(magicName), position, rotation).GetComponent<Magic>();
        newMagic.owner = player;
        newMagic.birthTime = Time.time;
        newMagic.lifetime = lifetime;
        Collider magicCollider = newMagic.GetComponent<Collider>();
        if (magicCollider != null)
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), magicCollider);
        }

        NetworkServer.Spawn(newMagic.gameObject);
    }




}
