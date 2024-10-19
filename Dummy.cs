using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Dummy : Player
{



    private void Start()
    {
        magician = transform.Find("Magician");
        dizzy = transform.Find("Dizzy").gameObject;
        deathEffect = transform.Find("Death Effect").gameObject;
        deathEffect.SetActive(false);
        lastHealth = health;

    }

    private void FixedUpdate()
    {

        dizzy.SetActive(isStunned);
        dizzy.transform.Rotate(new Vector3(0, 60*Time.deltaTime, 0));
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


        if (lastHealth > health)
        {
            int healthDelta = (int)(lastHealth - health);
            if (healthDelta != 0)
            {
                GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere * .5f, healthDelta.ToString("#,#"), Color.red);

            }
            lastHealth = health;
        }
        if (lastHealth < health)
        {
            int healthDelta = (int)(health - lastHealth);
            if (healthDelta != 0)
            {
                GameManager.instance.ClientSpawnFloatingText(transform.position + Random.insideUnitSphere * .5f, healthDelta.ToString("#,#"), Color.green);
            }
            lastHealth = health;
        }

        if ((health <= 0) && !isDead)
        {
            isDead = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * 200 + transform.up * 400);
            rb.AddTorque(Random.insideUnitSphere.normalized * Random.Range(70f, 100f));
            deathEffect.SetActive(true);
            deathEffect.transform.parent = null;
            Destroy(deathEffect, 10);
            Destroy(gameObject);
        }
    }


}
