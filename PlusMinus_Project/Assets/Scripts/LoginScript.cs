using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class LoginScript : MonoBehaviour
{
    public InputField Id;
    public InputField Password;
    public Button Login;
    public Button Main_SignUp;

    private bool requestFinish = false;
    private string result;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Load();
        if (requestFinish)
        {
            if (result.Equals("Success"))
            {
                print("성공");
            }
            else
            {
                print("실패");
            }
        }
    }

    #region 로그인
    public void LoginButton()
    {
        StartCoroutine(LoginStart());
    }

    IEnumerator LoginStart()
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("userid", Id.text));
        form.Add(new MultipartFormDataSection("password", Password.text));
        UnityWebRequest webRequest = UnityWebRequest.Post("http://pm.dalae37.com:5000/login", form);
        yield return webRequest.SendWebRequest();

        result = webRequest.downloadHandler.text;
        requestFinish = true;
    }
    #endregion

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
