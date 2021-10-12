using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
    Normal, // 일반차량
    Speed,  // 가볍고 빠른차량
    Repair, // 수리차량
    Solid,  // 튼튼한차량
    Count,
}

public class TankInfo : MonoBehaviour
{
    public TankType m_Type = TankType.Normal;
    public float maxHp;   // 최대체력
    public float speed;   // 이동속도
    public float attRate; // 공격속도
    public float skillCool; // 스킬 쿨타임

    public void TankInit() // 탱크의 기본정보 세팅
    {
        switch(m_Type)
        { 
            case TankType.Normal:
                {
                    maxHp = 100.0f;
                    speed = 5.0f;
                    attRate = 3.0f;
                    skillCool = 5.0f;
                    break;
                }
            case TankType.Speed:
                {
                    maxHp = 70.0f;
                    speed = 10.0f;
                    attRate = 1.5f;
                    skillCool = 5.0f;

                    break;
                }
            case TankType.Repair:
                {
                    maxHp = 80.0f;
                    speed = 5.0f;
                    attRate = 3.0f;
                    skillCool = 5.0f;

                    break;
                }
            case TankType.Solid:
                {
                    maxHp = 200.0f;
                    speed = 2.0f;
                    attRate = 4.0f;
                    skillCool = 5.0f;

                    break;
                }
        }
    }
}
