using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour {
    public GameObject prefabToSpawn;
    public float interval;
    private float lastSpawnedTime;

    private void Start()
    {
        lastSpawnedTime = Time.time;
    }
    private void Update()
    {
        if(Time.time - lastSpawnedTime >= interval)
        {
            Instantiate(prefabToSpawn, transform.position, transform.rotation);
            lastSpawnedTime += interval;
        }
    }


}
