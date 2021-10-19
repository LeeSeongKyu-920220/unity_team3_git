using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
public class Title_Mgr : MonoBehaviour
{
    [Header("LoginPanel")]              //이렇게 쓰면 편집창에 태그들이 나온다. 
    public GameObject m_LoginPanelObj;
    public InputField IDInputField;     //Email 로 받을 것임
    public InputField PassInputField;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanelObj;
    public InputField New_IDInputField;  //Email 로 받을 것임
    public InputField New_PassInputField;
    public InputField New_NickInputField;
    public Button m_CABtn = null;
    public Button m_CancelBtn = null;

    [Header("LoginNotice")]
    public Image noticeimg = null;
    public Button noticebtn = null;
    public Text noticetxt = null;

    [Header("CANotice")]
    public Image noticeimg2 = null;
    public Button noticebtn2 = null;
    public Text noticetxt2 = null;

    string g_Message = "";
    string LoginUrl;
    string CreateUrl;
    string LoginUserDataUrl;

    // Start is called before the first frame update
    void Start()
    {
        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CreateCancelBtn);

        if (m_CABtn != null)
            m_CABtn.onClick.AddListener(CreateAccountBtn);

        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);

        LoginUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/Login.php";
        LoginUserDataUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/LoginUserData.php";
        CreateUrl = "http://pmaker.dothome.co.kr/TeamProject/TitleScene/CreateAccount.php";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenCreateAccBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(false);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(true);
    }

    public void CreateCancelBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(true);

        if (m_CreateAccPanelObj != null)
            m_CreateAccPanelObj.SetActive(false);
    }

    public void CreateAccountBtn() //계정 생성 요청 함수
    {
        string a_IdStr = New_IDInputField.text;
        string a_PwStr = New_PassInputField.text;
        string a_NickStr = New_NickInputField.text;

        StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
    }

    IEnumerator CreateCo(string a_IdStr, string a_PwStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", a_PwStr);        
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

    }

    public void LoginBtn()
    {
        string a_IdStr = IDInputField.text;
        string a_PwStr = PassInputField.text;
        StartCoroutine(LoginCo(a_IdStr, a_PwStr));
    }//public void LoginBtn()

    IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", a_PwStr); // 나중에 암호화 필요

        UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            
            GetUserData(sz, a_IdStr);            
        }//if (a_www.error == null)
        else 
        {
            Debug.Log("통신 에러");
        }
    }//IEnumerator LoginCo(string a_IdStr, string a_PwStr)

    void GetUserData(string LoginData, string a_IdStr) 
    {
        if (LoginData.Contains("Login-Success!!") == false) 
        {
            // 에러 띄우기
            if (LoginData.Contains("Pass does not Match"))
            {
                Debug.Log("로그인 패스워드 에러");
            }
            else 
            {
                Debug.Log($"PHP 에러 : {LoginData}");
            }
            return;
        }        

        MyInfo.m_ID = a_IdStr;

        //JSON 파싱
        if (LoginData.Contains("UserNick") == false) 
        {
            // 에러 띄우기
            Debug.Log("닉네임이 없음, 서버관리자 문의");
            return;
        }

        GetLoginUserInfo(LoginData);

        // 유저 정보를 통해 유닛 정보 받는 부분
        StartCoroutine(UserUnitGetData(MyInfo.m_No));
    }

    // 로그인 시 유저 정보 가져오는 부분
    void GetLoginUserInfo(string LoginData) 
    {        
        var N = JSON.Parse(LoginData);
        
        if (N["UserNo"] != null)
            MyInfo.m_No = N["UserNo"];

        if (N["UserNick"] != null)
            MyInfo.m_Nick = N["UserNick"];

        if (N["UserWin"] != null)
            MyInfo.m_Win = N["UserWin"].AsInt;

        if (N["UserDefeat"] != null)
            MyInfo.m_Defeat = N["UserDefeat"].AsInt;

        if (N["UserGold"] != null)
            MyInfo.m_Gold = N["UserGold"].AsInt;
               
    }

    IEnumerator UserUnitGetData(int UserNo) 
    {
        if (UserNo <= 0) 
        {
            Debug.Log("유저 No 파싱 안됨");
            yield break;
        }
        
        WWWForm form = new WWWForm();
        form.AddField("Input_No", UserNo.ToString(), System.Text.Encoding.UTF8);        
        UnityWebRequest a_www = UnityWebRequest.Post(LoginUserDataUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올때까지 대기하기...

        if (a_www.error == null) //에러가 나지 않았을 때 동작
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            UserGetUnitData(sz);

            // 데이터 가지고 와서 마지막에 씬 넘김
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }//if (a_www.error == null)
        else
        {
            Debug.Log("통신 에러");
        }
    }   

    // 유저가 가지고 있는 모든 유닛 정보 가져오기
    void UserGetUnitData(string LoginData) 
    {
        // 파싱된 결과를 바탕으로 아이템 초기화
        UserUnit a_UserUt;
        var N = JSON.Parse(LoginData);
        GlobalValue.m_AttUnitUserItem.Clear();
        GlobalValue.m_DefUnitItem.Clear();
        // 아이템을 전체적으로 초기화한다.
        // 먼저 JSON에 저장되어 있던 정보 초기화
        for (int i = 0; i < N["UnitList"].Count; i++)
        {
            int itemNo = N["UnitList"][i]["ItemNo"].AsInt;
            string itemName = N["UnitList"][i]["ItemName"];
            int Level = N["UnitList"][i]["Level"].AsInt;
            int isBuy = N["UnitList"][i]["isBuy"].AsInt;
            int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;
            int ItemUsable = N["UnitList"][i]["ItemUsable"].AsInt;
            int isAttack = N["UnitList"][i]["isAttack"].AsInt;

            int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
            int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
            int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
            float UnitAttSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;
            float UnitMoveSpeed = N["UnitList"][i]["UnitAttack"].AsFloat;
            int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
            int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;
            int UnitRange = N["UnitList"][i]["UnitRange"].AsInt;

            a_UserUt = new UserUnit();
            a_UserUt.m_UnitNo = itemNo;
            a_UserUt.m_Name = itemName;
            a_UserUt.m_Level = Level;
            a_UserUt.m_isBuy = isBuy;
            a_UserUt.m_unitkind = (Unitkind)KindOfItem;
            a_UserUt.m_unitType = UnitType.Att;
            a_UserUt.ItemUsable = ItemUsable;

            a_UserUt.m_Att = UnitAtt + Level * GlobalValue.UnitIncreValue;
            a_UserUt.m_Def = UnitDef + Level * GlobalValue.UnitIncreValue;
            a_UserUt.m_Hp = UnitHP + Level * GlobalValue.UnitIncreValue;
            a_UserUt.m_AttSpeed = UnitAttSpeed;
            a_UserUt.m_Speed = UnitMoveSpeed;
            a_UserUt.m_Price = Unitprice;
            a_UserUt.m_UpPrice = UnitUprice;
            a_UserUt.m_Range = UnitRange;

            if (isAttack == 0) // 방어일 경우
            {
                GlobalValue.m_DefUnitItem.Add(a_UserUt);
            }
            else // 공격일 경우
            {
                GlobalValue.m_AttUnitUserItem.Add(a_UserUt);
            }    
        }//for (int i = 0; i < N["UnitList"].Count; i++)  
    }//void UserGetUnitData(string LoginData) 
}
