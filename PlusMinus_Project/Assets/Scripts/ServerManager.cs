using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using socket.io;
using Newtonsoft.Json;

#region Server Connection Forms

[System.Serializable]
public class RequestForm
{
    public string nickname;
}

[System.Serializable]
public class DecisionRequestForm
{
    public int index;
    public int[] decision = new int[3];
}

[System.Serializable]
public class BettingRequestForm
{
    public int index;
    public int betMoney;
    public int totalMoney;
}

[System.Serializable]
public class PickResponseForm
{
    public int yourTurn;
}

[System.Serializable]
public class CardResponseForm
{
    public List<List<int>> arrPlayer = new List<List<int>>();
}

[System.Serializable]
public class BattingResponseForm
{
    public int addMoney;
}

[System.Serializable]
public class GameEndResponseForm
{
    public int addMoney;
}

#endregion

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;
    Socket socket;

    public int yourTurn;
    public int[,] arrPlayer = new int[5, 3];
    public bool firstData = true;
    public bool endData = false;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

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
        socket.On("gameReady", OnGameReady);
        socket.On("betting", OnBetting);
        socket.On("paseEnd", OnPaseEnd);
        socket.On("gameEnd", OnGameEnd);
        socket.On("destory", OnDestroyRoom);
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
        PickResponseForm form = JsonUtility.FromJson<PickResponseForm>(json);

        print("내 차례는" + form.yourTurn + "입니다");

        yourTurn = form.yourTurn;
        SceneManager.LoadScene("InGame");
    }

    public void EmitGameReady()
    {
        socket.Emit("gameReady");
    }

    public void EmitDeicision(int index, int[] decision)
    {
        if (firstData)
        {
            firstData = false;
        }
        else
        {
            endData = true;
        }
        DecisionRequestForm form = new DecisionRequestForm();
        form.index = index;
        form.decision = decision;

        socket.EmitJson("decision", JsonUtility.ToJson(form));
    }

    void OnGameReady(string json)
    {
        CardResponseForm form = JsonConvert.DeserializeObject<CardResponseForm>(json);
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                arrPlayer[i, j] = form.arrPlayer[i][j];
            }
        }
    }

    public void EmitBetting(int index, int betMoney, int totalMoney)
    {
        BettingRequestForm form = new BettingRequestForm();
        form.index = index;
        form.betMoney = betMoney;
        form.totalMoney = totalMoney;

        socket.EmitJson("batting", JsonUtility.ToJson(form));
    }

    void OnBetting(string json)
    {

    }

    void OnPaseEnd(string json)
    {

    }

    void OnGameEnd(string json)
    {
        GameEndResponseForm form = JsonUtility.FromJson<GameEndResponseForm>(json);
        int currentMoney = PlayerPrefs.GetInt("Money");
        currentMoney += form.addMoney;
        PlayerPrefs.SetInt("Money", currentMoney);
        RefreshMoney();
    }

    void OnDestroyRoom(string json)
    {
        SceneManager.LoadScene("Matching");
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
        form.Add(new MultipartFormDataSection("money", PlayerPrefs.GetInt("Money").ToString()));
        UnityWebRequest webRequest = UnityWebRequest.Post("http://pm.dalae37.com:5000/refresh", form);
        yield return webRequest.SendWebRequest();

        string result = webRequest.downloadHandler.text;
        if (result.Equals("Success"))
        {
            print("돈 업데이트 성공");
        }
    }
    #endregion
}
