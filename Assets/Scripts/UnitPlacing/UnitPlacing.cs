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
    [HideInInspector] public UnitPlacingState placingState = UnitPlacingState.PRIMARY;

    // 유닛 배치 UI 관련 변수
    public Button unitPlace_Btn = null;             // 유닛 배치 버튼
    public Button[] unitButton = new Button[5];     // 5마리의  유닛을 위한 버튼 ... 각 버튼에 맞는 인덱스 넣어줘야 함

    // 테스트용 오브젝트
    public GameObject testObj = null;

    // 유닛 드래그 앤 드롭 관련 변수
    //private TankCtrl virtualUnitObj = null;
    private GameObject virtualUnitObj = null;          // 아직 배치되지 않은 상태의 유닛 오브젝트
    Vector3 targetPos = Vector3.zero;
    Ray ray = new Ray();
    RaycastHit hit = new RaycastHit();

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
                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = InstanceUnit(unitPlace_Btn);   // 유닛 생성
                virtualUnitObj.GetComponent<VirtualObjMove>().objIndex = 0;     // 임시로 인덱스 할당
            });
        }


    }
    //---------------------------------------------------------------------------- Start()

    // 성능 향상을 위한 LateUpdate 사용
    void Update()
    {
        // 유닛 상태를 확인하며 모든 버튼을 꺼주는 함수 실행
        OffAllUnitButton();
    }
    //---------------------------------------------------------------------------- FixedUpdate()
    //======================================================================================== ↑ 유니티 함수 부분

    //======================================================================================== ↓ 사용자 정의 함수 부분
    //---------------------------------------------------------------------------- OffAllUnitButton()
    //--------- 유닛 배치 모드 시 모든 버튼을 꺼주는 함수
    private void OffAllUnitButton()
    {
        if (placingState == UnitPlacingState.PRIMARY)
        {
            unitPlace_Btn.enabled = true;
            //for (int i = 0; i < unitButton.Length; i++)
            //{
            //    if (unitButton[i].enabled == false)
            //        unitButton[i].enabled = true;                                
            //}

            return;
        }

        else
        {
            //for (int i = 0; i < unitButton.Length; i++)
            //{
            //    unitButton[i].enabled = false;
            //}
        }
    }
    //---------------------------------------------------------------------------- OffAllUnitButton()


    //---------------------------------------------------------------------------- InstanceUnit()
    //--------- 유닛을 생성해주는 함수
    private GameObject InstanceUnit(Button button)
    {        
        // 테스트용 !!
        button.enabled = false;     // 버튼 활성화 꺼줌
     
        // 얘는 진짜용!!
        UnitButtonInfo unitButton = button.GetComponent<UnitButtonInfo>();

        // 버튼에 해당하는 유닛 인스턴스
        // unitButton.InstanceUnit();

        // 테스트를 위한 인스턴스
        GameObject testobj = (GameObject)Instantiate(testObj);      // 여기에 testObj 대신 버튼 정보에서 유닛을 할당받아 넣어야한다.
        testObj.transform.position = button.transform.position;

        return testObj;
    }
    //---------------------------------------------------------------------------- InstanceUnit()

    //======================================================================================== ↑ 사용자 정의 함수 부분
}
