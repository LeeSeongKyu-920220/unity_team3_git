using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMgr : MonoBehaviour
{
    [Header("====== GameObject ======")]
    public GameObject[] SpawnPointGroup;
    public GameObject[] m_Tower;

    MeshRenderer[] m_TowerSpawnPointList;
    List<GameObject> m_SpawnPointList = new List<GameObject>();
    int m_GroupIndex = 0;

    [Header("====== ReSpawn ======")]
    public GameObject[] m_SpawnTank = null;
    public GameObject m_TankSpawnPoint = null;
    float m_ReSpawnTime = 3.0f;

    [HideInInspector] public int m_TankNumbers = 0;
    [HideInInspector] public int m_TowerNumbers = 0;

    static public InGameMgr Init = null;
    private void Awake()
    {
        m_GroupIndex = (int)GlobarValue.g_UserMap;
        Init = this;

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_TowerSpawnPointList = SpawnPointGroup[m_GroupIndex].transform.GetComponentsInChildren<MeshRenderer>();
        SetSpawnPoint();
        SpawnTower();
        ReSpwanTank();
    }

    // Update is called once per frame
    void Update()
    {
        if(0.0f < m_ReSpawnTime)
        {
            m_ReSpawnTime -= Time.deltaTime;
            if (m_ReSpawnTime < 0.0f)
            {
                m_ReSpawnTime = 3.0f;
                ReSpwanTank();
            }
        }
    }

    void SetSpawnPoint()
    {
        for(int i = 0; i < m_TowerSpawnPointList.Length; i++)
        {
            //Debug.Log(m_TowerSpawnPointList[i].gameObject);
            m_SpawnPointList.Add(m_TowerSpawnPointList[i].gameObject);
        }
    }

    void SpawnTower()
    {
        int a_pawnNum = 0;

        for(int i = 0; i < GlobarValue.g_SpawnPoint.Length; i++)
        {
            if(GlobarValue.g_SpawnPoint[i] == true)
            {
                GameObject a_Tower = Instantiate(m_Tower[(int)GlobarValue.g_TowerType[a_pawnNum]]);
                a_Tower.transform.position = m_TowerSpawnPointList[i].transform.position;
                TowerCtrl_Team a_WowerCtrl_Team = a_Tower.GetComponent<TowerCtrl_Team>();
                a_WowerCtrl_Team.m_TowerNumber = i;
                a_WowerCtrl_Team.m_TowerType = GlobarValue.g_TowerType[a_pawnNum];
                a_pawnNum++;
            }
        }
    }

    void ReSpwanTank()
    {
        GameObject a_Tank = Instantiate(m_SpawnTank[(int)GlobarValue.g_UserMap]);
        GameObject a_TankGroup = GameObject.Find("TankGroup");
        MoveTank a_MoveTank = a_Tank.GetComponent<MoveTank>();
        a_MoveTank.m_TankNumber = m_TankNumbers;
        a_Tank.transform.position = m_TankSpawnPoint.transform.position;
        a_Tank.transform.SetParent(a_TankGroup.transform, false);
        m_TankNumbers++;
    }
}
