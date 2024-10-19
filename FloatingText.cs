using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {

    public string text;
    public float t = 0;
    private Vector3 velocity = Vector3.zero;
    private TextMesh textMesh;
    private Color originalColour = Color.clear;

    private void OnValidate()
    {
        if(textMesh == null)
        {
            textMesh = GetComponent<TextMesh>();
        }
        textMesh.text = text;
    }

    void Start () {
        textMesh = GetComponent<TextMesh>();
	}
	

	void Update () {
        t += Time.deltaTime;
        textMesh.text = text;
        if(originalColour == Color.clear)
        {
            originalColour = textMesh.color;
        }

        Color colour = textMesh.color;



        if (velocity == Vector3.zero)
        {
            velocity = Random.insideUnitSphere * 2f + Vector3.up * 4f;
        }
        velocity += Vector3.down * 9 * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        colour.a = originalColour.a * (2 - t);
        if (t > 2)
        {
            Destroy(gameObject);
        }


        textMesh.color = colour;
        if(Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        

    }
}
