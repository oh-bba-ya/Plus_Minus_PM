using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public GameObject SignupCanvas;
    public InputField Id;
    public InputField Password;
    public Button Login;
    public Button Main_SignUp;

    public Button Signup;
    // Start is called before the first frame update
    void Start()
    {
        SignupCanvas.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickMainSignUp()
    {
        SignupCanvas.SetActive(true);
    }
}
