using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Magic : NetworkBehaviour {
    public Player owner;
    public float lifetime;
    public float birthTime;

    private void LateUpdate()
    {
       if(Time.time - birthTime >= lifetime)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        if (isServer)
        {
            RpcDestroySelf();
        }
    }




    [ClientRpc]
    public void RpcDestroySelf()
    {
        ParticleSystem[] psOfChild = GetComponentsInChildren<ParticleSystem>();
        Light[] lOfChild = GetComponentsInChildren<Light>();
        foreach (ParticleSystem ps in psOfChild)
        {
            ps.Stop();
            
            ps.transform.parent = null;
            Destroy(ps.gameObject, 3f);

        }

        foreach(Light l in lOfChild)
        {
            l.enabled = false;
            l.transform.parent = null;
            Destroy(l.gameObject, 3f);
        }

        Destroy(gameObject);

    }
}
