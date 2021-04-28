using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] handcard;

    //CardInfo : 진짜 카드 sprite정보에 접근하기 위한 index.
    public int[] CardInfo = new int[3]; //->int[,] index = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } }; player별로 쪼갠거

    public int money = 10000;
    public bool isDead = false;
    public int deadturn = 0;

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
