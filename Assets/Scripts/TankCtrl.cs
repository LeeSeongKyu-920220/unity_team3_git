using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCtrl : MonoBehaviour
{
    GameObject target_Obj;              // 타겟 오브젝트 저장
    Vector3 tank_Pos = Vector3.zero;    // 탱크의 좌료 저장
    Vector3 target_Pos = Vector3.zero;  // 타겟의 좌표 저장
    List<GameObject> target_List = new List<GameObject>();  // 타겟 목록 저장
    float att_Delay = 0.0f;     // 공격 딜레이 시간
    float turn_Speed = 10.0f;   // 포탑 회전 속도
    public GameObject turret_Obj = null;  // 포탑 오브젝트
    public GameObject fire_Pos = null;  // 발사 위치 오브젝트
    public GameObject bullet_Obj = null;    // 총알 오브젝트
    public GameObject turret_Explo = null;  // 발사 이펙트 오브젝트

    float h, v;

    void Start()
    {
    }

    void Update()
    {
        tank_Pos = this.transform.position;
        tank_Pos.y = 0.0f;

        TankMove();

        if (att_Delay > 0.0f)
            att_Delay -= Time.deltaTime;

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

        Attack();


    }

    #region ---------- 탱크 이동 부분(임시)

    void TankMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //회전과 이동처리
        this.transform.Rotate(Vector3.up * 150.0f * h * Time.deltaTime);
        this.transform.Translate(Vector3.forward * v * 5.0f * Time.deltaTime);
    }

    #endregion

    #region ---------- 탱크 공격 부분

    void Attack()
    {
        if (target_List.Count <= 0)
            return;

        if (att_Delay > 0.0f)
            return;

        float[] target_Dist = new float[target_List.Count];

        for(int ii = 0; ii < target_List.Count;)
        {
            if (target_List[ii] == null)    // 타겟 리스트의 값이 null 인지 확인
            {
                target_List.Remove(target_List[ii]);    // null 값이 저장되어 있으면 지우기

                if(target_List.Count <= 0)  // null 값을 지워서 리스트가 비어있으면 함수를 빠져 나감
                    return;
            }
            else
            {
                float dis = Vector3.Distance(tank_Pos, target_List[ii].transform.position);
                target_Dist[ii] = dis;
                ii++;
            }
        }

        int target_Index = 0;
        GetMinCheck(target_Dist, out target_Index);

        target_Obj = target_List[target_Index];
        target_Pos = target_Obj.transform.position;
        target_Pos.y = 0.0f;
        att_Delay = 0.5f;
        GameObject bullet = Instantiate(bullet_Obj, fire_Pos.transform.position, turret_Obj.transform.rotation);
        bullet.GetComponent<BulletCtrl>().target_Obj = target_Obj;
        Instantiate(turret_Explo, fire_Pos.transform.position, Quaternion.identity);
    }

    #endregion

    #region ---------- 사정거리 충돌 체크

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.tag.Contains("Enemy") == true)
        {
            target_List.Add(coll.gameObject);
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.tag.Contains("Enemy") == true)
        {
            target_List.Remove(coll.gameObject);

            if (target_Obj == coll.gameObject)
            {
                target_Obj = null;

            }
                
        }
    }

    #endregion

    #region ---------- 배열의 최소값 체크 (제일 가까운 적 체크 용)

    void GetMinCheck(float[] a_Array, out int a_Min)
    {
        float min = a_Array[0];
        a_Min = 0;

        for(int ii = 0; ii < a_Array.Length; ii++)
        {
            if (min > a_Array[ii])
            {
                min = a_Array[ii];
                a_Min = ii;
            }
                
        }
    }

    #endregion

}
