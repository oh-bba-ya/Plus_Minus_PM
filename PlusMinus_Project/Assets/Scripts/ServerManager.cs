using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using socket.io;
using Newtonsoft.Json;
[System.Serializable]
public class RequestForm
{
    public string nickname;
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

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;
    Socket socket;

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
            print("���� ����");
            EmitJoin();
        });
        socket.On("join", OnJoin);
        socket.On("pick", OnPick);
        socket.On("gameReady", OnGameReady);
        socket.On("batting", OnBatting);
        socket.On("gameEnd", OnGameEnd);
        socket.On("destory", OnDestroyRoom);
    }

    void EmitJoin()
    {
        print("���� �׽�Ʈ �ǽ�");
        RequestForm request = new RequestForm();
        request.nickname = PlayerPrefs.GetString("Nickname");
        socket.EmitJson("join", JsonUtility.ToJson(request));
    }

    void OnJoin(string json)
    {
        print("���� �׽�Ʈ ����");
    }

    public void EmitPick()
    {
        if (socket.IsConnected)
        {
            print("��Ī �ǽ�");

            RequestForm request = new RequestForm();
            request.nickname = PlayerPrefs.GetString("Nickname");
            socket.EmitJson("pick", JsonUtility.ToJson(request));
        }
    }

    void OnPick(string json)
    {
        SceneManager.LoadScene("InGame");
        PickResponseForm form = JsonUtility.FromJson<PickResponseForm>(json);

        print("�� ���ʴ�" + form.yourTurn + "�Դϴ�");

        GameObject.Find("GameManagaer").GetComponent<GameManager_GM>().SetMyPortIndex(form.yourTurn);
    }

    public void EmitGameReady()
    {
        socket.Emit("gameReady");
    }

    void OnGameReady(string json)
    {
        for(int i=0;i<5; i++)
        {
            for(int j=0; j<3; j++)
            {
                CardResponseForm form = JsonConvert.DeserializeObject<CardResponseForm>(json);
                GameObject.Find("GameManagaer").GetComponent<GameManager_GM>().arrPlayer[i, j] = form.arrPlayer[i][j];
            }
        }
    }

    public void EmitBatting(int turnIndex, int battingMoney)
    {
        string json = "";
        socket.EmitJson("batting", json);
    }

    void OnBatting(string json)
    {

    }

    void OnGameEnd(string json)
    {
        RefreshMoney();
    }

    void OnDestroyRoom(string json)
    {
        SceneManager.LoadScene("Matching");
    }

    #region �� ������Ʈ
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
            print("�� ������Ʈ ����");
        }
    }
    #endregion
}