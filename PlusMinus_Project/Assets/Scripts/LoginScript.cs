using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour
{
    public InputField Id;
    public InputField Password;
    public Button Login;
    public Button Main_SignUp;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Nickname"))
        {
            Id.text = PlayerPrefs.GetString("Id");
        }
    }

    public void OnClickMainSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }
}
