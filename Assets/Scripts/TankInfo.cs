using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
    Normal, // 일반차량
    Speed,  // 가볍고 빠른차량
    Repair, // 수리차량
    Solid,  // 튼튼한차량
    Cannon, // 장거리 미사일
    Count,
}

public class TankInfo
{
    public TankType m_Type;
    public float maxHp;   // 최대체력
    public float speed;   // 이동속도
    public float atk;     // 공격력
    public float attRate; // 공격속도
    public float skillCool; // 스킬 쿨타임
    public float attRange;

    public void TankInit() // 탱크의 기본정보 세팅
    {
        switch(m_Type)
        { 
            case TankType.Normal:
                {
                    maxHp = 100.0f;
                    speed = 5.0f;
                    atk = 10.0f;
                    attRate = 3.0f;
                    skillCool = 5.0f;
                    attRange = 15.0f;
                    break;
                }
            case TankType.Speed:
                {
                    maxHp = 70.0f;
                    speed = 10.0f;
                    atk = 2.0f;
                    attRate = 1.5f;
                    skillCool = 8.0f;
                    attRange = 10.0f;
                    break;
                }
            case TankType.Repair:
                {
                    maxHp = 80.0f;
                    speed = 5.0f;
                    atk = 8.0f;
                    attRate = 3.0f;
                    skillCool = 5.0f;
                    attRange = 15.0f;
                    break;
                }
            case TankType.Solid:
                {
                    maxHp = 200.0f;
                    speed = 2.0f;
                    atk = 3.0f;
                    attRate = 4.0f;
                    skillCool = 10.0f;
                    attRange = 15.0f;
                    break;
                }
            case TankType.Cannon: // 멀리까지 공격이 가능한 차량
                {
                    maxHp = 150.0f;
                    speed = 4.0f;
                    atk = 20.0f;
                    attRate = 5.0f;
                    skillCool = 8.0f;
                    attRange = 15.0f;
                    break;
                }
        }
    }
}
