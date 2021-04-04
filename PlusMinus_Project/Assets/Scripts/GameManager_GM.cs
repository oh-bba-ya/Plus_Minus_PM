using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_GM : MonoBehaviour
{
    
    //public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [ total 참여 가능한 플레이어의  수 , total 받을 수 있는 카드의 수 ]  
    public int curPlayer = 5;
    public Sprite[] cards;                  // cards[0] = 카드 뒷면.

    /*
    public GameObject[] player01 = new GameObject[3];
    public GameObject[] player02 = new GameObject[3];
    public GameObject[] player03 = new GameObject[3];
    public GameObject[] player04 = new GameObject[3];
    public GameObject[] player05 = new GameObject[3];
    */

    public PlayerScript[] players;
    int CardInfoLoop = 1;
    private int checkCount = 0;

    private bool isDistribute = false;
    private bool isCheckCard = false;


    //*****베팅 변수***********//
    public Button FirstBtn;
    public Button DoubleBtn;
    public Button QuarterBtn;
    public Button HalfBtn;
    public Button DieBtn;
    public Button CheckBtn;
    public Button CallBtn;

    int turn = 0;
    
    int startturn;
    private int totalmoney = 0;
    List<int> MoneyLog = new List<int>();
    int Min = 10;
    int startmoney = 20; //인당


    // Start is called before the first frame update
    void Start()
    {
        Shuffle();
        DistributeCard(curPlayer);
        StartCoroutine("ClickCard", 2);

        startturn = Random.Range(0, curPlayer-1); // 시작지점 랜덤 지정 (구현 예정)

        for(int i = 0; i < curPlayer; i++)
        {
            players[i].money -= startmoney;
            totalmoney += startmoney;
        }
        //클릭시 함수 호출하는 이벤트 트리거
        FirstBtn.onClick.AddListener(() => OnClickFirstBtn());
        DoubleBtn.onClick.AddListener(() => OnClickDoubleBtn());
        QuarterBtn.onClick.AddListener(() => OnClickQuaterBtn());
        HalfBtn.onClick.AddListener(() => OnClickHalfBtn());
        DieBtn.onClick.AddListener(() => OnClickDieBtn());
        CheckBtn.onClick.AddListener(() => OnClickCheckBtn());
        CallBtn.onClick.AddListener(() => OnClickCallBtn());

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
            //cardback으로 먼저 설정하기.
            players[i].SetPlayerCard(cards, 0, 0);
            players[i].SetPlayerCard(cards, 1, 0);
            players[i].SetPlayerCard(cards, 2, 0);
        }

        
        isDistribute = true;
        if (isDistribute)
        {
            PlayerCardAllocation();
            //Playerallocation(curPlayer, RandomCardIndex(curPlayer));
        }
        
    }

    /// <summary>
    /// 카드 확인 ClickCard() 함수 코루틴 시작. 
    /// </summary>
    void StartCheckCard()
    {
        StartCoroutine("ClickCard",2);
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
    IEnumerator ClickCard(int select )
    {
        Vector3[] myCardPos = new Vector3[3];
        myCardPos[0] = players[select].handcard[0].transform.position; //left
        myCardPos[1] = players[select].handcard[1].transform.position; //center
        myCardPos[2] = players[select].handcard[2].transform.position;//right

        int[] playerCard = new int[3];

        for(int i = 0; i < 3; i++)
        {
            playerCard[i] = players[select].CardInfo[i];
        }
        /*
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
        */
    while (checkCount < 1)
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInpo = Physics2D.Raycast(touchPos, Camera.main.transform.forward);
            if(hitInpo.collider != null && (hitInpo.transform.position == myCardPos[0] || hitInpo.transform.position == myCardPos[1] || hitInpo.transform.position == myCardPos[2]))
            {
                Vector3 touchObj = hitInpo.transform.position; // 터치한 카드 오브젝트 위치.
                for (int i = 0; i < 3; i++)
                {
                    if (touchObj == myCardPos[i])
                    {
                        players[select].handcard[i].GetComponent<SpriteRenderer>().sprite = cards[playerCard[i]];
                        isCheckCard = true;
                        checkCount = 1;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.05f);             // While 루프 내부를 0.05초마다 실행.
    }
}




    void Shuffle()
    {
        for (int i = cards.Length - 1; i > 1; --i)
        {
            int j = Random.Range(1, cards.Length - 1);
            Sprite temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    void PlayerCardAllocation()
    {
        for(int i = 0; i < curPlayer; i++)
        {
            players[i].CardInfo[0] = CardInfoLoop;
            CardInfoLoop++;
            players[i].CardInfo[1] = CardInfoLoop;
            CardInfoLoop++;
            players[i].CardInfo[2] = CardInfoLoop;
            CardInfoLoop++;
            Debug.Log("Player"+i+1+": "+cards[players[i].CardInfo[0]].name + ", " + cards[players[i].CardInfo[1]].name + ", " + cards[players[i].CardInfo[2]].name);
        }
    }

    /*
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
    */




    private void OnClickCallBtn()
    {
        throw new System.NotImplementedException();
    }

    private void OnClickCheckBtn()
    {
        throw new System.NotImplementedException();
    }

    private void OnClickDieBtn()
    {
        throw new System.NotImplementedException();
    }

    private void OnClickHalfBtn()
    {
        throw new System.NotImplementedException();
    }

    private void OnClickQuaterBtn()
    {
        throw new System.NotImplementedException();

    }

    private void OnClickDoubleBtn()
    {
        int previous = turn - 1;
        if (turn >= 1)
        {
            players[(previous + 1) % curPlayer].money -= 2 * MoneyLog[previous];
            MoneyLog.Add(2 * MoneyLog[previous]);
            totalmoney += MoneyLog[turn];
            turn++;
        }
        else
        {
            Debug.Log("선택 불가");
        }

        Debug.Log("턴: " + turn + "총 금액 : " + totalmoney + "전 사람 금액 : " + MoneyLog[previous]);
    }

    private void OnClickFirstBtn()
    {
        if(turn == 0)
        {
            MoneyLog.Add(Min);
            players[0].money -= MoneyLog[turn];
            totalmoney += MoneyLog[turn];
            turn++;
        }
        else
        {
            Debug.Log("선택할 수 없습니다.");
        }
        Debug.Log("턴: "+turn + "총 금액 : " + totalmoney+"전 사람 금액 : 없음");
    }

}
    