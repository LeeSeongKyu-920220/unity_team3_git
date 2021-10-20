using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TankCtrl : MonoBehaviour
{
    // 기본 탱크 정보 변수
    public int level = 0;
    public TankType m_Type = TankType.Normal;      // 탱크타입
    float moveVelocity = 10.0f;             // 이동속도
    float atk = 0.0f;                       // 공격력
    float attRate = 0.0f;                   // 공격 속도
    float curHp = 0.0f;                     // 현재체력
    float maxHp = 0.0f;                     // 최대체력
    float skillCool = 0.0f;                 // 스킬 쿨타임
    float attRange = 0.0f;
    // 기본 탱크 정보 변수

    // 기본 탱크 정보 변수
    [HideInInspector] public GameObject target_Obj;           // 타겟 오브젝트 저장
    //[HideInInspector] public List<GameObject> target_List = new List<GameObject>();  // 타겟 목록 저장
    Vector3 tank_Pos = Vector3.zero;        // 탱크의 좌료 저장
    Vector3 target_Pos = Vector3.zero;      // 타겟의 좌표 저장
    float att_Delay = 0.0f;                 // 공격 딜레이 타이머
    float skill_Delay = 0.0f;               // 스킬 딜레이 타이머
    float turn_Speed = 10.0f;               // 포탑 회전 속도
    public GameObject turret_Obj = null;    // 포탑 오브젝트
    public GameObject fire_Pos = null;      // 발사 위치 오브젝트
    public GameObject bullet_Obj = null;    // 총알 오브젝트
    public GameObject turret_Explo = null;  // 발사 이펙트 오브젝트
    public GameObject cannon_Obj = null;    // 포대 오브젝트

    //float h, v;

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

    TankInfo tankInfo = new TankInfo();

    // 유닛 특성 관련 변수
    int mGBullet = 3; // 기관총 특성이 발동될 때 격발할 탄환의 수
    int bulletIdx = 0; // 현재 격발한 탄환의 수
    float mGRate = 0.2f; // 탄환을 격발할 때 잠깐 사이의 텀
    float mGTimer = 0.0f; // 탄환 격발시 타이머
    [Header("차량타입에 따른 변수")]
    public Transform machineGun_Pos = null; // Speed타입차량의 기관총 트랜스폼
    public GameObject missilePrefab;
    public GameObject barrier;
    // 유닛 특성 관련 변수

    // 탱크 움직임 오디오소스
    AudioSource m_MvSource;

    [HideInInspector] public int m_TankNumber = 0;

    private void Awake()
    {
        // 이 탱크의 오디오소스 할당
        m_MvSource = this.GetComponent<AudioSource>();
    }

    void Start()
    {
        // 탱크 기본정보 받아오기
        //Init();
        // 탱크 기본정보 받아오기
        movePath = new NavMeshPath();
        navAgent = this.gameObject.GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        beginTarPos = GameObject.Find("Begin_Tar_Pos").transform;

        StartCoroutine(SetDestinationCo());
        Init();
        enemies = new List<GameObject>();
        //cannon_Obj.transform.eulerAngles = new Vector3(-45, 0, 0);

    }

    void Update()
    {
        if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            TakeDamage(20);

        tank_Pos = this.transform.position;
        tank_Pos.y = 0.0f;

        AgentOffsetControl();

        if (att_Delay > 0.0f)
            att_Delay -= Time.deltaTime;

        if (skill_Delay > 0.0f)
            skill_Delay -= Time.deltaTime;

        if (target_Obj != null)
        {
            if (m_Type != TankType.Cannon || skill_Delay > 0.0f) // 캐논형 타입의 차량은 스킬 사용중에는 터렛회전을 여기서 하지 않는다.
            { 
                target_Pos = target_Obj.transform.position;
                target_Pos.y = 0.0f;
                Vector3 dir = target_Pos - tank_Pos;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(turret_Obj.transform.rotation, lookRotation, Time.deltaTime * turn_Speed).eulerAngles;
                turret_Obj.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }

        if(GameMgr.Inst.enemy_List.Count <= 0)
        {
            if (m_Type != TankType.Cannon || skill_Delay > 0.0f) // 캐논형 타입의 차량은 스킬 사용중에는 터렛회전을 여기서 하지 않는다.
            { 
                turret_Obj.transform.rotation = Quaternion.Slerp(turret_Obj.transform.rotation, 
                this.transform.rotation, Time.deltaTime * turn_Speed);
                turret_Obj.transform.localEulerAngles = new Vector3(0.0f, turret_Obj.transform.localEulerAngles.y, 0.0f);
            }
        }

        TankUIRotate();
        NavUpdate(); // 길찾기
        Attack();
        // 유닛특성 함수들
        Repair(20); // 리페어 탱크인 경우에만 실행
        MachineGun();
        Cannon();
        Barrier();

    }

    void Init()
    {
        // 탱크 기본정보 받아오기
        tankInfo.m_Type = m_Type;
        tankInfo.TankInit();
        level = GameMgr.tankLevel[(int)m_Type];
        atk = tankInfo.atk * level;
        moveVelocity = tankInfo.speed * level;
        attRate = tankInfo.attRate;
        maxHp = tankInfo.maxHp * level;
        curHp = maxHp;
        skillCool = tankInfo.skillCool;
        attRange = tankInfo.attRange;
        // 탱크 기본정보 받아오기
    }

    void AgentOffsetControl() // 플레이어의 발을 땅에 자연스럽게 올려 놓기 위해 에이전트의 오프셋을 조절해주는 함수
    {
        float baseOffset = 0.0f;
        float curPosY = 0.0f;
        float tarPosY = 0.0f;

        curPosY = transform.position.y;

        tarPosY = GetFootYPos();
        //Debug.Log("목표지점 : " + tarPosY);
        //Debug.Log("현재지점 : " + curPosY);
        baseOffset = tarPosY - curPosY;
        navAgent.baseOffset += baseOffset;
    }
    
    Ray ray;
    RaycastHit hit;

    float GetFootYPos() // 현재 밟고 있는 땅의 높이 구하기
    {
        float a_TarPosY = 0.0f;
        ray.origin = transform.position + new Vector3(0, 1.0f, 0);
        ray.direction = -Vector3.up;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            a_TarPosY = hit.point.y;
            GameObject hitObj = hit.collider.gameObject;
        }
        return a_TarPosY;
    }

    public void TakeDamage(int a_Damage)
    {
        curHp -= a_Damage;

        if (hp_Img != null)
            hp_Img.fillAmount = curHp / maxHp;
        
        MonitorTankDie();
    }

    #region ---------- 탱크 이동 부분(임시)

    void TankUIRotate()
    {
        //h = Input.GetAxis("Horizontal");
        //v = Input.GetAxis("Vertical");

        ////회전과 이동처리
        //this.transform.Rotate(Vector3.up * 150.0f * h * Time.deltaTime);
        //this.transform.Translate(Vector3.forward * v * 5.0f * Time.deltaTime);

        if(tank_Canvas != null)
        {
            tank_Canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            Vector3 pos = this.transform.position;
            pos.y += 0.5f;
            pos.z -= 1;
            tank_Canvas.transform.position = pos;
        }
    }

    #endregion

    #region ---------- 탱크 공격 부분

    void Attack()
    {
        if (TankType.Speed == m_Type || TankType.Cannon == m_Type) // 스킬 사용중일 때는 일반공격 못하도록
            return;

        if (att_Delay > 0.0f)
            return;

        if (GameMgr.Inst.enemy_List.Count <= 0)
        {
            target_Obj = null;
            return;
        }


        List<float> target_Dist = new List<float>();
        List<int> index_List = new List<int>();
        
        for (int ii = 0; ii < GameMgr.Inst.enemy_List.Count;)
        {
            if (GameMgr.Inst.enemy_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                GameMgr.Inst.enemy_List.Remove(GameMgr.Inst.enemy_List[ii]);    // null 값이 저장되어 있으면 지우기

                if (GameMgr.Inst.enemy_List.Count <= 0)
                {
                    target_Obj = null;
                    return;
                }
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, GameMgr.Inst.enemy_List[ii].transform.position);

                if (dis <= attRange)
                {
                    index_List.Add(ii);
                    target_Dist.Add(dis);
                }

                ii++;
            }
        }

        if (target_Dist.Count <= 0)
        {
            target_Obj = null;
            isMoveOn = true;
            return;
        }
            

        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);
        int a = index_List[target_Index];

        target_Obj = GameMgr.Inst.enemy_List[a].gameObject;

        if (target_Obj.name.Contains("Enemy_Base") == true)
            isMoveOn = false;

        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;
        att_Delay = attRate;
        GameObject bullet = Instantiate(bullet_Obj, fire_Pos.transform.position, fire_Pos.transform.rotation);
        bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
        GameObject explo_Obj = Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
        explo_Obj.transform.SetParent(fire_Pos.transform);

    }

    #endregion

    #region ----------- 유닛 특성 구현 부분

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
    // Solid 유닛 스킬 관련 변수
    GameObject a_Barrier = null;
    bool isBarrier = false; // 보호막이 활성화 중인지
    // Solid 유닛 스킬 관련 변수
    void Barrier() // 일정 범위에 보호막을 쳐서 아군을 보호
    {
        if (m_Type != TankType.Solid)
            return;

        if (skill_Delay > 0.0)
            return;

        if(isBarrier == false)
        {
            a_Barrier = Instantiate(barrier, transform.position, Quaternion.identity);
            a_Barrier.transform.SetParent(this.transform);
            isBarrier = true;
        }

        if(isBarrier == true && a_Barrier == null)
        {
            isBarrier = false;
            skill_Delay = skillCool;
        }

    }

    void MachineGun()
    {
        if (m_Type != TankType.Speed)
            return;

        if (att_Delay > 0.0f)
            return;

        if (mGTimer > 0.0f)
        {
            mGTimer -= Time.deltaTime;
            return;
        }

        if (GameMgr.Inst.enemy_List.Count <= 0)
        {
            target_Obj = null;
            mGTimer = 0.0f;
            bulletIdx = 0;
            return;
        }


        List<float> target_Dist = new List<float>();
        List<int> index_List = new List<int>();

        for (int ii = 0; ii < GameMgr.Inst.enemy_List.Count;)
        {
            if (GameMgr.Inst.enemy_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                GameMgr.Inst.enemy_List.Remove(GameMgr.Inst.enemy_List[ii]);    // null 값이 저장되어 있으면 지우기

                if (GameMgr.Inst.enemy_List.Count <= 0)
                {
                    target_Obj = null;
                    mGTimer = 0.0f;
                    bulletIdx = 0;
                    return;
                }
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, GameMgr.Inst.enemy_List[ii].transform.position);

                if (dis <= attRange)
                {
                    index_List.Add(ii);
                    target_Dist.Add(dis);
                }

                ii++;
            }
        }

        if (target_Dist.Count <= 0)
        {
            target_Obj = null;
            mGTimer = 0.0f;
            bulletIdx = 0;
            isMoveOn = true;
            return;
        }


        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);
        int a = index_List[target_Index];

        target_Obj = GameMgr.Inst.enemy_List[a].gameObject;

        if (target_Obj.name.Contains("Enemy_Base") == true)
            isMoveOn = false;

        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;


        if (mGTimer <= 0.0f)
        {
            GameObject bullet = Instantiate(bullet_Obj, machineGun_Pos.transform.position, fire_Pos.transform.rotation);
            bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
            //bullet.GetComponent<MeshRenderer>().material.SetColor("_Color",Color.red);
            GameObject explo_Obj = Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
            explo_Obj.transform.SetParent(fire_Pos.transform);
            mGTimer = mGRate; // 텀 충전
            bulletIdx++;
            if(bulletIdx == mGBullet) // 모든 탄환을 격발하고 나면 스킬쿨타임 돌기 시작
            { 
                att_Delay = attRate; // 스킬 사용후 바로 기본공격 못하게
                mGTimer = 0.0f;
                bulletIdx = 0;
            }
        }
    }
    
    // -------- Cannon 유닛 스킬 관련 변수
    bool isShot = false;
    GameObject missile;
    float tx;
    float ty;
    float tz;
    float v;
    public float g = 9.8f;
    float elapsed_time;
    public float max_height;
    float t;
    Vector3 start_pos;
    Vector3 end_pos;
    float dat;  //도착점 도달 시간 
    float actionTimer = 2.0f;
    int ranEnemyIdx = -1;
    float cannonSD = 30.0f; // 미사일 차량 스킬 사거리
    List<GameObject> enemies = null;
    // -------- Cannon 유닛 스킬 관련 변수
    void Cannon() // 랜덤으로 선택한 적에게 포물선으로 미사일 타격
    {
        if (m_Type != TankType.Cannon)
            return;

        if (enemies.Count > 0) // 사거리 안에 적이 있으면 안움직임
        {
            isMoveOn = false;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                    enemies.RemoveAt(i);
            }
        }
        else
            isMoveOn = true;   // 사거리 안에 적이 없으면 움직임

        // 적 탐색 ---------------------------------------------------------------------------------------------------------
        GameObject[] a_Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        for (int i = 0; i < a_Enemies.Length; i++)
        {
            if ((a_Enemies[i].transform.position - transform.position).magnitude < cannonSD) // 스킬 사거리를 통해 타겟측정
            {
                enemies.Add(a_Enemies[i]);
            }
        }

        if (enemies.Count < 1) // 탐색된 적이 없으면 리턴
            return;
        // 적 탐색 ---------------------------------------------------------------------------------------------------------
        if (skill_Delay > 0.0f) // 아직 스킬 쿨타임이 남아있다면 리턴
            return;

        if (missile == null && isShot == true) // 미사일 격추가 완료 되면
        {
            isShot = false;
            //isMoveOn = true; // 미사일 격추가 끝나면 다시 움직임.
            skill_Delay = skillCool; // 스킬 쿨타임 재활성화
            att_Delay = attRate; // 스킬 사용후 바로 기본공격 못하게
            ranEnemyIdx = -1; // 적선택인덱스 초기화
            actionTimer = 2.0f;
            cannon_Obj.transform.localEulerAngles = new Vector3(0, cannon_Obj.transform.localEulerAngles.y, cannon_Obj.transform.localEulerAngles.z);
            return;
        }

        //------------------------------------------------------------------------------------------------------------------
        if(isShot == false)
        {
            isMoveOn = false; // 스킬 사용중에는 움직이지 않는다.
            
            if(actionTimer > 0.0f) // 시즈모드On 연출타임
            { 
                actionTimer -= Time.deltaTime;
            }

            if(ranEnemyIdx < 0) // 적 선택
            { 
                ranEnemyIdx = Random.Range(0, enemies.Count);
            }
            
            if (enemies[ranEnemyIdx] == null) // 조준 도중에 적이 파괴됐다면
            {
                ranEnemyIdx = -1;
                return;
            }
            
            Quaternion a_TargetDir = Quaternion.LookRotation(enemies[ranEnemyIdx].transform.position - transform.position);
            turret_Obj.transform.rotation = Quaternion.Slerp(turret_Obj.transform.rotation, a_TargetDir, Time.deltaTime * 10.0f);
            
            Vector3 dir = new Vector3(0, 0.5f, 0.5f);
            cannon_Obj.transform.localRotation = Quaternion.LookRotation(dir);


            if (actionTimer <= 0.0f) // 시즈모드On 연출타임이 끝나면 미사일 발사
            { 
                missile = Instantiate(missilePrefab, fire_Pos.transform.position, Quaternion.identity);
                missile.GetComponent<MissileCtrl>().target_Obj = enemies[ranEnemyIdx];
                Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);

                start_pos = missile.transform.position;
                end_pos = enemies[ranEnemyIdx].transform.position;
                max_height = 20.0f;

                var dh = end_pos.y - start_pos.y;
                var mh = max_height - start_pos.y;
                ty = Mathf.Sqrt(2 * g * mh);

                float a = g;
                float b = -2 * ty;
                float c = 2 * dh;

                dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

                tx = -(start_pos.x - end_pos.x) / dat;
                tz = -(start_pos.z - end_pos.z) / dat;

                elapsed_time = 0;

                isShot = true;
            }
        }

        if(isShot == true)
        {
            StartCoroutine(ShootImp());
        }
    }

    IEnumerator ShootImp()
    {
        while (true)
        {
            if (missile == null)
            {
                yield break;
            }

            this.elapsed_time += Time.deltaTime * 0.02f;

            var tx = start_pos.x + this.tx * elapsed_time;
            var ty = start_pos.y + this.ty * elapsed_time - 0.5f * g * elapsed_time * elapsed_time;
            var tz = start_pos.z + this.tz * elapsed_time;

            var tpos = new Vector3(tx, ty, tz);

            missile.transform.LookAt(tpos);
            missile.transform.position = tpos;

            if (this.elapsed_time >= this.dat)
                break;

            yield return null;
        }
    }
    // 유닛 스킬 구현 부분 ------------------------------------------------------------------------------------------------------------------------------
    #endregion
    public IEnumerator SetDestinationCo()
    {
        yield return new WaitForSeconds(0.2f);
        SetDestination(beginTarPos.position);
    }
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

    #region -------------- 길찾기 부분
    public void SetDestination(Vector3 a_SetTargetVec)
    {

        //Debug.Log(a_SelectObj);
        // 캐릭터들의 Hp바와 닉네임바 RaycastTarget을 모두 꺼주어야 피킹이 정상작동한다.
        // 그렇지 않으면 if(IsPointerOverUIObject() == false) 에 의해 막히게 된다.
        startPos = this.transform.position; // 출발 위치
        cacLenVec = a_SetTargetVec - startPos; // 현재지점과 목표지점사이의 거리 벡터

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
    [HideInInspector] public bool isSuccessed = true;
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
                curPathIndex = curPathIndex + 1; // 다음 꼭지점 업데이트
            }

            addTimeCount = addTimeCount + Time.deltaTime; // 경과 시간 증가
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

        if (0 < movePath.corners.Length)
        {
            movePath.ClearCorners();    // 경로 모두 제거
        }
        curPathIndex = 1; // 진행 인덱스 초기화
        // 피킹을 위한 동기화 부분
    }
    #endregion

    #region 탱크 사망처리 부분
    // -------------- 탱크의 사망을 감지하는 함수
    private void MonitorTankDie()
    {
        // 현재 HP가 0 이하인 경우
        if (curHp <= 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            turret_Obj.transform.rotation = this.transform.rotation;
            this.GetComponent<NavMeshAgent>().enabled = false;
            // ---- 폭발 오디오 재생하는 부분
            // 추후 경로 오류 발생시 path만 수정!
            //string resorcepath = "SoundEffect/Explosion01.ogg";
            //AudioClip audio = Resources.Load(resorcepath) as AudioClip;
            //Camera.main.GetComponent<AudioSource>().PlayOneShot(audio);

            // ----- 탱크를 오브젝트 풀로 돌리는 부분
            UnitObjPool.Inst.ReturnObj(this.gameObject, (int)m_Type);
            curHp = maxHp;
            hp_Img.fillAmount = 1.0f;
            ClearPath();
        }
    }
    #endregion
}
