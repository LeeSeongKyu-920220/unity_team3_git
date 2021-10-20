using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomMgr : MonoBehaviour
{
    [Header("======= GameObject =======")]
    public GameObject m_UserSellMap_Panel = null;
    public GameObject m_TowerSet_Panel = null;
    public GameObject[] m_MAP_OBJ = null;
    
    [Header("======= Button =======")]
    public Button m_MapSet_Btn = null;
    public Button m_UserSellMapCloseBtn = null;
    public Button m_MapSetCloseBtn = null;
    public Button m_GoInGame_Btn = null; //임시 변수
    public Button[] m_UserSellMap_Btn;
    public Button m_MapSetDone_Btn = null;

    public static bool m_MapSetDoneCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        if (m_MapSet_Btn != null)
            m_MapSet_Btn.onClick.AddListener(() => 
            {
                m_UserSellMap_Panel.SetActive(true);
            }); 

        if(m_UserSellMapCloseBtn != null)
            m_UserSellMapCloseBtn.onClick.AddListener(() =>
            {
                m_UserSellMap_Panel.SetActive(false);
            });

        if(m_MapSetCloseBtn != null)
            m_MapSetCloseBtn.onClick.AddListener(() =>
            {
                m_TowerSet_Panel.SetActive(false);
            });

        if (m_GoInGame_Btn != null)
            m_GoInGame_Btn.onClick.AddListener(() => 
            {
                if (m_MapSetDoneCheck == true)
                {
                    if(GlobarValue.g_UserMap == UserMap.MAP1)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                    else if (GlobarValue.g_UserMap == UserMap.MAP2)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("InGameMap2");
                }
                else
                    Debug.Log("세팅된 맵정보가 없습니다.");
            });

        for (int i = 0; i < m_UserSellMap_Btn.Length; i++)
        {
            int m_Index = i; 
            m_UserSellMap_Btn[i].onClick.AddListener(() => 
            {
                UserSellMapBtnClick(m_Index);
            });
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UserSellMapBtnClick(int index)
    {
        GlobarValue.g_UserMap = (UserMap)index;
        GlobarValue.g_SpawnPoint = null;
        GlobarValue.g_TowerType = null;

        if (m_UserSellMap_Panel != null)
        {
            m_UserSellMap_Panel.SetActive(true);
            m_TowerSet_Panel.SetActive(true);
            m_UserSellMap_Panel.SetActive(false);
            for(int i = 0; i < m_MAP_OBJ.Length; i++)
            {
                if (i == index)
                    m_MAP_OBJ[i].SetActive(true);
                else
                    m_MAP_OBJ[i].SetActive(false);
            }
            GlobarValue.UserMapSetting(GlobarValue.g_UserMap);
        }
    }
}
