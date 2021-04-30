using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameButton : MonoBehaviour
{

    public Button ruleBtn;
    public GameObject rule;
    public Button Close;
    bool isClickRule = false;
    // Start is called before the first frame update
    void Start()
    {
        Close.interactable = false;
        rule.SetActive(false);
        //ruleBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickRule()
    {
        isClickRule = true;
        rule.SetActive(true);

        Close.interactable = true;
    }
    public void onClickClose()
    {
        isClickRule = false;
        rule.SetActive(false);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void ReStart()
    {
        SceneManager.LoadScene("Maching");
    }
}
