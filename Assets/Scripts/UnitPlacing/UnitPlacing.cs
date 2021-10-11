/*
=========================================================================================================================
Title : <유닛 배치 시스템>
Ver : 1.0
Date : 2021/10/08 ~
BluePrint: 
 1) UI의 유닛 배치 버튼 클릭 → 2) 유닛 배치 상태로 넘어감 → 3) 배치 가능한 공간에 유닛을 배치(가능하면 초록, 불가능하면 붉은색)

Content :
 
 - 레이 캐스트 활용한 유닛 배치
 - 유닛끼리는 콜리전을 통해 배치 가능한 공간과 그렇지 않은 공간 둠
 - 그리드로 구현할지 미지수!

=========================================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitPlacingState
{
    PRIMARY = 0,   // 초기 단계
    INSTANCE,      // 유닛 생성 단계
    PLACING,       // 유닛 배치
    COMPLETE,      // 유닛 배치 완료 단계
}

public class UnitPlacing : MonoBehaviour
{
    //======================================================================================== ↓ 변수 선언부
    // 배치 상태 변수
    private UnitPlacingState placingState = UnitPlacingState.PRIMARY;

    // 유닛 배치 UI 관련 변수
    public Button unitPlace_Btn = null;             // 유닛 배치 버튼
    public Canvas unitCanvas;                       // 유닛 켄버스

    // 테스트용 오브젝트
    public GameObject testObj = null;

    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    void Start()
    {
        // 유닛 배치 버튼 클릭 감지
        if (unitPlace_Btn != null && unitPlace_Btn.enabled == true)
        {
            unitPlace_Btn.onClick.AddListener(() =>
            {
                SetState(UnitPlacingState.INSTANCE);            // 게임 상태 전환 (유닛 생성 단계)
                InstanceUnit(unitPlace_Btn);                    // 유닛 생성
            });
        }


    }
    //---------------------------------------------------------------------------- Start()

    // 성능 향상을 위한 LateUpdate 사용
    void LateUpdate()
    {

    }
    //---------------------------------------------------------------------------- FixedUpdate()
    //======================================================================================== ↑ 유니티 함수 부분


    //======================================================================================== ↓ 사용자 정의 함수 부분

    //---------------------------------------------------------------------------- SetState()
    //--------- 진행 상태를 전환시키는 함수
    private void SetState(UnitPlacingState state)
    {
        // 진행 상태를 전환 (현재 INSTANCE)
        placingState = state;
    }
    //---------------------------------------------------------------------------- SetState()



    //---------------------------------------------------------------------------- InstanceUnit()
    //--------- 유닛을 드래그 해주는 함수
    private GameObject InstanceUnit(Button button)
    {
        UnitButtonInfo unitButton = button.GetComponent<UnitButtonInfo>();

        // 버튼에 해당하는 유닛 인스턴스
        // unitButton.InstanceUnit();

        // 테스트를 위한 인스턴스
        GameObject testobj = (GameObject)Instantiate(testObj);
        Vector3 targetPos = Vector3.zero;

        // UI위치에 오브젝트 생성
        targetPos = button.transform.position;
        testObj.transform.position = targetPos;

        return testObj;
    }
    //---------------------------------------------------------------------------- InstanceUnit()

    //======================================================================================== ↑ 사용자 정의 함수 부분
}
