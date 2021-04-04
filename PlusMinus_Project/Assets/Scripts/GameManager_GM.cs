using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_GM : MonoBehaviour
{
    public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [ total 참여 가능한 플레이어의  수 , total 받을 수 있는 카드의 수 ]  
    public int curPlayer = 5;
    public Sprite[] cards;                  // cards[0] = 카드 뒷면.


    public GameObject[] player01 = new GameObject[3];
    public GameObject[] player02 = new GameObject[3];
    public GameObject[] player03 = new GameObject[3];
    public GameObject[] player04 = new GameObject[3];
    public GameObject[] player05 = new GameObject[3];

    private int checkCount = 0;

    private bool isDistribute = false;
    private bool isCheckCard = false;





    // Start is called before the first frame update
    void Start()
    {
        DistributeCard(curPlayer);
        StartCoroutine("ClickCard", player03);
        

    }

    // Update is called once per frame
    void Update()
    {
    }


    /// <summary>
    /// 게임 시작시 참가한 인원 만큼 카드를 나눠줌.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 게임 인원 </param>
    void DistributeCard(int in_player)
    {

        for(int i = 0; i < in_player; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                switch (i)
                {
                    case 0:
                        player01[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player01[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player01[2].GetComponent<SpriteRenderer>().sprite = cards[0];
                        break;
                    case 1:
                        player02[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player02[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player02[2].GetComponent<SpriteRenderer>().sprite = cards[0];
                        break;
                    case 2:
                        player03[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player03[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player03[2].GetComponent<SpriteRenderer>().sprite = cards[0];
                        break;
                    case 3:
                        player04[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player04[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player04[2].GetComponent<SpriteRenderer>().sprite = cards[0];
                        break;
                    case 4:
                        player05[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player05[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                        player05[2].GetComponent<SpriteRenderer>().sprite = cards[0];
                        break;

                }


            }
        }



        isDistribute = true;
        if (isDistribute)
        {
            Playerallocation(curPlayer, RandomCardIndex(curPlayer));
        }

    }

    /// <summary>
    /// 카드 확인 ClickCard() 함수 코루틴 시작. 
    /// </summary>
    void StartCheckCard()
    {
        StartCoroutine("ClickCard",player03);
    }

    /// <summary>
    /// 카드 확인 ClickCard() 함수 코루틴 종료
    /// </summary>
    void StopCheckCard()
    {
        StopCoroutine("ClickCard");
    }



    /// <summary>
    /// 카드 한장만 터치 할 수 있음.
    /// </summary>
    /// <param name="myplayer"> 본인 카드 배열 입력 ( player01 ~ 05 중에서 )  </param>
    /// <returns></returns>
    IEnumerator ClickCard(GameObject[] myplayer )
    {
        Vector3[] myCardPos = new Vector3[3];
        int[] playerCard = new int[3];
        myCardPos[0] = myplayer[0].transform.position;          // leftCard.
        myCardPos[1] = myplayer[1].transform.position;          // centerCard.
        myCardPos[2] = myplayer[2].transform.position;          // rightCard.


        // 파라미터로 입력받은 게임오브젝트 배열이 player01~05 중에서 어떤 것인지 확인 후 맞는 플레이어 카드 인덱스 번호 넣어줌.
        if(myCardPos[0] == player01[0].transform.position)
        {
            for(int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[0,i];                 
            }
        }
        else if(myCardPos[0] == player02[0].transform.position)
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[1, i];
            }
        }
        else if(myCardPos[0] == player03[0].transform.position)
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[2, i];
            }
        }
        else if (myCardPos[0] == player04[0].transform.position)
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[3, i];
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[4, i];
            }
        }

        while (checkCount < 1)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInpo = Physics2D.Raycast(touchPos, Camera.main.transform.forward);
                if(hitInpo.collider != null && (hitInpo.transform.position == myCardPos[0] || hitInpo.transform.position == myCardPos[1] || hitInpo.transform.position == myCardPos[2]))
                {
                    Vector3 touchObj = hitInpo.transform.position; // 터치한 카드 오브젝트 위치.
                    if (touchObj == myCardPos[0])
                    {
                        myplayer[0].GetComponent<SpriteRenderer>().sprite = cards[playerCard[0]];
                        isCheckCard = true;
                        checkCount = 1;
                    }
                    else if (touchObj == myCardPos[1])
                    {
                        myplayer[1].GetComponent<SpriteRenderer>().sprite = cards[playerCard[1]];
                        isCheckCard = true;
                        checkCount = 1;
                    }
                    else if (touchObj == myCardPos[2])
                    {
                        myplayer[2].GetComponent<SpriteRenderer>().sprite = cards[playerCard[2]];
                        isCheckCard = true;
                        checkCount = 1;
                    }

                }


            }
            yield return new WaitForSeconds(0.05f);             // While 루프 내부를 0.05초마다 실행.
        }

        
    }



    /// <summary>
    /// 플레이어 인원수 만큼 RandomCardIndex를 통해 반한된 2차원 배열을 플레이어 에게 저장함.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 플레이어의 숫자를 입력 받음.</param> 
    /// <param name="index"> GameManager_GM 스크립트에 저장된 카드 스프라이트 인덱스를 RandomCardIndex 함수로 부터 return 한 index를 입력받음.</param> 
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



    /// <summary>
    /// 스프라이트 카드 랜덤 인덱스 생성 ( 중복 제거)
    /// 여기서 인덱스를 입력받아 각 플레이어에게 인덱스에 맞춰서 카드를 결정해줌.
    /// </summary>
    /// <param name="in_player"> 현재 참가한 게임 인원.</param> 
    /// <returns></returns>
    int[,] RandomCardIndex(int in_player)
    {
        List<int> rand = new List<int>();
        int randnumber = Random.Range(1,53);
        int[,] index = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };

        for(int i=0; i<in_player * 3;)
        {
            if (rand.Contains(randnumber))
            {
                randnumber = Random.Range(1, 53);
            }
            else
            {
                rand.Add(randnumber);
                i++;
            }
        }

        int count = 0;
        for (int i = 0; i < in_player; i++)
        {
            
            for(int j = 0; j < 3; j++)
            {
                index[i, j] = rand[count];
                count++;
            }
        }
        return index;
    }




}
