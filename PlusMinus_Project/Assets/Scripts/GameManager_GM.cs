using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_GM : MonoBehaviour
{

    public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [ total 참여 가능한 플레이어의  수 , total 받을 수 있는 카드의 수 ]  
    int[] cardValue = { -1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 1, 1, 1, 1, 11, 11, 11, 11, 13, 13, 13, 13, 12, 12, 12, 12 };
    public int curPlayer = 5;               // 현재 방에 참여한 인원.
    public Sprite[] cards;                  // cards[0] = 카드 뒷면.

    //****playerscript로 이동

    public PlayerScript[] players;
    private float checkCount = 0;



    private bool isCheckCard = false;
    private bool isDragDrop = false;
    private bool isChangeCard = false;                  // 카드 가운데 + , - 로 바뀜.


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


    //*********** 텍스트 변수  단계별 표시 ***********//
    public Text stepText;


    // Start is called before the first frame update
    void Start()
    {
        int[] testplayer = { 0, 1, 2, 3, 4 };
        int[,] testcards = { { 0, 1, 2 }, { 3, 4, 5 } };
      
        DistributeCard(curPlayer,4,testplayer,testcards);
        

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

        DisplayStepByStep();

        if (turn/curPlayer == chapter)
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


    void DisplayStepByStep()
    {
        if(inGameTime < set_turnTime)
        {
            stepText.text = "카드를 확인 \n 해주세요 ";
        }
        else if(inGameTime < set_turnTime * 2)
        {
            stepText.text = "카드를 배치 \n 해주세요";
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
    /// <param name="in_player"> 현재 참여한 총 게임 인원 </param>
    /// <param name="myNumber"> 자기 자신의 번호 (서버에서 받아올 플레이어 정보랑 비교하기 위한것) </param>
    /// <param name="playerNumber"> 서버에서 넘어오는 플레이어들의 고유 번호 (본인것도 포함) </param>
    /// <param name="playerCards"> 서버에서 넘어오는 플레이어들의 카드 정보 (본인것도 포함) </param>
    void DistributeCard(int in_player,int myNumber,int[] playerNumber , int[,] playerCards)
    {
        int playerIndex = 2;                // 본인의 고유정보와 , 서버에서 전달된 본인의 정보 인덱스


        RandomCardIndex(in_player);

        for(int i = 0; i < in_player; i++)
        {
            if(myNumber == playerNumber[i])
            {
                playerIndex = i;
            }
        }


        switch (playerIndex)
        {
            case 0:
                for(int i = 0; i < in_player; i++)
                {
                    if (i < 3)
                    {
                        players[i+2].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 0]];
                        players[i+2].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 1]];
                        players[i+2].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 2]];
                    }
                    else
                    {
                        players[i - 3].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 0]];
                        players[i - 3].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 1]];
                        players[i - 3].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 2]];
                    }
                }
                break;

            case 1:
                for(int i = 0; i < in_player; i++)
                {
                    if( i < 4)
                    {
                        players[i + 1].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 0]];
                        players[i + 1].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 1]];
                        players[i + 1].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }
                    else
                    {
                        players[i - 4].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 0]];
                        players[i - 4].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 1]];
                        players[i - 4].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }
                }
                break;

            case 2:
                for (int i = 0; i < in_player; i++)
                {
                    players[i].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 0]];
                    players[i].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 1]];
                    players[i].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 2]];

                }
                break;

            case 3:
                for (int i = 0; i < in_player; i++)
                {
                    if( i > 0)
                    {
                        players[i - 1].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i, 0]];
                        players[i - 1].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 1]];
                        players[i - 1].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }
                    else
                    {
                        players[i + 4].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 0]];
                        players[i + 4].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 1]];
                        players[i + 4].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }

                }
                break;
            case 4:
                for (int i = 0; i < in_player; i++)
                {
                    if (i < 2)
                    {
                        players[i + 3].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 0]];
                        players[i + 3].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 1]];
                        players[i + 3].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }
                    else
                    {
                        players[i - 2].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 0]];
                        players[i - 2].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i  , 1]];
                        players[i - 2].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[i , 2]];
                    }

                }
                break;
            default:
                break;



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
    /// 일정 시간동안 카드를 확인 안할시 플레이어 카드 3장중 center 카드 자동 확인.
    /// CheckCard 코루틴 함수 중단.
    /// </summary>
    /// <param name="select"> 선택 가능한 플레이어 카드 , 인덱스로 전달받음(player01~05 중에서 player03이 디폴트) </param>
    void AutoCheck(int select)
    {
        players[select].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[select,1]];
        isCheckCard = true;
        StopCheckCard();
    }
    

    /// <summary>
    /// 카드 한장만 터치 할 수 있음.
    /// </summary>
    /// <param name="select"> 선택 가능한 플레이어 카드 , 인덱스로 전달받음(player01~05 중에서 player03이 디폴트(index = 2))  </param>
    /// <returns></returns>
    IEnumerator ClickCard(int select )
    {
        Vector3[] myCardPos = new Vector3[3];
        myCardPos[0] = players[select].handcard[0].transform.position; //left
        myCardPos[1] = players[select].handcard[1].transform.position; //center
        myCardPos[2] = players[select].handcard[2].transform.position;//right
        

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
                            players[select].handcard[i].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[select,i]];
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
    /// <param name="in_player"> 현재 참가한 게임 총 인원.</param> 
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

        count = 1;
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
        Debug.Log("card 01 : " + arrPlayer[0, 0] + ", " + arrPlayer[0, 1] + ", " + arrPlayer[0, 2]);
        Debug.Log("card 02 : " + arrPlayer[1, 0] + ", " + arrPlayer[1, 1] + ", " + arrPlayer[1, 2]);
        Debug.Log("card 03 : " + arrPlayer[2, 0] + ", " + arrPlayer[2, 1] + ", " + arrPlayer[2, 2]);
        Debug.Log("card 04 : " + arrPlayer[3, 0] + ", " + arrPlayer[3, 1] + ", " + arrPlayer[3, 2]);
        Debug.Log("card 05 : " + arrPlayer[4, 0] + ", " + arrPlayer[4, 1] + ", " + arrPlayer[4, 2]);

        
    }
    

    // 플레


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
    