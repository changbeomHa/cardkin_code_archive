using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyText : MonoBehaviour {

    private Vector3 originalPosition;
    public float timeMultiplier;
    public float magnitudeMultiplier;
    public float timeOffset;
    private void Awake()
    {
        originalPosition = transform.position;
    }

    void Update () {
        transform.position = originalPosition + Mathf.Sin(Time.time * timeMultiplier + timeOffset) * magnitudeMultiplier * Vector3.up;
	}
}
