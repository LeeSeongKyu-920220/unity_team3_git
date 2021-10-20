using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using SimpleJSON;

public class LobbyMgr : MonoBehaviour
{
    public Transform m_ScrollViewContent;
    public GameObject m_TextPrefab;

    public Button m_TestTextCreateBtn;
    public Button m_GameStartBtn;
    public Button m_DefConfigBtn;
    public Button m_StoreBtn;
    public Button m_MyRoomBtn;
    public Button m_LogOutBtn;

    public Text m_UserNameText;
    public Text m_UserGoldText;
    public Text m_WinLoseText;

    string LobbyStartUrl = "";
    string RankingSortUrl = "";

    string m_UserNick = "";
    int m_UserGold = 0;
    int m_UserWin = 0;
    int m_UserDefeat = 0;
    int m_rowsCount;

    string m_RankUserNick = "";
    int m_RankWinCount = 0;
    int m_Ranking = 0;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        if (m_TestTextCreateBtn != null)
            m_TestTextCreateBtn.onClick.AddListener(() =>
            {
                if (m_TextPrefab != null)
                {
                    GameObject obj = (GameObject)Instantiate(m_TextPrefab, m_ScrollViewContent);
                }
            });

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("SearchOpponentScene");
            });

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("StoreScene");
            });

        if (m_LogOutBtn != null)
            m_LogOutBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        LobbyStartUrl = "http://pmaker.dothome.co.kr/TeamProject/LobbyScene/TeamProjectTest.php";
        RankingSortUrl = "http://pmaker.dothome.co.kr/TeamProject/LobbyScene/RankingSortTest.php";

        StartCoroutine(StartTestCo());
        StartCoroutine(RankingSortCo());
    }

    IEnumerator StartTestCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", MyInfo.m_No.ToString(), System.Text.Encoding.UTF8);
        
        UnityWebRequest a_www = UnityWebRequest.Post(LobbyStartUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {            
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("UserId") == false)
            {
                Debug.Log("UserId 값이 없음!! Break함.");
                yield break;
            }

            var N = JSON.Parse(sz);
            if (N != null)
                Debug.Log("파싱 성공!");

            if (N["UserNick"] != null)
                m_UserNick = N["UserNick"];

            if (N["UserGold"] != null)
                m_UserGold = N["UserGold"].AsInt;

            if (N["UserWin"] != null)
                m_UserWin = N["UserWin"].AsInt;

            if (N["UserDefeat"] != null)
                m_UserDefeat = N["UserDefeat"].AsInt;

            m_UserNameText.text = m_UserNick + "님, 반갑습니다.";
            m_WinLoseText.text = "전적 : " + m_UserWin + "승  " + m_UserDefeat + "패";
            m_UserGoldText.text = "보유 골드 : " + m_UserGold;

            if (sz.Contains("Login-Success!!") == false)
            {
                Debug.Log("php에서 Login-Success!!까지 가지 못함.");
                yield break;
            }

        }
        else
            Debug.Log(a_www.error);

    }

    IEnumerator RankingSortCo()
    {
        UnityWebRequest a_www = UnityWebRequest.Get(RankingSortUrl);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            Debug.Log(sz);

            //if (sz.Contains("") == false)
            //{
            //    Debug.Log("UserId 값이 없음!! Break함.");
            //    yield break;
            //}

            var JSONBUFF = JSON.Parse(sz);
            if (JSONBUFF != null)
                //Debug.Log("JSONBUFF 파싱 성공!");

                if (JSONBUFF["RkCount"] != null)
                {
                    m_rowsCount = JSONBUFF["RkCount"].AsInt;
                    //Debug.Log("랭킹카운트 불러옴 : " + m_rowsCount);
                }

            if (JSONBUFF["RkList"] != null)
            {
                //Debug.Log("랭킹리스트 불러옴");

                for (int ii = 0; ii < m_rowsCount; ii++)
                {
                    m_RankUserNick = JSONBUFF["RkList"][ii]["UserNick"];
                    m_RankWinCount = JSONBUFF["RkList"][ii]["UserWin"].AsInt;
                    m_Ranking = ii + 1;

                    GameObject obj = (GameObject)Instantiate(m_TextPrefab, m_ScrollViewContent);
                    obj.GetComponent<Text>().text = "\n 이름(" + m_RankUserNick + ")\n" + " 승리(" + m_RankWinCount + ") "
                                                  + +m_Ranking + " 위";

                    if (ii == 0)
                        obj.GetComponent<Text>().color = new Color(255, 215, 0);

                    if (ii == 1)
                        obj.GetComponent<Text>().color = Color.red;

                    if (ii == 2)
                        obj.GetComponent<Text>().color = Color.blue;

                }

            }

        }
        else
            Debug.Log(a_www.error);


        //yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
