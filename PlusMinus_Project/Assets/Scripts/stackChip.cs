using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stackChip : MonoBehaviour
{
    //GameObject chip1;
    public List<GameObject> Slot = new List<GameObject>();
    public GameObject myObject;
    public List<GameObject> Chips = new List<GameObject>();
    public Button Check; public Button gameQuit;
    public Button gameRestart;
    int maxChips = 10;

    public GameObject AudioBg;
    public GameObject winner;
    public GameObject loser;
    public GameObject draw;

    public bool isWinner = false;
    public bool isLoser = false;
    public bool isDraw = false;
    // Start is called before the first frame update
    private void Start()
    {
        AudioBg.SetActive(false);
        winner.SetActive(false);
        loser.SetActive(false);
        draw.SetActive(false);
        gameRestart.enabled = false;
        gameQuit.enabled = false;
    }
  
    // Update is called once per frame
    void Update()
    {
        setAudio();
    }

    public void CreateGameObject()
    {
        if(maxChips > 0)
        {
            int amount = Random.Range(0, 3);
            
            for(int i = 0; i < amount; i++)
            {
                int slotI = Random.Range(0, Slot.Count);
                int chipI = Random.Range(0, Chips.Count);
                GameObject myChip = Instantiate(Chips[chipI], Slot[slotI].transform.position, Quaternion.identity) as GameObject;
                myChip.transform.SetParent(Slot[slotI].transform);
                maxChips--;

            }
            
        }        
    }

    public void OnClickCheckChip()
    {
        CreateGameObject();
    }

    public void setAudio()
    {

        if (!isWinner & !isLoser & !isDraw)
        {
            AudioBg.SetActive(true);
        }
        else if (isWinner || isLoser || isDraw)
        {
            gameQuit.enabled = true;
            gameRestart.enabled = true;
            if (isWinner)
            {
                AudioBg.SetActive(false);
                winner.SetActive(true);
            }
            else if (isLoser)
            {
                AudioBg.SetActive(false);
                loser.SetActive(true);
            }
            else
            {
                AudioBg.SetActive(false);
                draw.SetActive(true);
            }
        }

    }
}
