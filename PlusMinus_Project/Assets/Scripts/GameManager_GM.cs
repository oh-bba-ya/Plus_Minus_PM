using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_GM : MonoBehaviour
{
    public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [ total 참여 가능한 플레이어의  수 , total 받을 수 있는 카드의 수 ]  
    public int curPlayer = 5;
    public Sprite[] cards;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("arrPlayer : " + arrPlayer[0, 0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// 1. 현재 게임내에 존재하는 플레이어만큼 배열을 할당해주는 함수.
    /// 2. 인덱스를 입력받아 플레이어에게 카드 인덱스에 따른 카드를 할당해줌.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 플레이어의 숫자를 입력 받음.</param> 
    /// <param name="index"> GameManager_GM 스크립트에 저장된 카드 스프라이트 인덱스를 입력받음.</param> 
    void Playerallocation(int in_player,int[,] index)
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (i < in_player)
                {
                    arrPlayer[i, j] = index[i,j];
                }
            }
        }
    }

    int[,] RandomCardIndex()
    {
        int[,] index = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };

        return index;
    }

}
