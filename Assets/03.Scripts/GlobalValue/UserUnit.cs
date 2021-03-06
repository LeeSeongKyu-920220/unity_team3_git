using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 형식이 공격형인지, 수비형인지, 만약 해당 개수 증가하면 DB 정보 바꿔야함
/// </summary>
public enum UnitType
{
    Def = 0,
    Att,
    TypeCount
}

/// <summary>
/// 유닛 갯수 관련 Enum, 유닛의 갯수가 증가할 시 넣어야 함
/// </summary>
public enum Unitkind
{
    Unit_0 = 0,
    Unit_1,
    Unit_2,
    Unit_3,
    Unit_4,
    UnitCount
}

/// <summary>
/// 유닛 정보를 담는 클래스
/// </summary>
public class UserUnit
{
    // DB에서 가져올 영역
    public int m_UnitNo = 0;    
    public string m_Name = "";                          // 유닛 이름
    public UnitType m_unitType = UnitType.Att;          // 유닛 타입 <- DB에서 bool 형식으로 저장됨
    public Unitkind m_unitkind = Unitkind.Unit_0;       // 유닛 종류 <- DB에서 string 형식으로 저장됨    
    public int m_Level = 0;                             // 유닛 레벨
    public int ItemUsable = 0;                          // 유닛 사용 제한 수량
    //public float m_PosX = 0.0f;                         // 유닛 X 위치
    //public float m_PosY = 0.0f;                         // 유닛 Y 위치
    public int m_UserId = 0;                            // 유닛 소유주 No
    public int m_isBuy = 0;                             // 구매 여부 <-- 추가됨
    public int m_Hp = 0;                               // 유닛 공격력
    public int m_Att = 0;                               // 유닛 공격력
    public int m_Def = 0;                               // 유닛 방어력
    public float m_AttSpeed = 0;                          // 유닛 공격속도
    public float m_Speed = 0;                             // 유닛 이동속도    
    public int m_Price = 0;                             // 아이템 기본 가격
    public int m_UpPrice = 0;                           // 업그레이드 가격

    // 로컬 아이템 사용 영역
    public Sprite m_IconImg = null;                     // 유닛 이미지
    public Vector2 m_IconSize = Vector2.one;            // 유닛 크기    
    public string m_SkillExp = "";                      // 유닛 설명
}
