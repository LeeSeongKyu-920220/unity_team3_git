using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TowerState
{
    Tracking,
    Attack,
    Die,
    Count
}

public class TowerCtrl_Team : MonoBehaviour
{
    [HideInInspector] public TowerState m_TowerState = TowerState.Tracking;

    //-----터렛 구현을 위한 변수
    [Header("========= Turret =========")]
    public GameObject m_Turret = null;
    public GameObject m_BulletSpwanPoint = null;

    //---타겟과의 거리 계산용
    public List<GameObject> m_TargetList = new List<GameObject>();
    Vector3 m_MoveDir = Vector3.zero;
    Vector3 m_CacVLen = Vector3.zero;

    public GameObject m_TargetObj = null;      //타겟 오브젝트
    float m_AttackDelayTime = 0.2f;            //공격 딜레이 시간
    float m_TaggetDistance = float.MaxValue;
    float m_RotSpeed = 4.0f;                    // 회전 속도
    //---타겟과의 거리 계산용

    //-----터렛 구현을 위한 변수

    //-----타워의 스텟을 담당하는 변수
    float m_HP = 0.0f;              //타워의 체력
    float m_AttackRate = 0.0f;      //타워의 공격력
    string m_TowerName = "";        //타워의 이름(종류)

    [Header("========= UI =========")]
    public Text m_TowerNameTxt = null;
    public Image m_HPImg = null;
    //-----타워의 스텟을 담당하는 변수

    public static int m_TargetCount = 0;

    [HideInInspector] public string g_Message = "";

    // ------------------- 진진원
    [SerializeField] GameObject m_BulletObj = null;
    GameObject m_Effobj = null;
    Vector3 m_EffPos = Vector3.zero;
    Vector3 m_BulletPos = Vector3.zero;
    [SerializeField] GameObject m_EffPosObj = null;

    [HideInInspector]public int m_TowerNumber = 0;
    public TowerType m_TowerType = TowerType.Emp_Tower;

    // Start is called before the first frame update
    void Start()
    {

    }

    // ------------------ 진진원
    // Update is called once per frame
    void Update()
    {
        if (m_TowerState == TowerState.Tracking) //대기 모드
        {
            Target_Tracking();
        }
        if (m_TowerState == TowerState.Count) //타겟모드
        {
            Target_Choice();
        }
        if (m_TowerState == TowerState.Attack) //공격 모드
        {
            Target_Attack();
        }
    }

    void TakeDamge(GameObject _Unit)
    {

    }

    void Attack_Eff()
    {

    }

    void Tower_Broken()
    {

    }

    void Target_Attack() //공격을 담당하는 함수
    {
        //공격유닛에게 데미지를 주는 부분(여기는 비워주세요. 나중에 합칠때 구현할 부분입니다.)
        if (m_TargetList[0] == null)
        {
            m_TargetList.RemoveAt(0);           //m_TargetList에서 0번째를 삭제
            m_TowerState = TowerState.Count;    //타겟모드로 전환
            return;
        }

        // ------------------------------------- 진진원
        if (m_TargetObj == null)
            return;

        m_CacVLen = m_TargetObj.transform.position - this.transform.position;
        m_CacVLen.y = 0.0f;
        m_MoveDir = m_CacVLen.normalized;
        Quaternion a_TargetRot = Quaternion.LookRotation(m_MoveDir);
        m_Turret.transform.rotation = Quaternion.Slerp(m_Turret.transform.rotation, a_TargetRot, Time.deltaTime * m_RotSpeed);

        //  데미지를 입히는 부분을 테스트하기 위해 구현하였습니다.
        m_AttackDelayTime -= Time.deltaTime;

        if (m_AttackDelayTime <= 0.0f)
        {
            if (m_TowerType == TowerType.MachineGun_Tower || m_TowerType == TowerType.Missile_Tower)
            {// 총알 생성
                m_AttackDelayTime = 1.5f;
                GameObject a_Bullet = Instantiate(m_BulletObj);
                GameObject a_BulletGroup = GameObject.Find("BulletGroup");
                a_Bullet.transform.SetParent(a_BulletGroup.transform, false);
                a_Bullet.transform.position = m_BulletSpwanPoint.transform.position;
                a_Bullet.transform.LookAt(m_TargetObj.transform);
                a_Bullet.TryGetComponent(out Bullet a_RefBullet);

                m_Effobj = EffectPool.Inst.GetEffectObj("WFX_Explosion_Small", Vector3.zero, Quaternion.identity);
                a_RefBullet.TargetObj = m_TargetObj;
                m_EffPos = m_EffPosObj.transform.position;
                m_Effobj.transform.position = m_EffPos + (-m_CacVLen.normalized * 0.93f);
                m_Effobj.transform.LookAt(m_EffPos + (m_CacVLen.normalized * 2.0f));
            }
            else if(m_TowerType == TowerType.Super_MachineGun_Tower)
            {
                m_AttackDelayTime = 0.3f;
                
                GameObject a_Bullet = Instantiate(m_BulletObj);
                GameObject a_BulletGroup = GameObject.Find("BulletGroup");

                a_Bullet.transform.SetParent(a_BulletGroup.transform, false);
                a_Bullet.transform.position = m_BulletSpwanPoint.transform.position;
                a_Bullet.transform.LookAt(m_TargetObj.transform);
                a_Bullet.TryGetComponent(out Bullet a_RefBullet);
                a_RefBullet.TargetObj = m_TargetObj;
                EffectPool.Inst.GetEffectObj("FX_Fire_01", m_EffPosObj.transform.position, Quaternion.identity);
            }
            else if (m_TowerType == TowerType.Emp_Tower)
            {
                m_AttackDelayTime = 2.0f;

                m_Effobj = EffectPool.Inst.GetEffectObj("LaserImpactPFX", Vector3.zero, Quaternion.identity);
                m_EffPos = m_EffPosObj.transform.position;
                m_Effobj.transform.position = m_EffPos + (-m_CacVLen.normalized * 0.93f);
                m_Effobj.transform.LookAt(m_EffPos + (m_CacVLen.normalized * 2.0f));
            }


        }
    }

    void Target_Tracking() //평소 타워의 상태
    {
        m_Turret.transform.Rotate(Vector3.up, 150.0f * Time.deltaTime);
    }

    void Target_Choice()
    {
        if (m_TargetList.Count == 0)
        {
            m_TowerState = TowerState.Tracking;
            return;
        }

        for (int i = 0; i < m_TargetList.Count; i++)
        {
            if (m_TargetList[i] == null)
                continue;

            Vector3 a_Distance = m_TargetList[i].transform.position - this.transform.position;
            float a_TargetDis = a_Distance.magnitude;
            if (a_TargetDis < m_TaggetDistance)
            {
                m_TaggetDistance = a_TargetDis;
                //--------------------------KTH 추가
                m_TowerState = TowerState.Attack;
                //--------------------------KTH 추가
                m_TargetObj = m_TargetList[i];
            }
        }

    }

    private void OnTriggerEnter(Collider other) //공격거리안으로 적이 들어왔는지 판단하는 함수
    {
        // m_TargetList에 추가
        if (other.tag == "TANK")
        {
            m_TargetList.Add(other.gameObject);

            if (this.gameObject.name == "SuperTower_0_Size1")
            {
                Debug.Log("현재 공격거리내 타겟수 : " + m_TargetList.Count);
                Debug.Log("현재 공격거리내 1번째 타켓 : " + m_TargetList[0].name);
            }

            if(m_TargetList.Count == 1)
            {
                m_TargetObj = m_TargetList[0].gameObject;
                m_TowerState = TowerState.Attack;
            }
            //--------------------------KTH 추가
            //콜라이더의 충돌한 Enemy 수가 0이상일 때 타겟모드
            //m_TowerState = TowerState.Count;
            //--------------------------KTH 추가
        }

        m_TargetCount = m_TargetList.Count;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TANK")
        {
            int _index = DeletTankListCheck(other.gameObject);
            
            for(int i = 0; i < m_TargetList.Count; i++)
            {
                MoveTank a_MoveTank = m_TargetList[i].GetComponent<MoveTank>();
                if(_index == a_MoveTank.m_TankNumber)
                {
                    m_TargetList.RemoveAt(i);
                    if(m_TargetList.Count != 0)
                    {
                        m_TargetObj = m_TargetList[0].gameObject;
                        m_TowerState = TowerState.Attack;
                    }
                    else                         //리스트 삭제 후 List가 존재하지않는다면.
                    {
                        m_TowerState = TowerState.Tracking;
                    }
                    //--------------------------KTH 추가

                    break;
                }
            }

            m_TargetCount = m_TargetList.Count;
        }
    }

    int DeletTankListCheck(GameObject _obj)
    {
        MoveTank a_MoveTank = _obj.GetComponent<MoveTank>();
        return a_MoveTank.m_TankNumber;
    }

}
