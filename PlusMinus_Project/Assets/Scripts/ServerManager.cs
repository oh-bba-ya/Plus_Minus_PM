using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using socket.io;

[System.Serializable]
public class RequestForm
{
    public string nickname;
}
public class ServerManager : MonoBehaviour
{
    Socket socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = Socket.Connect("http://pm.dalae37.com:4000");
        socket.On(SystemEvents.connect, () =>
        {
            print("서버 연결");
            EmitJoin();
        });

        socket.On("join", OnJoin);
        socket.On("pick", OnPick);
    }

    void EmitJoin()
    {
        print("연결 테스트 실시");
        RequestForm request = new RequestForm();
        request.nickname = PlayerPrefs.GetString("Nickname");
        socket.EmitJson("join", JsonUtility.ToJson(request));
    }

    void OnJoin(string json)
    {
        print("연결 테스트 성공");
    }

    public void EmitPick()
    {
        if (socket.IsConnected)
        {
            print("매칭 실시");
            RequestForm request = new RequestForm();
            request.nickname = PlayerPrefs.GetString("Nickname");
            socket.EmitJson("pick", JsonUtility.ToJson(request));
        }
    }

    void OnPick(string json)
    {
        SceneManager.LoadScene("InGame");
    }

    #region 돈 업데이트
    public void RefreshMoney()
    {
        StartCoroutine(RefreshStart());
    }

    IEnumerator RefreshStart()
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("userid", PlayerPrefs.GetString("Id")));
        UnityWebRequest webRequest = UnityWebRequest.Post("http://pm.dalae37.com:5000/refresh", form);
        yield return webRequest.SendWebRequest();

        string result = webRequest.downloadHandler.text;
        if (result.Equals("Success")){
            print("돈 업데이트 성공");
        }
    }
    #endregion
}
