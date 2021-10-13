using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TankCtrl : MonoBehaviour
{
    // 기본 탱크 정보 변수
    TankType m_Type = TankType.Normal;      // 탱크타입
    float moveVelocity = 10.0f;             // 이동속도
    float attRate = 0.0f;                   // 공격 속도
    float curHp = 0.0f;                     // 현재체력
    float maxHp = 0.0f;                     // 최대체력
    float skillCool = 0.0f;                 // 스킬 쿨타임

    // 기본 탱크 정보 변수
    [HideInInspector] public GameObject target_Obj;           // 타겟 오브젝트 저장
    [HideInInspector] public List<GameObject> target_List = new List<GameObject>();  // 타겟 목록 저장
    Vector3 tank_Pos = Vector3.zero;        // 탱크의 좌료 저장
    Vector3 target_Pos = Vector3.zero;      // 타겟의 좌표 저장
    float att_Delay = 0.0f;                 // 공격 딜레이 타이머
    float skill_Delay = 0.0f;               // 스킬 딜레이 타이머
    float turn_Speed = 10.0f;               // 포탑 회전 속도
    public GameObject turret_Obj = null;    // 포탑 오브젝트
    public GameObject fire_Pos = null;      // 발사 위치 오브젝트
    public GameObject bullet_Obj = null;    // 총알 오브젝트
    public GameObject turret_Explo = null;  // 발사 이펙트 오브젝트

    float h, v;

    // </ 길찾기

    // </ 이동 관련 변수
    // </ Picking 관련 변수
    Vector3 moveDir = Vector3.zero;         // 이동 방향
    float rotSpeed = 7.0f;                  // 초당 회전 속도
    bool isMoveOn = false;                  // 이동 On/Off
    public Transform beginTarPos = null;    // 공격 탱크가 인스턴싱 될 때 지정하는 목적지
    Vector3 targetPos = Vector3.zero;       // 목적지
    double moveDurTime = 0.0f;              // 목표지점까지 도착하는데 걸리는 시간
    double addTimeCount = 0.0f;             // 누적 시간 카운트
    Vector3 startPos = Vector3.zero;
    Vector3 cacLenVec = Vector3.zero;
    Quaternion targetRot;
    // Picking 관련 변수 />

    Vector3 m_VecLen = Vector3.zero;
    // 이동 관련 변수 />

    // </ Navigation
    NavMeshAgent navAgent;
    NavMeshPath movePath;
    Vector3 pathEndPos = Vector3.zero;
    int curPathIndex = 1;
    // Navigation />

    // 길찾기 />

    // UI 관련 변수
    public Canvas tank_Canvas = null;
    public Image hp_Img = null;
    // UI 관련 변수

    TankInfo tankInfo = null;

    void Start()
    {
        // 탱크 기본정보 받아오기
        tankInfo = GetComponent<TankInfo>();
        tankInfo.TankInit();
        m_Type = tankInfo.m_Type;
        moveVelocity = tankInfo.speed;
        attRate = tankInfo.attRate;
        maxHp = tankInfo.maxHp;
        curHp = maxHp;
        skillCool = tankInfo.skillCool;
        // 탱크 기본정보 받아오기

        movePath = new NavMeshPath();
        navAgent = this.gameObject.GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        //SetDestination(beginTarPos.position); // 최초 목적지 설정
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(20);

        tank_Pos = this.transform.position;
        tank_Pos.y = 0.0f;

        TankMove();

        if (att_Delay > 0.0f)
            att_Delay -= Time.deltaTime;

        if (skill_Delay > 0.0f)
            skill_Delay -= Time.deltaTime;

        if (target_Obj != null)
        {
            target_Pos = target_Obj.transform.position;
            target_Pos.y = 0.0f;
            Vector3 dir = target_Pos - tank_Pos;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(turret_Obj.transform.rotation, lookRotation, Time.deltaTime * turn_Speed).eulerAngles;
            turret_Obj.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        if(target_List.Count <= 0)
        {
            turret_Obj.transform.rotation = Quaternion.Slerp(turret_Obj.transform.rotation, 
                this.transform.rotation, Time.deltaTime * turn_Speed);
            turret_Obj.transform.localEulerAngles = new Vector3(0.0f, turret_Obj.transform.localEulerAngles.y, 0.0f);

        }
        //NavUpdate(); // 길찾기
        Attack();
        Repair(20); // 리페어 탱크인 경우에만 실행
    }

    void TakeDamage(int a_Damage)
    {
        curHp -= a_Damage;

        if (hp_Img != null)
            hp_Img.fillAmount = curHp / maxHp;

        if (curHp < 0)
            curHp = 0;
    }

    #region ---------- 탱크 이동 부분(임시)

    void TankMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //회전과 이동처리
        this.transform.Rotate(Vector3.up * 150.0f * h * Time.deltaTime);
        this.transform.Translate(Vector3.forward * v * 5.0f * Time.deltaTime);

        if(tank_Canvas != null)
        {
            tank_Canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            Vector3 pos = this.transform.position;
            pos.z -= 1;
            tank_Canvas.transform.position = pos;
        }
    }

    #endregion

    #region ---------- 탱크 공격 부분

    void Attack()
    {
        if (target_List.Count <= 0)
            return;

        if (att_Delay > 0.0f)
            return;

        List<float> target_Dist = new List<float>();
        
        for (int ii = 0; ii < target_List.Count;)
        {
            if (target_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                target_List.Remove(target_List[ii]);    // null 값이 저장되어 있으면 지우기

                if (target_List.Count <= 0)  // null 값을 지워서 리스트가 비어있으면 함수를 빠져 나감
                    return;
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, target_List[ii].transform.position);
                target_Dist.Add(dis);
                ii++;
            }
        }

        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);

        target_Obj = target_List[target_Index];
        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;
        att_Delay = attRate;
        GameObject bullet = Instantiate(bullet_Obj, fire_Pos.transform.position, turret_Obj.transform.rotation);
        bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
        Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
    }

    #endregion



    // 유닛 스킬 구현 부분 ------------------------------------------------------------------------------------------------------------------------------
    void Repair(int repairValue)
    {
        if (m_Type != TankType.Repair)  // 탱크 타입 검사
            return;

        if (skill_Delay > 0.0)          // 스킬 딜레이 검사
            return;

        float skillRange = 5.0f; // 스킬범위 (임시)
        
        GameObject[] allyObjs = GameObject.FindGameObjectsWithTag("Tank"); // 아군 탱크들을 찾음
        
        for(int i =0; i<allyObjs.Length; i++)
        {
            if (allyObjs[i] == gameObject) // 자기자신은 치료하지 않음
                continue;

            if ((allyObjs[i].transform.position - transform.position).magnitude < skillRange) // 스킬 범위 내에 있는지 검사
            {
                allyObjs[i].GetComponent<TankCtrl>().curHp += repairValue; // 체력 회복
                Debug.Log(allyObjs[i].name + "을 " + repairValue + "만큼 수리함");
            }
        }

        skill_Delay = skillCool;
    }
    void Provocation() // 주변의 방어시설들이 자신을 공격하게 만드는 스킬
    {
        if (m_Type != TankType.Solid)
            return;

        if (skill_Delay > 0.0)
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Tower"); // 방어시설들을 찾음
        
        for(int i =0; i < enemies.Length; i++) // 방어시설의 공격거리보다 가까우면 자신을 공격하게 만듦.
        {
            // Tower tower = enemies[i].GetComponenet<Tower>();
            //if((enemies[i].transform.position - transform.position).magnitude < tower.attRange)
            //{
            //    tower.target = this.gameObject;
            //}
        }

    }

    // 유닛 스킬 구현 부분 ------------------------------------------------------------------------------------------------------------------------------

    #region ---------- 배열의 최소값 체크 (제일 가까운 적 체크 용)

    void GetMinCheck(List<float> a_List, out int a_Min)
    {
        float min = a_List[0];
        a_Min = 0;

        for(int ii = 0; ii < a_List.Count; ii++)
        {
            if (min > a_List[ii])
            {
                min = a_List[ii];
                a_Min = ii;
            }
        }
    }

    #endregion
    public void SetDestination(Vector3 a_SetTargetVec)
    {
        //Debug.Log(a_SelectObj);
        // 캐릭터들의 Hp바와 닉네임바 RaycastTarget을 모두 꺼주어야 피킹이 정상작동한다.
        // 그렇지 않으면 if(IsPointerOverUIObject() == false) 에 의해 막히게 된다.
        startPos = this.transform.position; // 출발 위치
        cacLenVec = a_SetTargetVec - startPos; // 현재지점과 목표지점사이의 거리 벡터

        if (cacLenVec.magnitude < 0.5f) // 근거리 피킹 스킵
            return;

        // 네비게이션 메쉬 길찾기를 이용할 때 코드
        float a_PathLen = 0.0f;
        if (MyNavCalcPath(startPos, a_SetTargetVec, ref a_PathLen) == false)
            return;

        a_SetTargetVec.y = this.transform.position.y; // 최종 목표 위치
        targetPos = a_SetTargetVec;                   // 최종 목표 위치
        isMoveOn = true;                              // 이동 OnOff

        moveDir = cacLenVec.normalized;
        // 네비게이션 메시 길찾기를 이용했을 때 거리 계산법
        moveDurTime = a_PathLen / moveVelocity; // 도착하는데 걸리는 시간 = 거리 / 속도
        addTimeCount = 0.0f;
    }

    void NavUpdate()
    {
        // 마우스 피킹 이동
        if (isMoveOn == true)
        {
            // 네비게이션 메시 길찾기를 이용할 때 코드
            isMoveOn = MoveToPath(); // 도착한 경우 false 리턴
        }
    }

    public bool MyNavCalcPath(Vector3 a_StartPos, Vector3 a_TargetPos, ref float a_PathLen)
    {
        // 경로 탐색 함수
        // 피킹이 발생된 상황이므로 초기화 하고 계산한다.
        movePath.ClearCorners(); // 경로 모두 제거
        curPathIndex = 1;        // 진행 인덱스 초기화
        pathEndPos = transform.position;

        if (navAgent == null || navAgent.enabled == false)
        {
            return false;
        }

        if (NavMesh.CalculatePath(a_StartPos, a_TargetPos, -1, movePath) == false)
        {
            // CalculatePath() 함수 계산이 끝나고 정상적으로 instance.final
            // 즉, 목적지까지 계산에 도달했다는 뜻
            // --> p.status == UnityEngine.AI.NavMeshPathStatus.PathComplete
            // 그럴 때, 정상적으로 타겟으로 설정해준다.는 뜻
            // 길찾기 실패 했을 때 점프하는 경향이 있다.
            Debug.Log("여기서 걸림");
            NavMeshHit hit;

            if (NavMesh.SamplePosition(a_TargetPos, out hit, 1.0f, NavMesh.AllAreas))
            // 갈 수 없는 위치를 전달했을 경우 갈 수 있는 가장 가까운 위치로 루트 검색
            {
                a_TargetPos = hit.position;
                MyNavCalcPath(a_StartPos, a_TargetPos, ref a_PathLen);
                // Debug.DrawRay(a_TargetPos, Vector3.up, Color.red, 100.0f);
            }
        }

        if (movePath.corners.Length < 2)
            return false;

        for (int i = 1; i < movePath.corners.Length; ++i)
        {
#if UNITY_EDITOR
            //맨마지막 인자(duration 라인을 표시하는 시간
            //Debug.DrawLine(movePath.corners[i], movePath.corners[i] + Vector3.up * i, Color.cyan, 100.0f);
#endif
            m_VecLen = movePath.corners[i] - movePath.corners[i - 1];
            m_VecLen.y = 0.0f;
            a_PathLen = a_PathLen + m_VecLen.magnitude;
        }

        if (a_PathLen <= 0.0f)
            return false;

        // 주인공이 마지막 위치에 도달했을 때 정확한 방향을 바라보게 하고 싶은 경우 때문에 계산해 놓는다.
        pathEndPos = movePath.corners[(movePath.corners.Length - 1)];

        return true;
    }

    // MoveToPath 관련 변수
    bool isSuccessed = true;
    Vector3 curCPos = Vector3.zero;
    Vector3 cacDestV = Vector3.zero;
    Vector3 targetDir;
    float cacSpeed = 0.0f;
    float nowStep = 0.0f;
    Vector3 velocity = Vector3.zero;
    Vector3 vTowardNom = Vector3.zero;
    int oldPathCount = 0;

    public bool MoveToPath(float overSpeed = 1.0f)
    {
        isSuccessed = true;

        if (movePath == null)
        {
            movePath = new NavMeshPath();
        }

        oldPathCount = curPathIndex;
        if (curPathIndex < movePath.corners.Length) // 최소 curPathIndex = 1 보다 큰 경우에
        {
            curCPos = this.transform.position;          // 현재 위치 업데이트
            cacDestV = movePath.corners[curPathIndex];  // 현재 이동해야할 꼭지점의 위치

            curCPos.y = cacDestV.y;         // 높이 오차가 있어서 도착 판정을 못하는 경우가 있다. ( 도착지점의 높이를 캐릭터의 높이에 넣음 )
            targetDir = cacDestV - curCPos; // 현재 이동해야할 목표지점 - 현재 위치 ( 위에서 높이 값을 맞춰줬으므로 같은 평면으로 놓고 구한 것이 된다. ) 
            targetDir.y = 0.0f;             // 한 번 더 평면처리 (쓸데없는 듯)
            targetDir.Normalize();          // 이동해야할 방향벡터 구하기

            cacSpeed = moveVelocity;         // 속력는 버퍼에 넣어 처리
            cacSpeed = cacSpeed * overSpeed; // 현재속도 * 배속 ( 기본배속 1.0f )

            nowStep = cacSpeed * Time.deltaTime; // ( 한 프레임에 이동할 거리 ) 이번에 이동했을 때 이 안으로만 들어와도...

            velocity = cacSpeed * targetDir; // 속도 = 크기 * 방향
            velocity.y = 0.0f;               // 속도 평면처리
            navAgent.velocity = velocity;    // 이동처리

            if ((cacDestV - curCPos).magnitude <= nowStep)   // 다음 지점까지 거리가 한 프레임에 이동할 거리보다 작아지면 중간점에 도착한 것으로 본다.
            {
                //movePath.corners[curPathIndex] = this.transform.position; // 코너의 위치를 캐릭터의 위치로 대체
                curPathIndex = curPathIndex + 1; // 다음 꼭지점 업데이트
            }

            addTimeCount = addTimeCount + Time.deltaTime; // 경과 시간 증가
            if (moveDurTime <= addTimeCount) // '실제 경과 시간'이 '예상 경과 시간'을 초과하면 '목표점에 도달'한 것으로 판정한다.
            {
                curPathIndex = movePath.corners.Length; // 이동종료 [ 현재 꼭지점 경로를 최종경로로 바꿔버림 => 다음 업데이트 때 동작 안한다. ]
            }
        }

        if (curPathIndex < movePath.corners.Length) // 목적지에 아직 도착하지 않았다면
        {
            // 캐릭터 회전 / 애니메이션 방향 조정
            vTowardNom = movePath.corners[curPathIndex] - this.transform.position; // 가야할 지점까지의 거리
            vTowardNom.y = 0.0f;
            vTowardNom.Normalize(); // 단위 벡터를 만든다.

            if (0.0001f < vTowardNom.magnitude) // 로테이션에서는 모두 들어가야 한다.
            {
                Quaternion targetRot = Quaternion.LookRotation(vTowardNom);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
            }
        }
        else // 최종 목적지에 도착한 경우 매 프레임 호출
        {
            if (oldPathCount < movePath.corners.Length) // 최종 목적지에 도착한 경우 한 번 발생시키기 위한 부분
            {
                ClearPath();
            }

            isSuccessed = false; // 아직 목적지에 도착하지 않았다면 다시 잡아 줄 것이기 때문에...
        }
        return isSuccessed;
    }

    void ClearPath()
    {
        isMoveOn = false;

        // 피킹을 위한 동기화
        pathEndPos = transform.position;
        //navAgent.velocity = Vector3.zero; // 목적지에 도착하면 즉시 멈춤

        if (0 < movePath.corners.Length)
        {
            movePath.ClearCorners();    // 경로 모두 제거
        }
        curPathIndex = 1; // 진행 인덱스 초기화
        // 피킹을 위한 동기화 부분
    }
}
