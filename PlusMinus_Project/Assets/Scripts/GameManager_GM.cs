using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_GM : MonoBehaviour
{

    public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [ total 참여 가능한 플레이어의  수 , total 받을 수 있는 카드의 수 ]  
    public int curPlayer = 5;               // 현재 방에 참여한 인원.
    public Sprite[] cards;                  // cards[0] = 카드 뒷면.

    //****playerscript로 이동

    public PlayerScript[] players;
    private float checkCount = 0;

    private bool isDistribute = false;
    private bool isCheckCard = false;

    public int set_turnTime = 10;                // 베팅 , 카드 배치 , 카드 확인 시간 설정.
    public float inGameTime = 0;                      // 게임 진행시간.



    //*****베팅 변수***********//
    public Button FirstBtn;
    public Button DoubleBtn;
    public Button QuarterBtn;
    public Button HalfBtn;
    public Button DieBtn;
    public Button CheckBtn;
    public Button CallBtn;

    int turn = 0;
    int chapter = 1;
    
    int startturn;
    private int totalmoney = 0;
    List<int> MoneyLog = new List<int>();
    int Min = 10;
    int startmoney = 20; //인당


    // Start is called before the first frame update
    void Start()
    {
      
        DistributeCard(curPlayer);

        StartCoroutine("ClickCard", 2);

        BettingStart();

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
        inGameTime += Time.deltaTime;

        
        if(turn/curPlayer == chapter)
        {
            chapter++;
        }

        if (isCheckCard)
        {
            StopCheckCard();
        }

        if(!isCheckCard && inGameTime >= set_turnTime)              // 카드 자동확인 함수.
        {
            AutoCheck(2);
        }
    }

    /// <summary>
    /// 게임 시작시 모든 인원에게서 초기 베팅비를 가져옴.
    /// </summary>
    void BettingStart()
    {
        startturn = Random.Range(0, curPlayer - 1); // 시작지점 랜덤 지정 (구현 예정)

        for (int i = 0; i < curPlayer; i++)
        {
            players[i].money -= startmoney;
            totalmoney += startmoney;
        }
    }
    /// <summary>
    /// 게임 시작시 참가한 인원 만큼 카드를 나눠줌.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 게임 인원 </param>
    void DistributeCard(int in_player)
    {
        RandomCardIndex(in_player);
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
    /// 일정 시간동안 카드를 확인 안할시 플레이어 카드 3장중 center 카드 자동 확인.
    /// CheckCard 코루틴 함수 중단.
    /// </summary>
    /// <param name="select"> 선택 가능한 플레이어 카드 , 인덱스로 전달받음(player01~05 중에서 player03이 디폴트) </param>
    void AutoCheck(int select)
    {
        players[select].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[players[select].CardInfo[1]];
        isCheckCard = true;
        StopCheckCard();
    }

    /// <summary>
    /// 카드 한장만 터치 할 수 있음.
    /// </summary>
    /// <param name="myplayer"> 본인 카드 배열 입력 ( player01 ~ 05 중에서 )  </param>
    /// <param name="select"> 선택 가능한 플레이어 카드 , 인덱스로 전달받음(player01~05 중에서 player03이 디폴트)  </param>
    /// <returns></returns>
    IEnumerator ClickCard(int select )
    {
        Vector3[] myCardPos = new Vector3[3];
        myCardPos[0] = players[select].handcard[0].transform.position; //left
        myCardPos[1] = players[select].handcard[1].transform.position; //center
        myCardPos[2] = players[select].handcard[2].transform.position;//right
        
        int[] playerCard = new int[3];

         // 파라미터로 입력받은 게임오브젝트 배열이 player01~05 중에서 어떤 것인지 확인 후 맞는 플레이어 카드 인덱스 번호 넣어줌.
        if(myCardPos[0] == players[0].transform.position)
        {
            for(int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[0,i];                 
            }
        }
        else if(myCardPos[0] == players[0].transform.position)
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[1, i];
            }
        }
        else if(myCardPos[0] == players[0].transform.position)
        {
            for (int i = 0; i < 3; i++)
            {
                playerCard[i] = arrPlayer[2, i];
            }
        }
        else if (myCardPos[0] == players[0].transform.position)
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





    /// <summary>
    /// 여기서 인덱스를 입력받아 각 플레이어에게 인덱스에 맞춰서 카드를 결정해줌.
    /// 셔플 알고리즘 사용.
    /// </summary>
    /// <param name="in_player"> 현재 참가한 게임 인원.</param> 
    /// <returns></returns>
    void RandomCardIndex(int in_player)
    {
        int rand01, rand02;
        int[] array_temp = new int[cards.Length];
        int variable_temp;

        int count = 0; 
        for(int i = 0; i < cards.Length; i++)
        {
            array_temp[i] = count;
            count++;
        }

        for(int i = 0; i<in_player*3;i++ )
        {
            rand01 = Random.Range(1, cards.Length);
            rand02 = Random.Range(1, cards.Length);

            variable_temp = array_temp[rand01];
            array_temp[rand01] = array_temp[rand02];
            array_temp[rand02] = variable_temp;

        }

        count = 0;
        for (int i = 0; i < in_player; i++)
        {

            for(int j = 0; j < 3; j++)
            {
                arrPlayer[i, j] = array_temp[count];
                count++;
            }
        }

        for(int i = 0; i < players.Length; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                players[i].SetPlayerCard(cards, j, 0);

            }

        }

        isDistribute = true;
    }
    




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
        int a = turn % curPlayer - 1;
        if (a == -1)
        {
            a = curPlayer - 1;
        }

        if (players[a].isDead)
        {
            MoneyLog.Add(0);
            turn++;
        }

        Debug.Log("턴 : " + turn + "/ 누구 턴인지 : " + turn % curPlayer + " /몫: " + turn / curPlayer + "/ 페이즈: " + chapter);
        players[turn % curPlayer].isDead = true;
        players[turn % curPlayer].deadturn = turn;
        players[turn % curPlayer].gameObject.SetActive(false);
        turn++;

        ////머니 로그는 다른 함수들에서 실행.
    }

    private void OnClickHalfBtn()
    {

        Debug.Log("턴 : " + turn + "/ 누구 턴인지 : " + turn % curPlayer + " /몫: " + turn / curPlayer + "/ 페이즈: " + chapter);
        //내 옆 플에이어가 죽었는지 안죽었는지 확인
        int a = turn % curPlayer - 1;
        if (a == -1)
        {
            a = curPlayer - 1;
        }

        if (players[a].isDead)
        {
            if (players[a].deadturn / curPlayer < chapter)
            {//같은 턴에서 죽은 사람
                MoneyLog.Add(0);
            }
            else if (players[a].deadturn / curPlayer > chapter)
            {
                //이미 죽어있는 상태면 그 유저 무시
                MoneyLog.Add(MoneyLog[turn - 1]);
                turn++;
            }
        }
        Debug.Log(turn);
        if (turn > 0)
        {
            players[turn % curPlayer].money -= MoneyLog[turn - 1]; //전 사람이 낸만큼 먼저 냄
            totalmoney += MoneyLog[turn - 1];
            int half = totalmoney / 2;
            players[turn % curPlayer].money -= half; //그 후 전체 금액의 1/4
            totalmoney += half;

            MoneyLog.Add(MoneyLog[turn - 1] + half);
            turn++;

        }

        else
        {
            Debug.Log("선택 불가");
        }

        //    Debug.Log("턴: " + turn + "총 금액 : " + totalmoney + "전 사람 금액 : " + MoneyLog[turn - 2]);
    }

    private void OnClickQuaterBtn()
    {

        Debug.Log("턴 : " + turn + "/ 누구 턴인지 : " + turn % curPlayer + " /몫: " + turn / curPlayer + "/ 페이즈: " + chapter);
        int a = turn % curPlayer - 1;
        if (a == -1)
        {
            a = curPlayer - 1;
        }
        
        if (players[a].isDead)
        {
            if (players[a].deadturn / curPlayer < chapter)
            {//같은 턴에서 죽은 사람
                MoneyLog.Add(0);
            }
            else if (players[a].deadturn / curPlayer > chapter)
            {
                //이미 죽어있는 상태면 그 유저 무시
                MoneyLog.Add(MoneyLog[turn - 1]);
                turn++;
            }
        }

        Debug.Log(turn);
        if (turn > 0)
        {
            players[turn % curPlayer].money -= MoneyLog[turn - 1]; //전 사람이 낸만큼 먼저 냄
            totalmoney += MoneyLog[turn - 1];

            int quarter = totalmoney / 4;//그 후 전체 금액의 1/4
            players[turn % curPlayer].money -= quarter;
            totalmoney += quarter;

            MoneyLog.Add(MoneyLog[turn - 1] + quarter);
            turn++;
        }

        else
        {
            Debug.Log("선택 불가");
        }

        //Debug.Log("턴: " + turn + "총 금액 : " + totalmoney + "전 사람 금액 : " + MoneyLog[turn - 2]);

    }

    private void OnClickDoubleBtn()
    {

        Debug.Log("턴 : " + turn + "/ 누구 턴인지 : " + turn % curPlayer + " /몫: " + turn / curPlayer + "/ 페이즈: " + chapter);

        int a = turn % curPlayer - 1;
        if (a == -1)
        {
            a = curPlayer - 1;
        }

        if (players[a].isDead)
        {
            if (players[a].deadturn / curPlayer < chapter)
            {//같은 턴에서 죽은 사람
                MoneyLog.Add(0);
            }
            else if (players[a].deadturn / curPlayer > chapter)
            {
                //이미 죽어있는 상태면 그 유저 무시
                MoneyLog.Add(MoneyLog[turn - 1]);
                turn++;
            }
        }
        if (turn > 0)
        {
            players[turn % curPlayer].money -= 2 * MoneyLog[turn - 1];
            MoneyLog.Add(2 * MoneyLog[turn - 1]);
            totalmoney += MoneyLog[turn];
            turn++;
        }
        else
        {
            Debug.Log("선택 불가");
        }
        //Debug.Log("턴: " + turn + "총 금액 : " + totalmoney + "전 사람 금액 : " + MoneyLog[turn - 2]);

    }

    private void OnClickFirstBtn()
    {

        Debug.Log("턴 : " + turn + "/ 누구 턴인지 : " + turn % curPlayer+ " /몫: "+turn/curPlayer+"/ 페이즈: "+chapter);

        if (turn % curPlayer == 0)
        {
            MoneyLog.Add(Min);
            players[turn % curPlayer].money -= MoneyLog[turn];
            totalmoney += MoneyLog[turn];
            turn++;
            
        }
        else
        {
            Debug.Log("선택할 수 없습니다.");
        }
        //Debug.Log("턴: " + turn + " 총 금액 : " + totalmoney + " 전 사람 금액 : 없음");
    }

}
    