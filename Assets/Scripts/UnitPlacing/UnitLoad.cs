using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;           // 네트워크 사용을 위한 네임스페이스
using SimpleJSON;                       // 심플 제이슨 사용을 위한 네임스페이스


/*
 ======================================================================
    사실 서버에서 데이터를 가져오긴 하는데 만약 유저 로그인 후 초반에 데이터들을
   Init 시키고 거기서 가져오는 것이 편리할 거 같긴 합니다.
 ======================================================================
 */

// 추후에 데이터를 가져오기 위한 클래스 선언
public class UnitInfo
{
    public int itemNo;
    public string itemName;
    public int itemLevel;
    public char isBuy;
    public float posX;
    public float posY;
    public string itemKind;
    public int itemUsable;
    public char isAttack;
    public int userID;

    public UnitInfo()
    {
    }

    public void UnitPrint()
    {

        Debug.Log("itemNo : " + itemNo);
        Debug.Log("itemName : " + itemName);
        Debug.Log("itemLevel : " + itemLevel);
        Debug.Log("isBuy : " + isBuy);
        Debug.Log("posX : " + posX);
        Debug.Log("posY : " + posY);
        Debug.Log("itemKind : " + itemKind);
        Debug.Log("itemUsable : " + itemUsable);
        Debug.Log("isAttack : " + isAttack);
        Debug.Log("userID : " + userID);
    }
}

/*
 ====================================================================================================
 서버에서 정보를 받아오는 것도 방법이지만
 글로벌 변수에 unit Usable(==unitCount) 가 있는 경우 그것을 받아와도 된다.

 아직 UserItem 테이블의 정보를 받아오는 스크립트가 없으므로 부득이하게 만들었다.
 만약 테이블의 정보를 받아온다면 그것을 UnitPool의 int[]인 UnitCountLimit 변수에 테이블 컬럼인 KindOfItem과 
 인덱스를 일치시켜주면 된다. (만약 KindOfItem 이 1이라면 int[0]에 할당한다.) 유닛 종류는 최대 5종이기 때문
 ====================================================================================================
 */
public class UnitLoad : MonoBehaviour
{
    // 싱글톤 활용을 위한 자기 자신 선언
    public static UnitLoad unitLoad;

    // ↓↓↓↓↓ 임시 유저 아이디
    int userID = 0;

    // 유저 정보의 리스트 ... 이것을 어떻게 활용할지는 추후에 논의
    [HideInInspector] public List<UnitInfo> userUnitInfoList = new List<UnitInfo>();

    string InitURL = "http://sangku2000.dothome.co.kr/UserItem/UserItemLoad.php";

    void Awake()
    {
        unitLoad = this;            // 싱글톤 활용을 위한 자기 자신 할당

        // ↓↓↓↓↓ 임시 할당
        userID = 1;             // 임시 유저 ID 할당 나중에 플레이어 값을 가져와야함
                                // 글로벌 변수의 ID로 대체해야한다.



        //// !!!!!! 글로벌 변수와 컬럼이 완벽히 일치되면 주석을 풀어주세요!!!
        //// ↓↓↓↓↓↓↓↓ 글로벌에서부터 아이템의 사용 횟수를 받아오는 함수 실행 
        LoadAttackUnit();

        // 유저의 유닛 정보를 받아온다.
        //StartCoroutine(GetUserUnitInfo_Co());
    }

    // 유저의 유닛 정보를 DB에서 받아온다.
    private IEnumerator GetUserUnitInfo_Co()
    {
        // ============================================
        // 여기에 UserID 검사 후 return 부분 추가 필요
        // =============================================

        // 통신을 위한 form 작성 ... 아이디 값 송신해서 서버와 대조
        WWWForm form = new WWWForm();
        form.AddField("Input_user", userID);        // 유저 키값 전송

        // 아이디 값 송신
        UnityWebRequest webRequest = UnityWebRequest.Post(InitURL, form);
        yield return webRequest.SendWebRequest();       // 데이터 수신까지 대기

        // 데이터 무결성 체크
        if (webRequest.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;   // 안드로이드 한글 깨지지 않기 위한 처리
            string unitData = enc.GetString(webRequest.downloadHandler.data);   // 서버에서 수신한 데이터 문자열로 변환

            if (unitData.Contains("Get_ItemList_Success..!") == true)
            {
                LoadServerInfo(unitData);
            }
        }
    }

    // 지금은 탱크 카운트 숫자만 받자
    void LoadServerInfo(string strJson)
    {
        // 수신 받은 데이터가 불완전하면 return
        if (strJson.Contains("Data does not exist.") == true)
            return;

        // 유저 유닛 리스트 클리어
        userUnitInfoList.Clear();

        // 지금은 유저가 보유한 유닛의 숫자만 받아오자
        var N = JSON.Parse(strJson);

        //==============================================
        // 여기에 유저 정보를 리스트로 받는 부분 추가해야함
        // 판단에 필요한 정보 ==  1순위 유저의 고유 아이디
        //                      2순위 공격용인지 방어용인지
        //                      3순위 구매했는지 안했는지
        //                      이런 검사들을 모두 거친 후 ItemUsable 값을 추출한다.
        //
        // ↑↑↑↑↑ 위 과정을 모두 서버에서 검사했음...!!!
        // 결국 생성된 JSON은 내가 구매했고 공격팀인 경우에만 추출됨
        //=============================================

        // 유저 정보를 리스트로 받아온다.
        for (int i = 0; i < N.Count; i++)
        {
            UnitInfo unitInfo = new UnitInfo();
            unitInfo.itemNo = N[i]["ItemNo"].AsInt;
            unitInfo.itemName = N[i]["ItemName"];
            unitInfo.itemLevel = N[i]["Level"].AsInt;
            unitInfo.isBuy = N[i]["isBuy"].AsChar;
            unitInfo.posX = N[i]["PosX"].AsFloat;
            unitInfo.posY = N[i]["PosY"].AsFloat;
            unitInfo.itemKind = N[i]["KindOfItem"];
            unitInfo.itemUsable = N[i]["ItemUsable"].AsInt;
            unitInfo.isAttack = N[i]["isAttack"].AsChar;
            userUnitInfoList.Add(unitInfo);
            //unitInfo.UnitPrint();
        }

        // pool의 인덱스에 맞는 유닛 유형 (인덱스 0 == 유닛유형1) 
        for (int i = 0; i < userUnitInfoList.Count; i++)
        {
            // 유닛 유형 1인경우 == Limit 인덱스의 0번 (근데 아직 KindItem의 정확한 입력값을 모름)
            if (userUnitInfoList[i].itemKind == "unit1")
            {
                UnitObjPool.Inst.tankCountLimit[0] = userUnitInfoList[i].itemUsable;
            }

            // 유닛 유형 2인 경우 .....
            else if (userUnitInfoList[i].itemKind == "unit2")
            {
                UnitObjPool.Inst.tankCountLimit[1] = userUnitInfoList[i].itemUsable;
            }

            // 유닛 유형 3인 경우 ....
            else if (userUnitInfoList[i].itemKind == "unit3")
            {
                UnitObjPool.Inst.tankCountLimit[2] = userUnitInfoList[i].itemUsable;
            }

            // 유닛 유형 4인 경우 ....
            else if (userUnitInfoList[i].itemKind == "unit4")
            {
                UnitObjPool.Inst.tankCountLimit[3] = userUnitInfoList[i].itemUsable;
            }

            // 유닛 유형 5인 경우 ....
            else if (userUnitInfoList[i].itemKind == "unit5")
            {
                UnitObjPool.Inst.tankCountLimit[4] = userUnitInfoList[i].itemUsable;
            }
        }
    }

    // UserUnit 클래스에서 enum UnitKind 를 enum TankType과 일치시켜주세요....!
    // AttackUnit을 리스트로 받아서 가지고 있는게 매우 현명할 듯...
    // 공격 유닛을 글로벌에서 받아오는 함수
    private void LoadAttackUnit()
    {
        for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
        {
            // 내가 구매했고 공격 아이템인지 확인
            if (GlobalValue.m_AttUnitUserItem[i].m_isBuy == 1 && GlobalValue.m_AttUnitUserItem[i].m_unitType == UnitType.Att)
            {
                // ↓↓↓↓↓↓ 이거 꼭 확인해야함
                // UnitKind 의 Unit_0 == NormalTank 라면....
                if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == Unitkind.Unit_0)
                    UnitObjPool.Inst.tankCountLimit[0] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == Unitkind.Unit_1)
                    UnitObjPool.Inst.tankCountLimit[1] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == Unitkind.Unit_2)
                    UnitObjPool.Inst.tankCountLimit[2] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == Unitkind.Unit_3)
                    UnitObjPool.Inst.tankCountLimit[3] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;

                else if (GlobalValue.m_AttUnitUserItem[i].m_unitkind == Unitkind.Unit_4)
                    UnitObjPool.Inst.tankCountLimit[4] = GlobalValue.m_AttUnitUserItem[i].ItemUsable;
            }
        }
    }

}
