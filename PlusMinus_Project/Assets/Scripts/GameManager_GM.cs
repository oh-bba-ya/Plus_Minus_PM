using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_GM : MonoBehaviour
{
    //@함수 수정
    //RandomCardIndex 함수 수정 -> 서버에서 배정된 카드를 출력하는 역할

    public int[,] arrPlayer = { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } };            // [[player01~05 , cards index ]  
    public int[] resultPlayer = { -1, -1, -1, -1, -1 };       // 플레이어들의 결과 합산 저장 변수. 

    /// <summary>
    /// cardValue의 경우 cards가 스프라이트 배열이므로 각 인덱스에 해당하는 카드 숫자(ex : cards[5] = 하트 7 일 경우  cardValue[5] = 7)를 저장하는 배열.
    /// 조커 = 0 ,
    /// </summary>
    public int[] cardValue = { -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0, 0 };

    public int[] expressionValue = { -2, -3, -4 };          // -2 = plus , -3 = minus , -4 = mult .
    public int curPlayer = 0;               // 현재 방에 참여한 인원.
    public Sprite[] cards;                  // cards[0] = 카드 뒷면.
    public Sprite[] expressions;           // +, = , * 스프라이트 저장.

    /// <summary>
    /// player03 그룹 오브젝트 변수.
    /// </summary>
    public GameObject[] originPos = new GameObject[3];
    /// <summary>
    /// player03 자식 오브젝트들 위치 저장.
    /// </summary>
    Vector3[] startPos = new Vector3[3];

    //****playerscript로 이동

    public PlayerScript[] players;
    private float checkCount = 0;

    public int myPortIndex;                    // 자신의 번호를 [0,1,2,3,4] 중 인덱스로 입력 , 카드 드래그 드랍때 정보를 바꿔주기 위해 쓸거임. , CardDrag에서도 쓰이니 꼭!! , Distribute 에서 입력받는다.

    /// <summary>
    /// bool isCheckCard , DragDrop , ChangeCard 들은 각 행동이 끝나면 true 로 전환.
    /// </summary>
    public bool isCheckCard = false;                    // 소지한 3장의 카드 중 한장의 카드 확인
    public bool isDragDrop = false;                     // 카드 드래그 드랍을 통해 3장의 카드 배치.
    public bool isChangeCard = false;                  // 카드 가운데 + , - 로 바뀜.
    public bool isLeftCard = false;                    // 왼쪽 카드 공개 여부.
    public bool isRightCard = false;                    // 오른쪽 카드 공개 여부.
    public bool isFirstBet = false;                      // 가운데 카드가 공개된 후 시작되는 베팅    [[ 첫번째 베팅]
    public bool isFirstBetEnd = false;                   // 첫번째 베팅 종료.
    public bool isLastBet = false;                       // 왼쪽 카드가 공개 된후 시작되는 베팅 [두번째 베팅.]
    public bool isLastBetEnd = false;                   // 두번째 베팅 종료
    public bool isDisplay = false;                      // 오른쪽 카드 공개 후 Loser, Winner 배너 공개.
    public bool isGameOver = false;                     // 게임 종료 , 다시 시작 버튼 생성

    public int set_turnTime = 10;                //  카드 배치 , 카드 확인 시간 설정.
    public float inGameTime = 0;                      // 게임 진행시간.

    /// <summary>
    /// 베팅 버튼 변수들.
    /// </summary>
    public Button CallBtn;
    public Button DoubleBtn;
    public Button HalfBtn;
    public Button DieBtn;

    //*********** 텍스트 변수  단계별 표시 ***********//
    public Text stepText;
    public Text TotalText;
    public Text betMoneyText;

    //************* 베팅 관련 변수. *****************//
    public int playerBetTime = 10;              // 플레이어 베팅 시간.
    int btnIndex;                               // 
    bool ismyTurn;                        // 나의 베팅 턴 , true가 될때마다 내 차례임.
    public int totalMoney = 0;
    public int startmoney = 20;         // 처음 시작시 판돈 금액
    public int betMoney = 0;
    int my_PreBetMoney = 0;             // 나의 이전 베팅 금액. [ 해당하는 베팅 턴 ex) 가운데 카드 공개후 시작한 베팅턴에서만 ] 
    int playerMoney;                    // 본인 플레이어[playerprefabs] 돈 가져와서 저장.

    int[] testplayer = { 0, 1, 2, 3, 4 };

    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        ServerManager.instance.EmitGameReady();
        myPortIndex = ServerManager.instance.yourTurn;

        ismyTurn = false;

        betMoney = startmoney;

        Load();           // 현재 참여한 플레이어 배열 여기다가 돈 저장해서 빼고 더하고 할거임.

        for (int i = 0; i < startPos.Length; i++)
        {
            startPos[i] = originPos[i].transform.position;
        }

        //int[] testplayer = { 0, 1, 2, 3, 4 };
        //int[,] testcards = { { 0, 1, 2 }, { 3, 4, 5 } };

        DistributeCard(curPlayer, myPortIndex, arrPlayer, "first");

        BaseBettingStart();

        Save(myPortIndex);
    }

    // Update is called once per frame
    void Update()
    {
        inGameTime += Time.deltaTime;
        DisplayStepByStep();            // 판돈 금액 표시 , 현재 베팅 금액 표시 , 단계별 안내문 표시.

        if (!isGameOver)
        {
            if (inGameTime <= set_turnTime)             // 설정된 시간 전까지는 카드 한장 확인과 , 3장의 카드 Drag&Drop 가능. 베팅 버튼도 비활성화.
            {
                OffBetting();
                if (!isCheckCard)
                {
                    Debug.Log(inGameTime);
                    slider.value -= inGameTime*0.001f;
                    StartCoroutine(ClickCard(myPortIndex));
                }
                else
                {
                    slider.value = 1f;
                    StopCheckCard();
                }
            }
            else if (inGameTime > set_turnTime)      // 설정된 시간 이후 3장의 카드 Drag&Drop 불가능
            {
                OFFCardDrag();
                if (!isCheckCard)        // 설정된 시간 이후에도 카드 확인이 완료되지 않았다면 가운데 카드 자동 확인.
                {
                    AutoCheck(myPortIndex);
                }
                else if (ServerManager.instance.endData)    // 모든 유저들이 카드를 바꾼 정보가 서버에서 취합완료됬으므로 가운데 카드 공개되자마자 + / - / * 중 하나로 바뀜.
                {
                    arrPlayer = ServerManager.instance.arrPlayer;
                    DistributeCard(curPlayer, myPortIndex, arrPlayer, "second");            // 가운데 카드를 공개했으므로 첫번째 베팅 시작.


                    if (isChangeCard && !isLeftCard && !isFirstBet && !isFirstBetEnd)                // 가운데 카드가 공개된후. 첫 베팅 시작전.
                    {
                        OnFirstBet();       // 첫번째 베팅 시작. [ isFirstBet = false 로 변경됨]
                    }
                    else if (isFirstBet && !isFirstBetEnd)    // 첫번째 베팅이 시작했다면.
                    {
                        if (ismyTurn)           // 베팅중 나의 턴일때만 버튼 함수 실행시킴.
                        {
                            OnBetting();        // 버튼 함수 활성화.
                        }
                    }
                    else if (isFirstBetEnd)     // 첫번째 베팅 종료 후. 왼쪽 카드 공개 후 마지막 베팅 시작.
                    {
                        DistributeCard(curPlayer, myPortIndex, arrPlayer, "third");
                        OnLastBet();
                        if (isLeftCard && !isLastBet && !isLastBetEnd)
                        {
                            if (ismyTurn)           // 베팅중 나의 턴일때만 버튼 함수 실행시킴.
                            {
                                OnBetting();        // 버튼 함수 활성화.
                            }
                        }
                        else if (isLastBetEnd)      // 마지막 베팅 종료시 오른쪽 카드 공개. 플레이어 카드 합산 결과 함수 실행.
                        {
                            ResultNumber(curPlayer);      // 모든 플레이어 카드 합산 결과 구함.
                            DistributeCard(curPlayer, myPortIndex, arrPlayer, "last");          // step = "last" 일때 isDisplay = true로 바뀌므로 플레이어 상황에 따라 Loser , Winner 배너 공개해야함.
                        }
                    }
                }
            }
        }
        else
        { // 게임 종료 후, 다시 시작 버튼을 누를시 실행되야함.

        }


















        /*  임시로 가끔 서버없이 클라이언트만 확인해야할때 있어서 주석 처리해놓음
        DisplayStepByStep();

        if (isBetting)
        {
            OffBetting();
        }
        else if (!isBetting)
        {
            OnBetting();
        }

        if (!isCheckCard && inGameTime < set_turnTime)
        {
            StartCoroutine(ClickCard(2, myPortIndex));
        }

        if (!isCheckCard && inGameTime >= set_turnTime)              // 카드 자동확인 함수 시작.
        {
            AutoCheck(2, myPortIndex);
            OFFCardDrag();
        }


        if (isCheckCard)
        {
            StopCheckCard();
            int[] tmpArray = new int[3];
            for(int i=0; i<3; i++)
            {
                tmpArray[i] = arrPlayer[myPortIndex, i];
            }
            ServerManager.instance.EmitDeicision(myPortIndex, tmpArray);
        }

        if (isCheckCard && inGameTime >= set_turnTime)           // 카드 확인도 하고 , 시간초과일때 드래그 스왑 종료.
        {
            OFFCardDrag();
        }

        if (ServerManager.instance.endData)
        {
            DistributeCard(curPlayer, myPortIndex, arrPlayer, "second");
        }

        if (isDragDrop && isChangeCard)
        {
            BackPosition();
            DistributeCard(curPlayer, myPortIndex, arrPlayer, "third");
            DistributeCard(curPlayer, myPortIndex, arrPlayer, "last");
            ResultNumber(curPlayer, myPortIndex, testplayer, arrPlayer);
        }

        if (turn)                   // 전체 플레이어 베팅이 시작되면
        {
            if (myTurn)             // 전체 플레이어중 나의 베팅 턴
            {
                OnBetting();
            }
            else
            {
                OffBetting();
            }
        }
        else                        // 전체 플레이어 베팅 턴이 종료되면. betMoney도 초기화.
        {
            OffBetting();
            betMoney = startmoney;
            Save(myPortIndex);
        }
        */

    }



    /// <summary>
    /// 서버에서 관리할 마이턴 차례.
    /// </summary>
    /// <param name="_myTurn"></param>
    public void setMyTurn(bool _myTurn)
    {
        ismyTurn = _myTurn;
    }

    public void setFirstBet(bool _OffFirstBet)
    {
        isFirstBetEnd = _OffFirstBet;
    }

    public void setLastBet(bool _OffLastBet)
    {
        isLastBetEnd = _OffLastBet;
    }



    void DisplayStepByStep()
    {
        if (inGameTime < set_turnTime && !isCheckCard)
        {
            stepText.text = "카드를 확인 \n 해주세요";
        }

        if (isCheckCard)
        {
            stepText.text = "카드를 배치 \n 해주세요";
        }

        if (ismyTurn && !isFirstBet)
        {
            stepText.text = playerBetTime.ToString();
        }

        if (ismyTurn && !isLastBet)
        {
            stepText.text = playerBetTime.ToString();
        }

        // 현재 전체 표시
        TotalText.text = totalMoney.ToString();

        // 전 플레이어가 제시한 베팅 머니 표현
        betMoneyText.text = betMoney.ToString();

        //if (!isBetting && isChangeCard)
        //{
        //    stepText.text = "베팅 시작";
        //}

    }

    /// <summary>
    /// 첫번째 베팅 시작. , isFirstBet = true로 변경됨.
    /// </summary>
    void OnFirstBet()
    {
        isFirstBet = true;
    }


    /// <summary>
    /// 첫번째 베팅 종료.
    /// </summary>
    void OffFirstBet()
    {
        isFirstBet = true;
        isLeftCard = true;
        my_PreBetMoney = 0;                 // 첫번째 베팅 끝나고 이전에 걸었던 판돈들 초기화.
        OffBetting();
    }

    void OnLastBet()
    {
        isLastBet = false;
    }

    /// <summary>
    /// 마지막 베팅 종료.
    /// </summary>
    void OffLastBet()
    {
        isLastBet = true;
        OffBetting();
    }

    /// <summary>
    /// 베팅 버튼 OFF;
    /// </summary>
    void OffBetting()
    {
        CallBtn.GetComponent<Button>().interactable = false;
        HalfBtn.GetComponent<Button>().interactable = false;
        DoubleBtn.GetComponent<Button>().interactable = false;
        DieBtn.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 베팅 버튼 ON;
    /// </summary>
    void OnBetting()
    {
        CallBtn.GetComponent<Button>().interactable = true;
        HalfBtn.GetComponent<Button>().interactable = true;
        DoubleBtn.GetComponent<Button>().interactable = true;
        DieBtn.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// 카드 드래그 스왑 종료 함수.
    /// </summary>
    void OFFCardDrag()
    {
        isDragDrop = true;
    }

    /// <summary>
    /// 내 인덱스에 대한 player정보를 저장 
    /// </summary>
    /// <param name="myindex"></param>
    void Save(int myindex)
    {

        PlayerPrefs.SetInt("Money", players[myindex].money);
    }

    /// <summary>
    /// Save된 각 플레이어 돈을 playerMoney 배열에 불러옴.
    /// </summary>
    /// <param name="myindex"></param>
    void Load()
    {
        PlayerPrefs.GetInt("Money", playerMoney);
    }


    /// <summary>
    /// 게임 시작시 모든 인원에게서 초기 베팅비를 가져옴.
    /// </summary>
    void BaseBettingStart()
    {
        for (int i = 0; i < curPlayer; i++)
        {
            players[i].money -= startmoney;
            totalMoney += startmoney;
        }
    }
    /// <summary>
    /// 게임 시작시 참가한 인원 만큼 카드를 나눠줌.\n
    /// first = 시작하자마자 카드 뒷면 스프라이트만 나눠줌 \n
    /// second = 가운데 카드 공개되자마자 , + / - / * 부호 스프라이트로 바뀜\n
    /// third =  왼쪽카드 스프라이트 공개\n
    /// last = 오른쪽 카드 스프라이트 공개.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 총 게임 인원 </param>
    /// <param name="myPlayerIndex"> 자기 자신의 인덱스 </param>
    /// <param name="playerCards"> 서버에서 넘어오는 플레이어들의 카드 정보 (본인것도 포함) /* 파라미터 입력대로 만들지 않고 전역변수 appPlayer를 이용해서 만들었기 때문에 만약 서버에서 카드를 나눠주는것이 온다면 코드 수정해야함. */ </param>   확인요망!!
    /// <param name="step"> "first"  ,"second "  ,"third" , "last" . </param>
    void DistributeCard(int in_player, int myPlayerIndex, int[,] playerCards, string step)
    {
        // 카드가 잘나눠지는지 확인하기 위해 앞면 카드를 배정해줬지만 발표가 끝난 후에는 카드 뒷면을 할당해줄거임.
        // 스프라이트 앞면에 해당하는 인덱스를 각 플레이어에게 할당해줄거임.
        // 뒷면 카드 확인 하기위해 mynumber = 4일때 가렸음.

        //Debug.Log("Distri : " + in_player);

        //int playerIndex = 2;                // 본인의 고유정보와 , 서버에서 전달된 본인의 정보 인덱스

        //for (int i = 0; i < in_player; i++)
        //{
        //    if (myNumber == playerNumber[i])
        //    {
        //        playerIndex = i;
        //        myPortIndex = i;
        //    }
        //}

        if (step == "first")             // start 에서 시작됨. 처음 카드를 나눠줌.
        {
            RandomCardIndex(in_player);
            for (int i = 0; i < in_player; i++)
            {
                players[i].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[0];
                players[i].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[0];
                players[i].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[0];
            }


        }
        else if (step == "second")           // 가운데 카드 공개
        {
            Debug.Log("가운데 카드 공개");
            switch (myPlayerIndex)
            {
                case 0:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 3)
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i + 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i + 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i + 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                        else
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i - 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i - 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i - 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 4)
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i + 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i + 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i + 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                        else
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i - 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i - 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i - 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                        {
                            players[i].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                        }
                        else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                        {
                            players[i].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                        }
                        else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                        {
                            players[i].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i > 0)
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i - 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i - 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i - 1].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                        else
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i + 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i + 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i + 4].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 2)
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i + 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i + 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i + 3].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                        else
                        {
                            if (cardValue[arrPlayer[i, 1]] == 0)            // 조커일경우.
                            {
                                players[i - 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[2];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
                            {
                                players[i - 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[0];
                            }
                            else if (cardValue[arrPlayer[i, 1]] % 2 != 0)   // 홀수
                            {
                                players[i - 2].handcard[1].GetComponent<SpriteRenderer>().sprite = expressions[1];

                            }
                        }
                    }
                    break;

            }

            isChangeCard = true;
            //isBetting = false;
        }
        else if (step == "third")               // 왼쪽 카드 공개.
        {
            Debug.Log("왼쪽 카드 공개");
            isLeftCard = true;
            switch (myPlayerIndex)
            {
                case 0:             // player 01. 
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 3)
                        {
                            players[i + 2].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                        else
                        {
                            players[i - 3].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                    }
                    break;

                case 1:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 4)
                        {
                            players[i + 1].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                        else
                        {
                            players[i - 4].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                    }
                    break;

                case 2:
                    for (int i = 0; i < in_player; i++)
                    {
                        players[i].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                    }
                    break;

                case 3:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i > 0)
                        {
                            players[i - 1].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                        else
                        {
                            players[i + 4].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }

                    }
                    break;
                case 4:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 2)
                        {
                            players[i + 3].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }
                        else
                        {
                            players[i - 2].handcard[0].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 0]];
                        }

                    }
                    break;
                default:
                    Debug.Log(" 왼쪽 카드 공개 예외 상황!!!");
                    break;

            }
        }
        else if (step == "last")
        {
            Debug.Log("오른쪽 카드 공개");
            isRightCard = true;
            isDisplay = true;
            switch (myPlayerIndex)
            {
                case 0:             // player 01. 플레이어 본인 카드 외에는 
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 3)
                        {
                            players[i + 2].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                        else
                        {
                            players[i - 3].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                    }
                    break;

                case 1:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 4)
                        {
                            players[i + 1].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                        else
                        {
                            players[i - 4].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                    }
                    break;

                case 2:
                    for (int i = 0; i < in_player; i++)
                    {
                        players[i].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                    }
                    break;

                case 3:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i > 0)
                        {
                            players[i - 1].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                        else
                        {
                            players[i + 4].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }

                    }
                    break;
                case 4:
                    for (int i = 0; i < in_player; i++)
                    {
                        if (i < 2)
                        {
                            players[i + 3].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }
                        else
                        {
                            players[i - 2].handcard[2].GetComponent<SpriteRenderer>().sprite = cards[playerCards[i, 2]];
                        }

                    }
                    break;
                default:
                    Debug.Log("오른쪽 카드 공개 예외 상황!!!");
                    break;

            }
        }
    }

    /// <summary>
    /// 카드 드래그 끝나면 처음 위치로 카드 돌리기.
    /// </summary>
    void BackPosition()
    {
        for (int i = 0; i < originPos.Length; i++)
        {
            originPos[i].transform.position = startPos[i];
        }

    }

    /// <summary>
    /// 카드 확인 ClickCard() 함수 코루틴 종료
    /// </summary>
    void StopCheckCard()
    {
        StopCoroutine(ClickCard(myPortIndex));
        checkCount = 1;
    }

    /// <summary>
    /// 일정 시간동안 카드를 확인 안할시 플레이어 카드 3장중 center 카드 자동 확인.
    /// CheckCard 코루틴 함수 중단.
    /// 만약 카드 확인을 못하게 된다면 확인할 시간도 없이 바로 + , - 로 바뀜 [ 하지만 굳이 보여줄 시간을 따로 줄 필요없음 ]
    /// </summary>
    /// <param name="myNumber"> 서버로 부터 내 번호를 입력 받아서 나의 카드 부여 </param>
    void AutoCheck(int myNumber)
    {
        players[2].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[myNumber, 1]];
        isCheckCard = true;
        StopCheckCard();
    }


    /// <summary>
    /// 카드 한장만 터치 할 수 있음.
    /// </summary>
    /// <param name="select"> 가운데 카드 인덱스02 , 가운데에 존재하는 오브젝트 위치.  </param>
    /// <param name="myNumber">  본인 번호 </param>
    /// <returns></returns>
    IEnumerator ClickCard(int myNumber)
    {
        GameObject[] myCardPos = new GameObject[3];
        // 인스펙터 창 01~05 플레이어중 03 플레이어의 카드 위치들이 본인카드들의 위치이므로 그 카드들만 클릭가능.
        myCardPos[0] = players[2].handcard[0]; //left               
        myCardPos[1] = players[2].handcard[1]; //center
        myCardPos[2] = players[2].handcard[2];//right

        while (checkCount < 1)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInpo = Physics2D.Raycast(touchPos, Camera.main.transform.forward);

                if (hitInpo.collider != null && (hitInpo.transform.gameObject == myCardPos[0] || hitInpo.transform.gameObject == myCardPos[1] || hitInpo.transform.gameObject == myCardPos[2]))
                {
                    GameObject touchObj = hitInpo.transform.gameObject; // 터치한 카드 오브젝트 위치.
                    for (int i = 0; i < 3; i++)
                    {
                        if (touchObj == myCardPos[i])
                        {
                            players[2].handcard[i].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[myNumber, i]];
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
    public void RandomCardIndex(int in_player)
    {
        //int rand01, rand02;
        //int[] array_temp = new int[cards.Length];
        //int variable_temp;

        //int count = 0;
        //for (int i = 0; i < cards.Length; i++)
        //{
        //    array_temp[i] = count;
        //    count++;
        //}


        //for (int i = 0; i < in_player * 3; i++)
        //{
        //    rand01 = Random.Range(1, cards.Length);
        //    rand02 = Random.Range(1, cards.Length);

        //    variable_temp = array_temp[rand01];
        //    array_temp[rand01] = array_temp[rand02];
        //    array_temp[rand02] = variable_temp;

        //}

        //count = 1;
        //for (int i = 0; i < in_player; i++)
        //{

        //    for (int j = 0; j < 3; j++)
        //    {
        //        arrPlayer[i, j] = array_temp[count];
        //        count++;
        //    }
        //}

        arrPlayer = ServerManager.instance.arrPlayer;

        //서버로 코드 옮김
        Debug.Log("card 01 : " + arrPlayer[0, 0] + ", " + arrPlayer[0, 1] + ", " + arrPlayer[0, 2]);
        Debug.Log("card 02 : " + arrPlayer[1, 0] + ", " + arrPlayer[1, 1] + ", " + arrPlayer[1, 2]);
        Debug.Log("card 03 : " + arrPlayer[2, 0] + ", " + arrPlayer[2, 1] + ", " + arrPlayer[2, 2]);
        Debug.Log("card 04 : " + arrPlayer[3, 0] + ", " + arrPlayer[3, 1] + ", " + arrPlayer[3, 2]);
        Debug.Log("card 05 : " + arrPlayer[4, 0] + ", " + arrPlayer[4, 1] + ", " + arrPlayer[4, 2]);
    }

    /// <summary>
    /// 콜 : 베팅만큼 베팅함
    /// </summary>
    public void OnClickCallBtn()
    {
        btnIndex = 0;
        Debug.Log("Call");
        if (my_PreBetMoney == 0)
        {
            totalMoney += betMoney;
            playerMoney -= betMoney;
            players[myPortIndex].money = playerMoney;
            my_PreBetMoney = betMoney;
            ServerManager.instance.EmitBetting(myPortIndex, betMoney, totalMoney);
        }
        else if (betMoney - my_PreBetMoney != 0)
        {
            int restBetMoney = betMoney - my_PreBetMoney;
            totalMoney += restBetMoney;
            playerMoney -= restBetMoney;
            players[myPortIndex].money = playerMoney;
            my_PreBetMoney = betMoney;
            ServerManager.instance.EmitBetting(myPortIndex, restBetMoney, totalMoney);
        }

    }

    /// <summary>
    /// 더블 : 베팅 머니에 2배를 해서 베팅함
    /// </summary>
    public void OnClickDoubleBtn()
    {
        btnIndex = 1;
        Debug.Log("Double");
        my_PreBetMoney = betMoney * 2;
        betMoney = betMoney * 2;
        totalMoney += betMoney;
        playerMoney = playerMoney - betMoney;
        players[myPortIndex].money = playerMoney;
        ServerManager.instance.EmitBetting(myPortIndex, betMoney, totalMoney);
    }

    /// <summary>
    /// 하프 버튼 : 전체 판돈의 절반을 베팅함.
    /// </summary>
    public void OnClickHalfBtn()
    {
        btnIndex = 2;
        Debug.Log("Half");
        betMoney = betMoney + (totalMoney / 2);
        playerMoney = playerMoney - betMoney;
        my_PreBetMoney = betMoney;
        totalMoney += betMoney;
        players[myPortIndex].money = playerMoney;
        ServerManager.instance.EmitBetting(myPortIndex, betMoney, totalMoney);
    }

    /// <summary>
    /// 다이버튼 / 죽으면 뭔가 어떤 기가막힌 스프라이트로 가리고 싶음.
    /// 조커들고 죽을시 기본판돈 만큼 베팅하고 죽음.
    /// </summary>
    public void OnClickDieBtn()
    {
        Debug.Log("Die");
        btnIndex = 3;

        Save(myPortIndex);

        if (cardValue[arrPlayer[myPortIndex, 0]] == 0 || cardValue[arrPlayer[myPortIndex, 1]] == 0 || cardValue[arrPlayer[myPortIndex, 02]] == 0)
        {
            // 소지하고 있는 카드 3장중 조커를 한장이라도 소유하고 있을 때 죽는다면 현재인원의 * 기본 판돈.
            totalMoney = totalMoney + (startmoney * curPlayer);
        }

        for (int i = 0; i < 3; i++)                  // Die 버튼을 누른다면 가지고 있는 3장의 카드 정보가 0으로 바뀜. 이럴경우 카드 뒷면 인덱스를 가지게됨.
        {
            arrPlayer[myPortIndex, i] = 0;
        }

        playerMoney -= startmoney * curPlayer;
        players[myPortIndex].money = playerMoney;

        ServerManager.instance.EmitBetting(myPortIndex, -1, totalMoney);
    }

    /// <summary>
    /// 플레이어 들의 3장의 카드 합산 결과를 저장하는 함수 
    /// 카드를 부여 받고 난 후 플레이어들의 카드 결과는 정해짐.
    /// </summary>
    /// <param name="in_player"> 현재 게임에 참여한 인원 </param>
    void ResultNumber(int in_player)
    {

        int[] resultPlayer = new int[in_player];         // 플레이어들의 합산 결과.



        for (int i = 0; i < in_player; i++)
        {
            // 트리플일 경우. 왼쪽카드 * 2
            if (arrPlayer[i, 0] == arrPlayer[i, 1] && arrPlayer[i, 0] == arrPlayer[i, 2])
            {
                resultPlayer[i] = cardValue[arrPlayer[i, 0]] * 2;

            }
            else
            {
                // 조커를 한장이라도 가지고 있다면.
                if (cardValue[arrPlayer[i, 0]] == 0 || cardValue[arrPlayer[i, 1]] == 0 || cardValue[arrPlayer[i, 0]] == 0)
                {
                    // 조커 카드가 가운데에 존재하는 경우
                    if (cardValue[arrPlayer[i, 1]] == 0)
                    {
                        // 조커 카드가 가운데에 존재하고 왼쪽에도 존재하는 경우
                        if (cardValue[arrPlayer[i, 0]] == 0)
                        {
                            resultPlayer[i] = cardValue[arrPlayer[i, 2]] * cardValue[arrPlayer[i, 2]];
                        }
                        // 조커 카드가 가운데에 존재하고 오른쪽에도 존재하는 경우
                        else if (cardValue[arrPlayer[i, 2]] == 0)
                        {
                            resultPlayer[i] = cardValue[arrPlayer[i, 0]] * cardValue[arrPlayer[i, 0]];
                        }
                        // 가운데에만 존재하는경우
                        else
                        {
                            resultPlayer[i] = cardValue[arrPlayer[i, 0]] * cardValue[arrPlayer[i, 2]];
                        }
                    }
                    // 조커 카드가 가운데에 없는 경우.
                    else
                    {
                        // 조커 카드 2장이 왼쪽 오른쪽 각각에 존재할 경우.
                        if (cardValue[arrPlayer[i, 0]] == 0 && cardValue[arrPlayer[i, 2]] == 0)
                        {
                            resultPlayer[i] = 0;
                        }
                        // 조커 카드가 왼쪽에만 존재할 경우.
                        else if (cardValue[arrPlayer[i, 0]] == 0)
                        {
                            // 조커 카드를 제외한 나머지 2장이 같은 숫자의 카드 일 경우.
                            if (cardValue[arrPlayer[i, 1]] == cardValue[arrPlayer[i, 2]])
                            {
                                resultPlayer[i] = cardValue[arrPlayer[i, 0]] * 2;
                            }
                            // 조커 카드를 제외하고 2장의 카드 숫자가 다를 경우.
                            else
                            {
                                // 가운데 카드가 홀수 일경우.
                                if (cardValue[arrPlayer[i, 1]] % 2 != 0)
                                {
                                    resultPlayer[i] = 0;
                                }
                                // 가운데 카드가 짝수 일경우.
                                else
                                {
                                    resultPlayer[i] = cardValue[arrPlayer[i, 2]] * 2;
                                }

                            }
                        }
                        // 조커 카드가 오른쪽에만 존재할 경우.
                        else if (cardValue[arrPlayer[i, 2]] == 0)
                        {
                            // 조커 카드를 제외한 나머지 2장이 같은 숫자의 카드 일 경우.
                            if (cardValue[arrPlayer[i, 0]] == cardValue[arrPlayer[i, 1]])
                            {
                                resultPlayer[i] = cardValue[arrPlayer[i, 0]] * 2;
                            }
                            // 조커 카드를 제외하고 2장의 카드 숫자가 다를 경우.
                            else
                            {
                                // 가운데 카드가 홀수 일경우.
                                if (cardValue[arrPlayer[i, 1]] % 2 != 0)
                                {
                                    resultPlayer[i] = 0;
                                }
                                // 가운데 카드가 짝수 일경우.
                                else
                                {
                                    resultPlayer[i] = cardValue[arrPlayer[i, 0]] * 2;
                                }

                            }
                        }
                    }

                }
                // 조커 카드가 한장이라도 존재하지 않을 경우.
                else
                {
                    // 가운데 카드가 홀수 일경우.
                    if (cardValue[arrPlayer[i, 1]] % 2 != 0)
                    {
                        resultPlayer[i] = cardValue[arrPlayer[i, 0]] - cardValue[arrPlayer[i, 2]];
                    }
                    // 가운데 카드가 짝수 일경우.
                    else
                    {
                        resultPlayer[i] = cardValue[arrPlayer[i, 0]] + cardValue[arrPlayer[i, 2]];
                    }
                }
            }
        }
        Debug.Log("player01 result : " + resultPlayer[0]);
        Debug.Log("player02 result : " + resultPlayer[1]);
        Debug.Log("player03 result : " + resultPlayer[2]);
        Debug.Log("player04 result : " + resultPlayer[3]);
        Debug.Log("player05 result : " + resultPlayer[4]);
    }



}
