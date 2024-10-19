using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeAppear : MonoBehaviour {

    private Image image;
    private Color originalColour;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalColour = image.color;
        StartCoroutine("AppearRoutine");
    }

    IEnumerator AppearRoutine()
    {
        for(float t = 0; t < 1; t += Time.deltaTime)
        {
            Color newColour = originalColour;
            newColour.a *= t;
            image.color = newColour;
            yield return null;
        }
        image.color = originalColour;
    }
}
