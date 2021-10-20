using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomSpawnMgr : MonoBehaviour
{
    [HideInInspector] public Button[] m_SpawnBtn = null;
    [HideInInspector] public bool[] m_CheckPoint;
    public Button[] m_UserSellTower;
    public GameObject m_SellTower_Panel = null;
    public Button m_Done_Btn = null;

    int m_MaxSpawn = 0;
    int m_SpawnNum = 0;

    int m_BtnNum = 0;
    Image[] a_CheckImg = null;
    Image m_CheckImg = null;
    // Start is called before the first frame update
    void Start()
    {
        m_SpawnBtn = this.transform.GetComponentsInChildren<Button>();
        m_CheckPoint = new bool[m_SpawnBtn.Length];
        m_MaxSpawn = m_SpawnBtn.Length / 2;

        Debug.Log(m_SpawnBtn.Length.ToString());

        for(int i = 0; i < m_CheckPoint.Length; i++)
        {
            m_CheckPoint[i] = false;
        }

        for (int i = 0; i < m_SpawnBtn.Length; i++)
        {
            int index = i;
            m_SpawnBtn[i].onClick.AddListener(() => 
            {
                BtnClick(index); 
            });

        }
        
        for (int i = 0; i < m_UserSellTower.Length; i++)
        {
            int index = i;
            m_UserSellTower[i].onClick.AddListener(() => 
            {
                SellTowerType((TowerType)index);
            });

        }
        
        m_Done_Btn.onClick.AddListener(DoneBtn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BtnClick(int _num)
    {
        if (m_SpawnNum == m_MaxSpawn && m_CheckPoint[_num] == false)
        {
            Debug.Log("더 이상 배치를 할 수 없습니다.");
            return;
        }

        m_BtnNum = _num;
        a_CheckImg = m_SpawnBtn[m_BtnNum].gameObject.transform.GetComponentsInChildren<Image>();
        m_CheckImg = a_CheckImg[1];
        if (m_CheckPoint[m_BtnNum] == false)
        {
            m_SellTower_Panel.SetActive(true);
        }

        else
        {
            m_CheckImg.enabled = false;
            m_CheckPoint[m_BtnNum] = false;
            m_SpawnNum--;
        }
    }

    public void DoneBtn()
    {
        if (m_SpawnNum == 0)
        {
            Debug.Log("세팅된 맵이 없습니다.");
            return;
        }
        MyRoomMgr.m_MapSetDoneCheck = true;
        this.gameObject.transform.parent.gameObject.SetActive(false);
        for (int i = 0; i < m_CheckPoint.Length; i++)
        {
            GlobarValue.g_SpawnPoint[i] = m_CheckPoint[i];
        }
    }

    void SellTowerType(TowerType _Type) 
    {
        if (m_SellTower_Panel == null && m_SellTower_Panel.activeSelf == false)
            return;

        if (m_CheckImg == null)
            return;

        m_CheckImg.enabled = true;
        m_CheckPoint[m_BtnNum] = true;
        GlobarValue.g_TowerType[m_SpawnNum] = _Type;
        m_SpawnNum++;
        m_SellTower_Panel.SetActive(false);
    }
}
