using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttUnitState
{
    BeforeBuy = 0,
    Active
}

public class AttItNodeCtrl : MonoBehaviour
{
    internal Unitkind m_Unitkind = Unitkind.Unit_0;
    internal AttUnitState m_UnitState = AttUnitState.BeforeBuy;

    public Button m_AttBtn;            // 자기 자신의 버튼
    public Text m_NameText;            // 이름 텍스트
    public Image m_UnitIconImg;        // 아이콘
    public Text m_UnitLevelText;       // 유닛 레벨(아이템 구매 체크)
    public Text m_UnitAttText;         // 유닛 공격력
    public Text m_UnitHPText;          // 유닛 피통
    public Text m_UnitPriceText;       // 유닛 가격

    // 해당 노드에 정보를 담을 맴버변수들
    int m_ItemNo = 0;
    string m_Name = "";
    int m_Price = 0;
    int m_UpPrice = 0;
    int m_Level = 0;
    int m_Hp = 0;
    int m_Att = 0;
    int m_Def = 0;
    float m_AttSpeed = 0;
    float m_Speed = 0;
    int m_Moveable = 0;

    // 게임 자세히 보기 시 사용할 GameObject
    GameObject ParentsObj;
    GameObject AttSelNode;

    // Start is called before the first frame update
    void Start()
    {
        if (m_AttBtn != null)
            m_AttBtn.onClick.AddListener(() =>
            {
                ParentsObj = GameObject.Find("SelItemViewPoint");

                if (ParentsObj != null)
                {
                    AttSelNode = ParentsObj.transform.Find("AttSelItem").gameObject;
                    if (AttSelNode != null)
                    {
                        AttSelNode.GetComponent<AttSelNodeCtrl>().ItemSel(m_Name, m_Level,
                            m_Hp, m_Att, m_Def, m_AttSpeed, m_Speed, m_Moveable, m_UnitState, m_Price,
                            m_UpPrice, (int)m_Unitkind + 1, 1, m_Moveable, m_ItemNo); // 유닛 ID는 Enum이 0부터 시작하기 때문에 +1을 해준다.
                        AttSelNode.SetActive(true);
                    }
                }
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitData(Unitkind a_UnitType)
    {
        if (a_UnitType < Unitkind.Unit_0 || Unitkind.UnitCount <= a_UnitType)
            return;

        m_Unitkind = a_UnitType;
        //m_UnitIconImg.sprite = GlobalValue.m_ItDataList[(int)a_ItType].m_IconImg; //<- 이미지 넣는 곳, 나중에 리소스 받으면 넣을 것
        //m_ItIconImg.GetComponent<RectTransform>().sizeDelta = new Vector2(GlobalValue.m_ItDataList[(int)a_ItType].m_IconSize.x * 135.0f, 135.0f);

        // 맴버 변수 초기화
        m_Name = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Name;
        m_Price = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Price;
        m_UpPrice = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_UpPrice;
        m_Level = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Level;
        m_Att = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Att;
        m_Hp = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Hp;
        m_Def = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Def;
        m_AttSpeed = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_AttSpeed;
        m_Speed = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Speed;
        m_Moveable = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].ItemUsable;
        m_ItemNo = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_UnitNo;

        // UI 관련 제어
        m_NameText.text = GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Name;
        m_UnitLevelText.text = $"Level : {GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Level}";
        m_UnitPriceText.text = $"{GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Price}";
        m_UnitAttText.text = $"유닛 공격력 : {GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Att}";
        m_UnitHPText.text = $"유닛 HP : {GlobalValue.m_AttUnitUserItem[(int)a_UnitType].m_Hp}";
    }

    public void SetState(AttUnitState a_UnitState, int a_Level = 1)
    {
        m_UnitState = a_UnitState;
        m_Level = a_Level;

        if (m_UnitState == AttUnitState.BeforeBuy) // 처음 구매 상태
        {
            m_UnitPriceText.text = m_Price.ToString();
            m_UnitIconImg.color = new Color32(255, 255, 255, 120); //new Color32(110, 110, 110, 255);
            m_UnitLevelText.text = "Buy!!"; //여기서는 그냥 기본 가격            
        }
        else if (m_UnitState == AttUnitState.Active) // 구매를 한 상태
        {
            m_UnitPriceText.text = m_UpPrice.ToString();
            m_UnitIconImg.color = new Color32(255, 255, 255, 255); //new Color32(110, 110, 110, 255);
            m_UnitLevelText.text = $"Level : {m_Level}";
            m_UnitAttText.text = $"유닛 공격력 : {m_Att + m_Level * GlobalValue.UnitIncreValue}";
            m_UnitHPText.text = $"유닛 HP : {m_Hp + m_Level * GlobalValue.UnitIncreValue}";
        }
    }//public void SetState(CrState a_CrState, int a_Price, int a_Lv = 0)
}
