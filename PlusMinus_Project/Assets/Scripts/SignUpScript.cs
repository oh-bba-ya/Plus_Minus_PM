using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class SignUpScript : MonoBehaviour
{
    public Button SignUpBtn;
    public Button BackBtn;
    public InputField Nickname;
    public InputField Id;
    public InputField Password;

    public GameObject Success;

    private bool requestFinish = false;
    private string result;
    void Start()
    {
        Success.SetActive(false);   
    }

    private void Update()
    {
        if (requestFinish)
        {
            if (result.Equals("Success"))
            {
                Success.SetActive(true);
            }
            else //회원가입 실패 메시지
            {

            }
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Nickname", Nickname.text);
        PlayerPrefs.SetString("Id", Id.text);
        PlayerPrefs.SetString("Password", Password.text);
    }


    public void OnClickSignUp()
    {
        StartCoroutine(Register());
    }

    #region 회원가입
    IEnumerator Register()
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("userid", Id.text));
        form.Add(new MultipartFormDataSection("password", Password.text));
        form.Add(new MultipartFormDataSection("username", Nickname.text));
        UnityWebRequest webRequest = UnityWebRequest.Post("http://pm.dalae37.com:5000/register", form);
        yield return webRequest.SendWebRequest();

        result = webRequest.downloadHandler.text;
        requestFinish = true;
    }
    #endregion

    public void OnClickBack()
    {
        SceneManager.LoadScene("Login");
    }
}
