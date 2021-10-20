using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoreMgr : MonoBehaviour
{
    // 공격과 방어 UI 분리용 Enum
    public enum StoreState
    {
        Attack,
        Defence
    }

    public StoreState storeState = StoreState.Attack;

    public Button m_BackBtn = null;

    [Header("Attack")]
    public RawImage m_AttackBack = null;
    public Button m_AttackStoreBtn = null;

    [Header("Shield")]
    public RawImage m_ShieldBack = null;
    public Button m_ShieldStoreBtn = null;

    [Header("Buy")]
    public Button m_BuyBtn = null;
    public GameObject m_DlgBox = null;

    [Header("ItemSel")]
    public GameObject ItemSel;

    [Header("Common")]
    public GameObject AttCheck;
    public GameObject DefCheck;

    #region 공격 부분 추가 맴버변수

    [Header("AttPart")]
    public GameObject m_AttItem_NodeObj;
    public GameObject m_AttItem_ScrollContent;

    // 공격 아이템 선택 여부
    bool isAttSel = false;
    bool isAttfirst = true;

    GameObject m_AttItemObj = null;
    AttItNodeCtrl[] m_AttItemObjs = null;
    AttItNodeCtrl m_AttNode = null;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.GetInstance().InitStoreAttData();

        //로비이동 버튼
        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                //로비로 이동
                SceneManager.LoadScene("LobbyScene");
            });

        //공격상점
        if (m_AttackStoreBtn != null)
            m_AttackStoreBtn.onClick.AddListener(() =>
            {
                isAttSel = true;
                storeState = StoreState.Attack;

                if (m_AttackBack != null)
                    m_AttackBack.gameObject.SetActive(true);

                if (m_ShieldBack != null)
                    m_ShieldBack.gameObject.SetActive(false);
            });

        //방어상점
        if (m_ShieldStoreBtn != null)
            m_ShieldStoreBtn.onClick.AddListener(() =>
            {
                isAttSel = false;
                storeState = StoreState.Defence;

                if (m_ShieldBack != null)
                    m_ShieldBack.gameObject.SetActive(true);

                if (m_AttackBack != null)
                    m_AttackBack.gameObject.SetActive(false);
            });

        //구매버튼
        if (m_BuyBtn != null)
            m_BuyBtn.onClick.AddListener(() =>
            {
                if (m_DlgBox != null)
                    m_DlgBox.SetActive(true);
            });

        #region 공격 부분 start 함수 추가부분

        isAttSel = true;   // 초기 부분 // 테스트 용으로

        #endregion        
    }

    // Update is called once per frame
    void Update()
    {
        if (storeState == StoreState.Attack)
        {
            // 공격 선택 시 UI 액션 부분
            AttackUpdate();
        }//if (storeState == StoreState.Attack)
        else if(storeState == StoreState.Defence)
        {
            AttCheck.SetActive(false);
            DefCheck.SetActive(true);
        }
    }

    #region 공격 유닛 관련 함수들 모음

    void AttackUpdate()
    {
        InitAttSetting();   // 처음 아이템 노드 배치하는 부분 한번만 돌고 만다.

        if (isAttSel == true)
        {            
            AttCheck.SetActive(true);
            DefCheck.SetActive(false);
        }        
    }//void AttackUpdate() 

    void InitAttSetting()
    {
        if (isAttfirst == true && GlobalValue.isAttDataInit == true) // DB 응답이 왔을 경우
        {
            isAttfirst = false;

            for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
            {
                m_AttItemObj = (GameObject)Instantiate(m_AttItem_NodeObj);
                m_AttNode = m_AttItemObj.GetComponent<AttItNodeCtrl>();
                m_AttNode.InitData(GlobalValue.m_AttUnitUserItem[i].m_unitkind);
                m_AttNode.SetState((AttUnitState)GlobalValue.m_AttUnitUserItem[i].m_isBuy, GlobalValue.m_AttUnitUserItem[i].m_Level);
                m_AttItemObj.transform.SetParent(m_AttItem_ScrollContent.transform, false);
            }
        }//if (isAttfirst == true && GlobalValue.isAttDataInit == true)
    }//void InitAttSetting()

    public void ResetAttState()
    {
        if (m_AttItem_ScrollContent != null)
        {
            if (m_AttItemObjs == null || m_AttItemObjs.Length <= 0)
                m_AttItemObjs = m_AttItem_ScrollContent.GetComponentsInChildren<AttItNodeCtrl>();
        }//if (m_AttItem_ScrollContent != null)

        for (int i = 0; i < GlobalValue.m_AttUnitUserItem.Count; i++)
        {
            if (m_AttItemObjs[i].m_Unitkind != GlobalValue.m_AttUnitUserItem[i].m_unitkind)
                continue;

            m_AttItemObjs[i].SetState((AttUnitState)GlobalValue.m_AttUnitUserItem[i].m_isBuy, GlobalValue.m_AttUnitUserItem[i].m_Level);
        }//for (int i = 0;i< GlobalValue.m_AttUnitUserItem.Count;i++)
    }

    #endregion

}
