using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SignUpScript : MonoBehaviour
{
    public Button SignUpBtn;
    public Button BackBtn;
    public InputField Nickname;
    public InputField Id;
    public InputField Password;

    public GameObject Success;
    
    void Start()
    {
        Success.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSignUp()
    {
        Success.SetActive(true);
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Login");
    }
}
