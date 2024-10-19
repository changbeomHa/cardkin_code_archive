using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Minigame : MonoBehaviour {
    public float remainingTime = 10f;

    private Canvas _canvas;

    protected Card card;
    public Canvas canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            return _canvas;
        }
    }
    public Text remainingTimeText
    {
        get
        {
            if(_remainingTimeText == null)
            {
                _remainingTimeText = transform.Find("이 아래로 미니게임 메커닉을 짜시면 됩니다/Remaining Time").GetComponent<Text>();
            }
            return _remainingTimeText;
        }
    }
    private Text _remainingTimeText;
    public abstract void Fail();
    public void SetCard(Card originCard)
    {
        card = originCard;
    }

    public void DestroyMinigameAndCard()
    {
        Destroy(gameObject);
        Destroy(card.gameObject);
        card.owner.magic.isHandOpen = false;
    }

}
