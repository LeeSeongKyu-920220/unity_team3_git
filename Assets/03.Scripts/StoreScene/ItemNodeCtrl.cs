using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemState
{
    Lock,                   //아이템 잠긴 상태
    BeforeBuy,                  //구매 전 상태
    Upgrade,              //아이템 업그레이드 상태
    Active
}

public enum ItemKind
{
    Attack,
    Shield,
}

public class ItemNodeCtrl : MonoBehaviour
{
    //[HideInInspector] public CharType m_CrType = ItemType.CrCount;  //초기화
    [HideInInspector] public ItemState m_ItState = ItemState.Lock;
    

    [Header("ItemInfo")]
    public Button m_ItNodeBtn = null;                   //아이템 노드
    public GameObject m_ItObj = null;                   //아이템 오브젝트
    public Text m_ItUpLevelTxt = null;                  //아이템 레벨
    public Text m_ItNameTxt = null;                     //아이템 이름
    public Text m_ItPriceTxt = null;                    //아이템 가격

    // Start is called before the first frame update
    void Start()
    {
        if (m_ItNodeBtn != null)
            m_ItNodeBtn.onClick.AddListener(()=> 
            {
                StoreMgr a_StoreMgr = null;
                GameObject a_StoreObj = GameObject.Find("Store_Mgr");
                if (a_StoreObj != null)
                    a_StoreMgr = a_StoreObj.GetComponent<StoreMgr>();


                string a_UpLevel = m_ItUpLevelTxt.text;
                string a_ItName = m_ItNameTxt.text;
                string a_ItAbility = "100\n+200\n+300\n+400\n+500\n5";
                string a_ItPrice = m_ItPriceTxt.text;

                //if (a_StoreMgr != null)
                //    a_StoreMgr.SelItem(a_UpLevel, a_ItName, a_ItAbility, a_ItPrice);
            }); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    //void SetInitData(Unitkind a_UnitKind)
    //{
    //    m_ItUpLevelTxt.text = "0/5";
    //    m_ItNameTxt.text = GlobalValue.g_UnitDefList[(int)a_UnitKind].m_ItName;
    //    m_ItPriceTxt.text = GlobalValue.g_UnitDefList[(int)a_UnitKind].m_ItPrice + " G";
    //}

    //void SetItemState(ItemState a_ItState, string ItName, int a_ItPrice, int a_ItLevel = 0)
    //{
    //    m_ItState = a_ItState;

    //    if(m_ItState == ItemState.Lock)                 //비활성화 상태 (구매불가)
    //    {
    //    }
    //    else if (m_ItState == ItemState.BeforeBuy)      //구매가능 상태 (첫 구매)
    //    {
    //    }
    //    else if (m_ItState == ItemState.Active)         //활성화 상태 (업그레이드)
    //    {
    //    }

    //}
}
