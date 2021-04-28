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
