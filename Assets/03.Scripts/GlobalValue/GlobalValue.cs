using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 기타 static 하게 관리하게 될 변수들 클래스
/// </summary>
public class GlobalValue : MonoBehaviour
{
    // 글로벌 Value DB 인스턴스 싱글턴 선언
    private static GlobalValue DBinstance;
    public static GlobalValue GetInstance()
    {
        if (!DBinstance)
        {
            DBinstance = GameObject.FindObjectOfType(typeof(GlobalValue)) as GlobalValue;
            if (!DBinstance)
            {
                GameObject container = new GameObject();
                container.name = "DBconnector";
                DBinstance = container.AddComponent(typeof(GlobalValue)) as GlobalValue;
            }
        }
        return DBinstance;
    }

    #region 아이템 증가에 따른 증가 수치량 변수 모음
    // 원래는 DB에 넣고 가져와서 조율하는 것이 맞다.
    public static int UnitIncreValue = 10;  // 지금은 공통적으로 증가하게 해놓고, 추후 분리

    #endregion

    #region 공격 아이템 글로벌 변수 부분

    // 공격 아이템 가져오는 부분 URL
    string GetUserAttItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/AttGetItem.php";
    string GetAttItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/AttGetDefaultItem.php";

    #region 상점 부분 아이템 저장 및 초기화 부분

    public static List<UserUnit> m_AttUnitUserItem = new List<UserUnit>();  // 아이템 정보를 받을 변수
    public static List<UserUnit> m_DefUnitItem = new List<UserUnit>();  // 아이템 정보를 받을 변수

    public static bool isAttDataInit = false;                           // 아이템 데이터베이스 응답 여부 확인
    public static bool GetAttDataLock = false;

    public void InitStoreAttData()
    {
        // 여기에서 DB에 정보를 가져온다.
        if (GetAttDataLock == false) 
        {
            StartCoroutine(GetStoreAttData());
        }
    }

    IEnumerator GetStoreAttData() 
    {
        GetAttDataLock = true;  // 네트워크 중복 안되는 조치

        WWWForm form = new WWWForm();
        //form.AddField("Input_ID", MyInfo.m_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_ID", "1", System.Text.Encoding.UTF8);        // 테스트 코드
        form.AddField("Input_itemType", "1", System.Text.Encoding.UTF8);    // 공격 아이템만 가져오기
        UnityWebRequest request = UnityWebRequest.Post(GetUserAttItemUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(request.downloadHandler.data);

            if (a_ReStr.Contains("Get_Item_Success~") == true)
            {
                // 확인 부분
                if (a_ReStr.Contains("UnitList") == false) 
                {
                    isAttDataInit = false;   // 데이터 저장 실패
                    yield break;
                }
                
                // 파싱된 결과를 바탕으로 아이템 초기화
                UserUnit a_UserUt;
                var N = JSON.Parse(a_ReStr);
                m_AttUnitUserItem.Clear();
                // 아이템을 전체적으로 초기화한다.
                // 먼저 JSON에 저장되어 있던 정보 초기화
                for (int i = 0; i < N["UnitList"].Count; i++)
                {                    
                    int itemNo  = N["UnitList"][i]["ItemNo"].AsInt;
                    string itemName = N["UnitList"][i]["ItemName"];
                    int Level = N["UnitList"][i]["Level"].AsInt;
                    int isBuy = N["UnitList"][i]["isBuy"].AsInt;
                    int KindOfItem = N["UnitList"][i]["KindOfItem"].AsInt - 1;
                    int ItemUsable = N["UnitList"][i]["ItemUsable"].AsInt;

                    int UnitAtt = N["UnitList"][i]["UnitAttack"].AsInt;
                    int UnitDef = N["UnitList"][i]["UnitDefence"].AsInt;
                    int UnitHP = N["UnitList"][i]["UnitHP"].AsInt;
                    float UnitAttSpeed = N["UnitList"][i]["UnitAttSpeed"].AsFloat;
                    float UnitMoveSpeed = N["UnitList"][i]["UnitAttack"].AsFloat;
                    int Unitprice = N["UnitList"][i]["UnitPrice"].AsInt;
                    int UnitUprice = N["UnitList"][i]["UnitUpPrice"].AsInt;                    
                    
                    a_UserUt = new UserUnit();
                    a_UserUt.m_UnitNo = itemNo;
                    a_UserUt.m_Name = itemName;
                    a_UserUt.m_Level = Level;
                    a_UserUt.m_isBuy = isBuy;
                    a_UserUt.m_unitkind = (Unitkind)KindOfItem;                    
                    a_UserUt.m_unitType = UnitType.Att;
                    a_UserUt.ItemUsable = ItemUsable;

                    a_UserUt.m_Att = UnitAtt + Level * UnitIncreValue;
                    a_UserUt.m_Def = UnitDef + Level * UnitIncreValue;
                    a_UserUt.m_Hp = UnitHP + Level * UnitIncreValue;
                    a_UserUt.m_AttSpeed = UnitAttSpeed;
                    a_UserUt.m_Speed = UnitMoveSpeed;
                    a_UserUt.m_Price = Unitprice;
                    a_UserUt.m_UpPrice = UnitUprice;                    

                    m_AttUnitUserItem.Add(a_UserUt);
                }//for (int i = 0; i < N["UnitList"].Count; i++)

                bool isInsert = false;
                UserUnit a_UserUtNew;
                // 유저가 가지고 있지 않은 아이템은 초기 아이템 정보로 초기화한다.
                // 다시 URL 통신
                WWWForm form2 = new WWWForm();
                form2.AddField("Input_itemType", "1", System.Text.Encoding.UTF8);    // 공격 아이템만 가져오기
                UnityWebRequest request2 = UnityWebRequest.Post(GetAttItemUrl, form2);
                yield return request2.SendWebRequest();

                if (request2.error == null)
                {
                    string a_ReStr2 = enc.GetString(request2.downloadHandler.data);
                    var N2 = JSON.Parse(a_ReStr2);
                    
                    for (int i = 0; i < N2["UnitList"].Count; i++)
                    {
                        // 2중 for문이긴 한데, 여기서 중복 체크를 한번 한다.
                        foreach (var tpitem in m_AttUnitUserItem)
                        {
                            if (tpitem.m_unitkind == (Unitkind)i)
                                isInsert = true;
                        }

                        if (isInsert == true)
                        {
                            isInsert = false;
                            continue;
                        }
                        else
                        {
                            isInsert = false;
                            a_UserUtNew = new UserUnit();

                            string UnitName = N2["UnitList"][i]["UnitName"];
                            int UnitAtt = N2["UnitList"][i]["UnitAttack"].AsInt;
                            int UnitDef = N2["UnitList"][i]["UnitDefence"].AsInt;
                            int UnitHP = N2["UnitList"][i]["UnitHP"].AsInt;
                            float UnitAttSpeed = N2["UnitList"][i]["UnitAttSpeed"].AsFloat;
                            float UnitMoveSpeed = N2["UnitList"][i]["UnitAttack"].AsFloat;
                            int Unitprice = N2["UnitList"][i]["UnitPrice"].AsInt;
                            int UnitUprice = N2["UnitList"][i]["UnitUpPrice"].AsInt;
                            int UnitUsable = N2["UnitList"][i]["UnitUseable"].AsInt;

                            a_UserUtNew = new UserUnit();
                            a_UserUtNew.m_UnitNo = 0; //기본값
                            a_UserUtNew.m_Name = UnitName;
                            a_UserUtNew.m_Level = 1;    // 구매 안함
                            a_UserUtNew.m_isBuy = 0;
                            a_UserUtNew.m_unitkind = (Unitkind)i;
                            a_UserUtNew.m_unitType = UnitType.Att;
                            a_UserUtNew.ItemUsable = UnitUsable;
                            a_UserUtNew.m_Att = UnitAtt;
                            a_UserUtNew.m_Def = UnitDef;
                            a_UserUtNew.m_Hp = UnitHP;
                            a_UserUtNew.m_AttSpeed = UnitAttSpeed;
                            a_UserUtNew.m_Speed = UnitMoveSpeed;
                            a_UserUtNew.m_Price = Unitprice;
                            a_UserUtNew.m_UpPrice = UnitUprice;                            

                            m_AttUnitUserItem.Add(a_UserUtNew);
                        }
                        
                    }//for (int i = 0; i < N["UnitList"].Count; i++)
                }//if (request2.error == null) 
                else 
                {
                    isAttDataInit = false; // 데이터 불러오기 실패
                }

                isAttDataInit = true;   // 데이터 저장 성공
                
                //// 성공했는지 로그 찍어보기
                //for (int i = 0; i< m_AttUnitUserItem.Count;i++) 
                //{
                //    Debug.Log($"{m_AttUnitUserItem[i].m_Name}\n");
                //}
            }
            else 
            {
                isAttDataInit = false;   // 데이터 저장 실패
            }

            GetAttDataLock = false; // 성공 시 네트워크 상태 해제
        }//if (request.error == null)
        else
        {
            Debug.Log(request.error);
            isAttDataInit = false;   // 데이터 저장 실패
            GetAttDataLock = false; // 실패했을 시 네트워크 상태 해제
        }
    }

    #endregion    

    #endregion

}
