using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    GameManager_GM GM;

    public int handindex = 0;
    public Sprite[] handcard;

    public int money = 10000;
    public int hand = 0;
    // Start is called before the first frame update
    public void StartHand()
    {
        SetCard();
        SetCard();
        SetCard();
    }
    
    void SetCard()
    {
        //뒷면으로 설정
        handcard[handindex] = GM.cards[0];
        handindex++;
    }
}
