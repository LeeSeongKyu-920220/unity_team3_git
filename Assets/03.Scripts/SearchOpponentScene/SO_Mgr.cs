using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SO_Mgr : MonoBehaviour
{
    #region ---------변수 선언
    [Header("Opponent List")]
    public GameObject Opponents_SV_Content;         //Scroll View Content
    public GameObject OpponentPrefab;       //Opponent Info Prefab
    public GameObject TouchCover_Panel;     //TouchCover                            //Opponents_SV가 터치가 안되도록

    [System.Serializable]
    public struct OI_Panel
    {
        public GameObject OpponentInfo_Panel;   //UserInfo_Panel
        [Tooltip("Opponent Info Nick UI text / '상대 닉네임' :")]
        public Text OI_Nick_txt;
        [Tooltip("Opponent Info Win UI text / '상대 승 수'승")]
        public Text OI_Win_txt;
        [Tooltip("Opponent Info Defeat UI text / '상대 패 수'패")]
        public Text OI_Defeat_txt;
        [Tooltip("Opponent Info UnitPower UI text / '유닛 전투력'")]
        public Text OI_UnitPower_txt;
        [Tooltip("Opponent Info UserItem.Count UI text / '상대 유닛 수'개")]
        public Text OI_UserItemCount_txt;
        public Button Fight_Btn;                //Opponent Fight Btn                    //공격 시작
        public Button Cancel_Btn;               //Opponent Cancle Btn                   //취소
        public static bool OI_OnOff = false;
    }

    [Header("Opponent Info Panel")]
    [Tooltip("툴팁 : 기능 / UI 예시")]
    public OI_Panel OI;

    [Header("Other UI")]
    public Button Go_Lobby_Btn;             //Go Lobby Btn                          //로비로 돌아가기

    [Header("ConfigObj")]
    public Button Setting_Btn;              //Setting Btn                           //환경설정버튼
    public Button Google_Btn;               //Google Btn                            //구글 관련 버튼
    public GameObject Google_ScrollRoot;    //Mask
                                            //public Button Acheive_Btn;              //Acheive Btn
                                            //public Button Leaderboard_Btn;          //Leaderboard Btn

    [Header("FadeInOut Object")]
    public GameObject LeftFade;
    public GameObject RightFade;
    public GameObject UpFade;
    Vector3 LeftFadePos = new Vector3(-820, 0, 0);
    Vector3 RightFadePos = new Vector3(500, 0, 0);
    Vector3 UpFadePos = new Vector3(0, 145, 0);
    float FadeTime = 1.5f;

    //DB
    string OI_Url = "";
    bool DBConnected = false;
    public static SO_Mgr Inst;
    #endregion

    public class OpponentInfo
    {
        public string m_ID = "";
        public string m_Nick = "";
        public int m_Win = 0;
        public int m_Defeat = 0;
        public int UnitCount = 0;
        public int UnitPower = 0;
    }

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DBConnected = false;
        OI_Url = "http://kdhhost.dothome.co.kr/TeamPortfoilo_TestDB/UserInfoConnect.php";

        if (OI.OpponentInfo_Panel != null)
        {
            if (Go_Lobby_Btn != null)
                Go_Lobby_Btn.onClick.AddListener(()=>
                {
                    GO_Lobby("LobbyScene");
                });

            if (OI.Cancel_Btn != null)
                OI.Cancel_Btn.onClick.AddListener(() => OI_Panel.OI_OnOff = false);

            if (OI.Fight_Btn != null)
                OI.Fight_Btn.onClick.AddListener(() =>
                {
                    GO_Lobby("InGame_Map1");
                });
        }

        if (Google_Btn != null)
            Google_Btn.onClick.AddListener(Google);

        DBConnect();

        if (LeftFade != null)
            StartCoroutine(FadeIn(LeftFade, LeftFadePos));

        if (RightFade != null)
            StartCoroutine(FadeIn(RightFade, RightFadePos));

        if (UpFade != null)
            StartCoroutine(FadeIn(UpFade, UpFadePos));

    }

    // Update is called once per frame
    void Update()
    {
        if (OI.OpponentInfo_Panel != null)
            OI.OpponentInfo_Panel.SetActive(OI_Panel.OI_OnOff);

        if (TouchCover_Panel != null)
            TouchCover_Panel.SetActive(OI_Panel.OI_OnOff);
    }

    void DBConnect()
    {
        StartCoroutine(UserInfo_DBConnect());
    }

    IEnumerator UserInfo_DBConnect()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", "test"/*GlobalaValue.g_UserId*/, System.Text.Encoding.UTF8);    //유저의 아이디

        UnityWebRequest a_www = UnityWebRequest.Post(OI_Url, form);
        yield return a_www.SendWebRequest();    //응답이 올 때까지 대기하기...

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            Debug.Log(sz);
            if (!sz.Contains("DB Connect"))
                yield break;

            DBConnected = true;

            //JSON 파싱
            var N = JSON.Parse(sz);
            if (N == null)
                yield break;

            Debug.Log(N["RkList"].Count);
            if (N["RkList"].Count <= 10)   //전체 유저가 10명 이하일 때
            {
                for (int i = 0; i < N["RkList"].Count; i++)
                {
                    string UserId = N["RkList"][i]["UserId"];
                    string Nick = N["RkList"][i]["UserNick"];
                    int WinTxt = N["RkList"][i]["UserWin"].AsInt;
                    int DefeatTxt = N["RkList"][i]["UserDefeat"].AsInt;
                    int UnitCount = N["RkList"][i]["Unit"].Count;
                    int UnitPower = 0;
                    if (UnitCount > 0)
                    {
                        for (int j = 0; j < UnitCount; j++)
                        {
                            int UnitLevel = N["RkList"][i]["Unit"][j]["Level"].AsInt;
                            int UnitAttack = N["RkList"][i]["Unit"][j]["UnitAttack"].AsInt;
                            UnitPower += UnitLevel * UnitAttack;    //레벨 * 유닛공격력
                        }
                    }
                    CreateList(UserId, Nick, WinTxt, DefeatTxt, UnitCount, UnitPower);
                }
            }
            else if (N["RkList"].Count > 10)    //테스트완료
            {
                int[] O_List = GetRandomInt(10, 1, N["RkList"].Count);
                for (int i = 0; i < O_List.Length; i++)
                {
                    string UserId = N["RkList"][O_List[i]]["UserId"];
                    string Nick = N["RkList"][O_List[i]]["UserNick"];
                    int WinTxt = N["RkList"][O_List[i]]["UserWin"].AsInt;
                    int DefeatTxt = N["RkList"][O_List[i]]["UserDefeat"].AsInt;
                    int UnitCount = N["RkList"][O_List[i]]["Unit"].Count;
                    int UnitPower = 0;
                    if (UnitCount > 0)
                    {
                        for (int j = 0; j < UnitCount; j++)
                        {
                            int UnitLevel = N["RkList"][O_List[i]]["Unit"][j]["Level"].AsInt;
                            int UnitAttack = N["RkList"][O_List[i]]["Unit"][j]["UnitAttack"].AsInt;
                            UnitPower += UnitLevel * UnitAttack;    //레벨 * 유닛공격력
                        }
                    }
                    CreateList(UserId, Nick, WinTxt, DefeatTxt, UnitCount, UnitPower);
                    Debug.Log(O_List[i]);
                }
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    int[] GetRandomInt(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;
        for (int i = 0; i < length; ++i)
        {
            while (true)
            {
                randArray[i] = Random.Range(min, max);
                isSame = false;
                for (int j = 0; j < i; ++j)
                {
                    if (randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame)
                    break;
                else
                    continue;
            }
        }
        return randArray;
    }

    void CreateList(string UserId, string Nick, int WinTxt, int DefeatTxt, int U_Count, int U_Power)
    {
        GameObject OI_Obj = Instantiate(OpponentPrefab, Opponents_SV_Content.transform);
        OI_Obj.GetComponent<OI_Item>().OpponentId = UserId;
        OI_Obj.GetComponent<OI_Item>().OpponentNick_txt.text = Nick + " :";
        OI_Obj.GetComponent<OI_Item>().OpponentWin_txt.text = WinTxt + "승";
        OI_Obj.GetComponent<OI_Item>().OpponentDefeat_txt.text = DefeatTxt + "패";
        OI_Obj.GetComponent<OI_Item>().U_Count = U_Count;
        OI_Obj.GetComponent<OI_Item>().U_Power = U_Power;
    }

    void GO_Lobby(string Scene_Str)
    {
        StartCoroutine(FadeOut(LeftFade, LeftFadePos));
        StartCoroutine(FadeOut(RightFade, RightFadePos));
        StartCoroutine(FadeOut(UpFade, UpFadePos));

        StartCoroutine(Wait(Scene_Str));
    }

    void Google()
    {
        if (Google_ScrollRoot == null)
            return;

        StartCoroutine(ScrollMove());
    }

    IEnumerator ScrollMove()
    {
        if (Google_ScrollRoot.transform.localPosition.x == 0)
        {
            for (int i = 0; i < 175; i++)
            {
                Google_ScrollRoot.transform.Translate(new Vector3(i, 0, 0), Space.Self);
                if (Google_ScrollRoot.transform.localPosition.x >= 175)
                {
                    Google_ScrollRoot.transform.localPosition = new Vector3(175, 0, 0);
                    yield break;
                }
                yield return new WaitForSeconds(.01f);
            }
        }
        else if (Google_ScrollRoot.transform.localPosition.x == 175)
        {
            for (int i = 0; i < 175; i++)
            {
                Google_ScrollRoot.transform.Translate(new Vector3(-i, 0, 0), Space.Self);
                if (Google_ScrollRoot.transform.localPosition.x <= 0)
                {
                    Google_ScrollRoot.transform.localPosition = Vector3.zero;
                    yield break;
                }
                yield return new WaitForSeconds(.01f);
            }
        }
    }

    #region =======FadeIn,Out
    IEnumerator FadeIn(GameObject FadeObj, Vector3 FadeInPos)
    {
        FadeObj.transform.localPosition = FadeInPos;
        while (FadeObj.transform.localPosition.normalized != -FadeInPos.normalized)
        {
            FadeObj.transform.Translate(-FadeInPos.normalized * 20f, Space.Self);
            yield return new WaitForSeconds(FadeTime / FadeInPos.magnitude);
        }
        FadeObj.transform.localPosition = Vector3.zero;
        yield break;
    }

    IEnumerator FadeOut(GameObject FadeObj, Vector3 FadeOutPos)
    {
        if (FadeObj.transform.localPosition != FadeOutPos)
        {
            while (FadeObj.transform.localPosition.magnitude < FadeOutPos.magnitude)
            {
                FadeObj.transform.Translate(FadeOutPos.normalized * 7.5f, Space.Self);
                yield return new WaitForSeconds(FadeTime / FadeOutPos.magnitude);
            }
            FadeObj.transform.localPosition = FadeOutPos;
            yield break;
        }
    }
    IEnumerator Wait(string Scene_Str)
    {
        yield return new WaitForSecondsRealtime(0.75f);
        SceneManager.LoadScene(Scene_Str);
    }
    #endregion

}
