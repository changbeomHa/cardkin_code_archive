using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public float cameraShake;
    public bool isDead;

    public float movementSpeedModifier = 1f;
    public bool isStunned = false;
    public Shader unityDiffuseShader;

    [SyncVar]
    public float health = 30f;
    protected float lastHealth = 0f;
    public float maxHealth = 30f;

    protected int stunCount = 0;
    public PlayerMagic magic;
    public PlayerMovement movement;
    public PlayerCameraRig cameraRig;
    protected GameObject dizzy;
    public Transform magician;
    protected GameObject deathEffect;

    private void Start()
    {
        magician = transform.Find("Magician");
        dizzy = transform.Find("Dizzy").gameObject;
        deathEffect = transform.Find("Death Effect").gameObject;
        deathEffect.SetActive(false);
        if (isLocalPlayer)
        {
            HUDManager.instance.player = this;
            magic = GetComponent<PlayerMagic>();
            movement = GetComponent<PlayerMovement>();


            magic.enabled = true;
            movement.enabled = true;
            cameraRig.enabled = true;
            for(int i = 0; i < magician.transform.childCount; i++)
            {
                Renderer renderer = magician.transform.GetChild(i).GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = unityDiffuseShader;
                }
                
            }
            
        }
        else
        {
            magic = GetComponent<PlayerMagic>();
            magic.enabled = true;
            cameraRig.gameObject.SetActive(false);
        }
        lastHealth = health;

    }

    [ClientRpc]
    public void RpcApplySlow(float amount, float duration)
    {
        StartCoroutine(Slow(amount, duration));
        if (HUDManager.instance.player == this)
        {
            HUDManager.instance.CreateDurationIndicator(duration);
        }
        GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere * .5f, "느려짐!", Color.white);
    }

    [ClientRpc]
    public void RpcApplyStun(float duration)
    {
        StartCoroutine(Stun(duration));
        if (HUDManager.instance.player == this)
        {
            HUDManager.instance.CreateDurationIndicator(duration);
        }
        GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere * .5f, "기절!", Color.white);
    }

    [ClientRpc]
    public void RpcApplyCameraShake(float amount)
    {
        if (cameraShake < amount)
        {
            cameraShake = amount;
        }
    }


    public void ApplySlow(float amount, float duration)
    {
        RpcApplySlow(amount, duration);
    }

    public void ApplyStun(float duration)
    {
        RpcApplyStun(duration);
    }
    
    public void ApplyCameraShake(float amount)
    {
        RpcApplyCameraShake(amount);
    }

    IEnumerator Slow(float amount, float duration)
    {
        movementSpeedModifier *= (1 - amount);
        yield return new WaitForSeconds(duration);
        movementSpeedModifier /= (1 - amount);
    }

    IEnumerator Stun(float duration)
    {
        stunCount++;
        isStunned = true;
        yield return new WaitForSeconds(duration);
        stunCount--;
        if (stunCount == 0)
        {
            isStunned = false;
        }
    }


    private void FixedUpdate()
    {
        Vector3 fixedPos = magician.position - transform.position;
        fixedPos.x = 0;
        fixedPos.z = 0;
        magician.position = fixedPos + transform.position;
        dizzy.SetActive(isStunned);
        dizzy.transform.Rotate(new Vector3(0, 60 * Time.deltaTime, 0));
        if (stunCount <= 0)
        {
            stunCount = 0;
            isStunned = false;
        }
        else
        {
            isStunned = true;
        }

        if (cameraShake >= 0)
        {
            cameraShake *= 0.94f;
        }
        else if (cameraShake < 0.2)
        {
            cameraShake = 0;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (isDead)
        {
            deathEffect.SetActive(true);
        }

        if (lastHealth > health)
        {
            int healthDelta = (int)Mathf.Round(lastHealth - health);
            if(healthDelta != 0)
            {
                GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere*.5f, healthDelta.ToString("#,#"), Color.red);
                
            }
            lastHealth = health;
        }
        if (lastHealth < health)
        {
            int healthDelta = (int)Mathf.Round(health - lastHealth);
            if (healthDelta != 0)
            {
                GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere*.5f, healthDelta.ToString("#,#"), Color.green);
            }
            lastHealth = health;
        }

        if ((health <= 0) && !isDead)
        {

            isDead = true;
            GetComponent<CharacterController>().enabled = false;
            magic.enabled = false;
            movement.enabled = false;
            GameManager.instance.onPlayerDeath(this);
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<CapsuleCollider>().height = 1.55f;
            rb.AddForce(Random.insideUnitSphere * 200 + transform.up * 400);
            rb.AddTorque(Random.insideUnitSphere.normalized * Random.Range(70f,100f));
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            GetComponentInChildren<PlayerCameraRig>().gameObject.transform.parent = null;
        }
    }
    

}
