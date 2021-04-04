using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] handcard;

    public int money = 10000;
    public int hand = 0;
    // Start is called before the first frame update


    /*
     * para : cards; 카드 스프라이트 리스트/ pcard; player가 가지고 있는 카드 인덱스/ deck: 카드 리스트에서 설정하고 싶은 인덱스
     */
    /// <summary>
    /// 함수 실행 시, 플레이어의 카드가 설정됨.
    /// </summary>
    public void SetPlayerCard(Sprite[] cards, int pcard, int deck)
    {
        //뒷면으로 설정
        handcard[pcard].GetComponent<SpriteRenderer>().sprite = cards[deck];
    }
}
