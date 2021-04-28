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
    public int[] cardValue = { -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ,11, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

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
    public bool isCheckCard = false;
    public bool isDragDrop = false;
    public bool isChangeCard = false;                  // 카드 가운데 + , - 로 바뀜.
    public bool isBetting = true;                       // 카드가 공개된 후 시작이므로 changeCard()함수가 시작된 후 실행됨.
    public bool isLeftCard = false;                    // 왼쪽 카드 공개 여부.
    public bool isRightCard = false;                    // 오른쪽 카드 공개 여부.

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
    bool myTurn = false;
    bool turn = false;
    public int totalMoney = 0;
    public int startmoney = 20;         // 처음 시작시 판돈 금액
    public int betMoney = 0;
    int my_PreBetMoney = 0;             // 나의 이전 베팅 금액. [ 해당하는 베팅 턴 ex) 가운데 카드 공개후 시작한 베팅턴에서만 ] 
    int playerMoney;                    // 본인 플레이어[playerprefabs] 돈 가져와서 저장.

    int[] testplayer = { 0, 1, 2, 3, 4 };

    #region 코드 추가 by 이한수
    public void SetMyPortIndex(int myPortIndex)
    {
        this.myPortIndex = myPortIndex;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
        isBetting = true;
        betMoney = startmoney;
        Load();           // 현재 참여한 플레이어 배열 여기다가 돈 저장해서 빼고 더하고 할거임.

        for (int i = 0; i < startPos.Length; i++)
        {
            startPos[i] = originPos[i].transform.position;
        }

        //int[] testplayer = { 0, 1, 2, 3, 4 };
        //int[,] testcards = { { 0, 1, 2 }, { 3, 4, 5 } };

        DistributeCard(curPlayer, myPortIndex, testplayer, arrPlayer, "first");

        BaseBettingStart();

        Save(2);
    }

    // Update is called once per frame
    void Update()
    {
        inGameTime += Time.deltaTime;

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
        }

        if (isCheckCard && inGameTime >= set_turnTime)           // 카드 확인도 하고 , 시간초과일때 드래그 스왑 종료.
        {
            OFFCardDrag();
            DistributeCard(curPlayer, myPortIndex, testplayer, arrPlayer, "second");
        }

        if (isDragDrop && isChangeCard)
        {
            BackPosition();
            DistributeCard(curPlayer, myPortIndex, testplayer, arrPlayer, "third");
            DistributeCard(curPlayer, myPortIndex, testplayer, arrPlayer, "last");
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

    }

    /// <summary>
    /// 서버에서 관리할 마이턴 차례.
    /// </summary>
    /// <param name="_myTurn"></param>
    public void setMyTurn(bool _myTurn)
    {
        myTurn = _myTurn;
    }

    /// <summary>
    /// 서버에서 관리할 전체 베팅 턴 종료시 true.
    /// </summary>
    /// <param name="_turn"></param>
    public void BetTurn(bool _turn)
    {
        turn = _turn;
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

        if (isCheckCard && isDragDrop)
        {
            stepText.text = "";
        }

        // 현재 전체 표시
        TotalText.text = totalMoney.ToString();

        // 전 플레이어가 제시한 베팅 머니 표현
        betMoneyText.text = betMoney.ToString();

        if (!isBetting && isChangeCard)
        {
            stepText.text = "베팅 시작";
        }

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
    /// 게임 시작시 참가한 인원 만큼 카드를 나눠줌.
    /// </summary>
    /// <param name="in_player"> 현재 참여한 총 게임 인원 </param>
    /// <param name="myNumber"> 자기 자신의 번호 (서버에서 받아올 플레이어 정보랑 비교하기 위한것) </param>
    /// <param name="playerNumber"> 서버에서 넘어오는 플레이어들의 고유 번호 (본인것도 포함) </param>
    /// <param name="playerCards"> 서버에서 넘어오는 플레이어들의 카드 정보 (본인것도 포함) /* 파라미터 입력대로 만들지 않고 전역변수 appPlayer를 이용해서 만들었기 때문에 만약 서버에서 카드를 나눠주는것이 온다면 코드 수정해야함. */ </param>   확인요망!!
    /// <param name="step"> "first" = 시작하자마자 카드 뒷면 스프라이트만 나눠줌 ,"second " = 가운데 카드 스프라이트 변경 ,"third" = 왼쪽카드 스프라이트 공개 , "last" = 오른쪽 카드 스프라이트 공개. </param>
    void DistributeCard(int in_player, int myNumber, int[] playerNumber, int[,] playerCards, string step)
    {
        // 카드가 잘나눠지는지 확인하기 위해 앞면 카드를 배정해줬지만 발표가 끝난 후에는 카드 뒷면을 할당해줄거임.
        // 스프라이트 앞면에 해당하는 인덱스를 각 플레이어에게 할당해줄거임.
        // 뒷면 카드 확인 하기위해 mynumber = 4일때 가렸음.

        Debug.Log("Distri : " + in_player);

        int playerIndex = 2;                // 본인의 고유정보와 , 서버에서 전달된 본인의 정보 인덱스

        for (int i = 0; i < in_player; i++)
        {
            if (myNumber == playerNumber[i])
            {
                playerIndex = i;
                myPortIndex = i;
            }
        }

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
        else if(step == "second")           // 가운데 카드 공개
        {
            Debug.Log("가운데 카드 공개");
            switch (playerIndex)
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
            isBetting = false;
        }
        else if (step == "third")               // 왼쪽 카드 공개.
        {
            Debug.Log("왼쪽 카드 공개");
            switch (playerIndex)
            {
                case 0:             // player 01. 플레이어 본인 카드 외에는 
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
            switch (playerIndex)
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
        StopCoroutine(ClickCard(2, myPortIndex));
        checkCount = 1;
    }

    /// <summary>
    /// 일정 시간동안 카드를 확인 안할시 플레이어 카드 3장중 center 카드 자동 확인.
    /// CheckCard 코루틴 함수 중단.
    /// 만약 카드 확인을 못하게 된다면 확인할 시간도 없이 바로 + , - 로 바뀜 [ 하지만 굳이 보여줄 시간을 따로 줄 필요없음 ]
    /// </summary>
    /// <param name="select"> player01~05 위치 중 모든 플레이어는 가운데에 자기 카드가 존재하므로 디폴트 = player03 오브젝트의 위치 </param>
    /// <param name="myNumber"> 서버로 부터 내 번호를 입력 받아서 나의 카드 부여 </param>
    void AutoCheck(int select, int myNumber)
    {
        players[select].handcard[1].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[myNumber, 1]];
        isCheckCard = true;
        StopCheckCard();
    }


    /// <summary>
    /// 카드 한장만 터치 할 수 있음.
    /// </summary>
    /// <param name="select"> 가운데 카드 인덱스02 , 가운데에 존재하는 오브젝트 위치.  </param>
    /// <param name="myNumber">  본인 번호 </param>
    /// <returns></returns>
    IEnumerator ClickCard(int select, int myNumber)
    {
        GameObject[] myCardPos = new GameObject[3];
        myCardPos[0] = players[select].handcard[0]; //left
        myCardPos[1] = players[select].handcard[1]; //center
        myCardPos[2] = players[select].handcard[2];//right

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
                            players[select].handcard[i].GetComponent<SpriteRenderer>().sprite = cards[arrPlayer[myNumber, i]];
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
        int rand01, rand02;
        int[] array_temp = new int[cards.Length];
        int variable_temp;

        int count = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            array_temp[i] = count;
            count++;
        }


        for (int i = 0; i < in_player * 3; i++)
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

            for (int j = 0; j < 3; j++)
            {
                arrPlayer[i, j] = array_temp[count];
                count++;
            }
        }
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
        totalMoney += betMoney;
        playerMoney -= betMoney;
        players[myPortIndex].money = playerMoney;
        my_PreBetMoney = betMoney;
    }

    /// <summary>
    /// 더블 : 베팅 머니에 2배를 해서 베팅함
    /// </summary>
    public void OnClickDoubleBtn()
    {
        btnIndex = 1;
        Debug.Log("Double");
        betMoney = betMoney * 2;
        totalMoney += betMoney;
        players[myPortIndex].money = playerMoney;
        my_PreBetMoney = betMoney;
    }

    /// <summary>
    /// 하프 버튼 : 전체 판돈의 절반을 베팅함.
    /// </summary>
    public void OnClickHalfBtn()
    {
        btnIndex = 2;
        Debug.Log("Half");
        betMoney = betMoney + (totalMoney / 2);
        totalMoney += betMoney;
        my_PreBetMoney = betMoney;

        players[myPortIndex].money = playerMoney;
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
        playerMoney -= startmoney * curPlayer;
        players[myPortIndex].money = playerMoney;
    }

    /// <summary>
    /// 플레이어 들의 3장의 카드 합산 결과를 저장하는 함수 
    /// 카드를 부여 받고 난 후 플레이어들의 카드 결과는 정해짐.
    /// </summary>
    /// <param name="in_player"> 현재 게임에 참여한 인원 </param>
    /// <param name="myNumber"> 자신의 포트 번호 </param>
    /// <param name="playerNumber"> 전체 플레이어(본인 포함) 포트 번호 </param>
    /// <param name="playerCards"> 전체 플레이어(본인 포함) 카드 번호</param>
    void ResultNumber(int in_player, int myNumber, int[] playerNumber, int[,] playerCards)
    {
        int myIndex = 0;
        int[] resultPlayer = new int[in_player];         // 플레이어들의 합산 결과.

        for (int i = 0; i < in_player; i++)
        {
            if (playerNumber[i] == myNumber)
            {
                myIndex = i;
            }

        }

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
