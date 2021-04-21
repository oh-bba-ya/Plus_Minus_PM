using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrag : MonoBehaviour
{
    
    public GameManager_GM GM;

    private GameObject[] playerCardPos = new GameObject[3];

    private int thisIndex;


    private void Start()
    {
        

        playerCardPos[0] = GameObject.FindGameObjectWithTag("player03_left");
        playerCardPos[1] = GameObject.FindGameObjectWithTag("player03_center");
        playerCardPos[2] = GameObject.FindGameObjectWithTag("player03_right");




        //Debug.Log("arraplyer[" + GM.myPortIndex + "] : " + GM.arrPlayer[GM.myPortIndex, 0] + ", " + GM.arrPlayer[GM.myPortIndex, 1] + ", " + GM.arrPlayer[GM.myPortIndex, 2]);
    }

    private void Update()
    {



        if (!GM.isDragDrop && GM.isCheckCard)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInpo = Physics2D.Raycast(touchPos, Camera.main.transform.forward);
                if (hitInpo.collider != null)
                {
                    if (hitInpo.transform.gameObject == playerCardPos[0])
                    {
                        thisIndex = 0;
                    }
                    else if (hitInpo.transform.gameObject == playerCardPos[1])
                    {
                        thisIndex = 1;
                    }
                    else if (hitInpo.transform.gameObject == playerCardPos[2])
                    {
                        thisIndex = 2;
                    }
                }
            }
        }

        if(GM.isDragDrop)
        {
            ChangeCenter(GM.myPortIndex);
            

        }

    }





    /// <summary>
    /// 스프라이트 정보만 바뀜. /
    /// 카드 확인 , 카드 배치 (드래그) 가 끝나면 가운데 카드 부호가 결정되며 부호 공개됨 /
    /// 어차피 모든 플레이어는 player03 오브젝트 위치에 자기 자신의 카드가 위치하므로 CardDrag.cs 에 함수를 작성함.
    /// </summary>
    /// <param name="myNumber"> 나의 인덱스 번호 </param>
    void ChangeCenter(int myNumber)
    {
        // 일단 카드 부호들 스프라이트가 없어서 뒷면으로 대체함.
        // GameManager_GM.cs에 존재하는 arrPlayer의 정보는 배팅이 끝나고 결과 공개 때 바꾸면되므로 굳이 여기서 바꾸지 않음.
        

        if(GM.cardValue[GM.arrPlayer[myNumber, 1]] == 0)            // 조커일경우.
        {
            //playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.expressions[2];

            playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.cards[0];
        }
        else if (GM.cardValue[GM.arrPlayer[myNumber, 1] % 2] == 0)   // 짝수        // arrplayer에 있는것은 스프라이트 인덱스이다. 
        {
            //playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.expressions[0];
  
            playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.cards[0];

        }
        else if (GM.cardValue[GM.arrPlayer[myNumber, 1] % 2] != 0)   // 홀수
        {
            //playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.expressions[1];

            playerCardPos[1].GetComponent<SpriteRenderer>().sprite = GM.cards[0];

        }


        GM.isChangeCard = true;         // 가운데 카드 이미지 변환 완료.
        GM.isBetting = false;           // 베팅 시작.
    }





    /// <summary>
    /// 마우스위치에 카드가 따라감.
    /// </summary>
    private void OnMouseDrag()
    {
        if (!GM.isDragDrop)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }


    /// <summary>
    /// 선택한 카드와 충돌한 카드와 카드 정보[스프라이트 , 내용] 스왑
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        switch (thisIndex)
        {
            case 0:
                if(collision.gameObject == playerCardPos[1])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 0];
                    tempObj = playerCardPos[0].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[0].GetComponent<SpriteRenderer>().sprite = playerCardPos[1].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[1].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 0] = GM.arrPlayer[GM.myPortIndex, 1];
                    GM.arrPlayer[GM.myPortIndex, 1] = temp;

                    
                }
                else if(collision.gameObject == playerCardPos[2])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 0];
                    tempObj = playerCardPos[0].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[0].GetComponent<SpriteRenderer>().sprite = playerCardPos[2].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[2].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 0] = GM.arrPlayer[GM.myPortIndex, 2];
                    GM.arrPlayer[GM.myPortIndex, 2] = temp;
                    


                }
                break;

            case 1:
                if (collision.gameObject == playerCardPos[0])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 1];
                    tempObj = playerCardPos[1].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[1].GetComponent<SpriteRenderer>().sprite = playerCardPos[0].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[0].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 1] = GM.arrPlayer[GM.myPortIndex, 0];
                    GM.arrPlayer[GM.myPortIndex, 0] = temp;
                    

                }
                else if (collision.gameObject == playerCardPos[2])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 1];
                    tempObj = playerCardPos[1].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[1].GetComponent<SpriteRenderer>().sprite = playerCardPos[2].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[2].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 1] = GM.arrPlayer[GM.myPortIndex, 2];
                    GM.arrPlayer[GM.myPortIndex, 2] = temp;

                    
                }
                break;

            case 2:
                if (collision.gameObject == playerCardPos[0])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 2];
                    tempObj = playerCardPos[2].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[2].GetComponent<SpriteRenderer>().sprite = playerCardPos[0].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[0].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 2] = GM.arrPlayer[GM.myPortIndex, 0];
                    GM.arrPlayer[GM.myPortIndex, 0] = temp;
                    

                }
                else if (collision.gameObject == playerCardPos[1])
                {
                    
                    Sprite tempObj;
                    int temp;

                    temp = GM.arrPlayer[GM.myPortIndex, 2];
                    tempObj = playerCardPos[2].GetComponent<SpriteRenderer>().sprite;

                    playerCardPos[2].GetComponent<SpriteRenderer>().sprite = playerCardPos[1].GetComponent<SpriteRenderer>().sprite;
                    playerCardPos[1].GetComponent<SpriteRenderer>().sprite = tempObj;

                    GM.arrPlayer[GM.myPortIndex, 2] = GM.arrPlayer[GM.myPortIndex, 1];
                    GM.arrPlayer[GM.myPortIndex, 1] = temp;
                    

                }
                break;
        }



    }

}
