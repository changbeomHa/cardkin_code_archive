using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLayoutManager : MonoBehaviour {
    public Transform handStart;
    public Transform handEnd;
    public Transform autoattack;
    private Vector3 handStartPoint;
    private Vector3 handEndPoint;
    public float startRotation;
    public float endRoation;
    private Canvas canvas;
    private RectTransform rt;
    private Vector3[] cv = new Vector3[10];
    private void Start()
    {

        canvas = transform.parent.GetComponent<Canvas>();
        rt = transform.GetComponent<RectTransform>();
        handStart.gameObject.SetActive(false);
        handEnd.gameObject.SetActive(false);
        autoattack.gameObject.SetActive(false);
    }

    void Update () {
        if (HUDManager.instance.player == null) { return; }
        int childCount = transform.childCount;
        if (childCount == 0) return;
        Vector3 handStartPosition = handStart.position;
        Vector3 handEndPosition = handEnd.position;


        RectTransform cardRt;
        Vector3 desiredPosition;

        transform.GetChild(0).transform.position = autoattack.position;

        for (int i = 1; i < childCount; i++)
        {

            cardRt = transform.GetChild(i).GetComponent<RectTransform>();

            if (HUDManager.instance.player.magic.isHandOpen)
            {
                cardRt.rotation = Quaternion.identity;
                if (childCount == 2)
                {
                    desiredPosition = Vector3.zero;

                }
                else
                {
                    desiredPosition = (Vector3.left + (Vector3.right-Vector3.left)*(i-1)/(childCount-2)) * Screen.width / 24;
                }

                if (HUDManager.instance.player.magic.selectedCard != null && HUDManager.instance.player.magic.selectedCard.transform == transform.GetChild(i).transform)
                {
                    desiredPosition += Vector3.up * rt.rect.height * 0.01f;
                }
                else
                {
                    desiredPosition += Vector3.down * rt.rect.height * 0.025f;
                }


                cardRt.position = Vector3.SmoothDamp(cardRt.position, desiredPosition, ref cv[i], 0.04f);

            }
            else
            {
                if (childCount == 2)
                {
                    desiredPosition = handEndPosition;

                    cardRt.rotation = Quaternion.Euler(0, 0, endRoation);
                }
                else
                {
                    desiredPosition = handStartPosition + (handEndPosition - handStartPosition) * (6-childCount + i-1) / (6 - 2);
                    cardRt.rotation = Quaternion.Euler(0, 0, startRotation + (endRoation - startRotation) * (6 - childCount + i - 1) / (6 - 2));
                }

                if (HUDManager.instance.player.magic.selectedCard!=null&& HUDManager.instance.player.magic.selectedCard.transform == transform.GetChild(i).transform)
                {
                    desiredPosition += Vector3.up * rt.rect.height * 0.05f;
                }

                cardRt.position = Vector3.SmoothDamp(cardRt.position, desiredPosition, ref cv[i], 0.07f);
            }

            

        }
	}
}
