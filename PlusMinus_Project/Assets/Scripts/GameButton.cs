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
        if (isClickRule == true)
        {
            isClickRule = false;
            rule.SetActive(false);
        }
        else
        {
            isClickRule = true;
            rule.SetActive(true);
        }
    }
    public void onClickClose()
    {
        isClickRule = false;
        rule.SetActive(false);
        Close.interactable = false;
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void ReStart()
    {
        Destroy(ServerManager.instance);
        SceneManager.LoadScene("Matching");
    }
}
