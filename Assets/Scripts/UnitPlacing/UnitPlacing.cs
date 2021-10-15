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
    // ========= 테스트용....
    // 테스트용 버튼
    public Button unitPlace_Btn = null;             // 유닛 배치 버튼
    public Text unitPlaceBtn_Txt = null;            // 유닛 배치 버튼의 Text

    // 테스트용 오브젝트
    public GameObject testObj = null;
    // ========= 테스트용....

    // 진짜용.....
    public Button[] unitButton = new Button[5];     // 5마리의  유닛을 위한 버튼 ... 각 버튼에 맞는 인덱스 넣어줘야 함


    // 유닛 드래그 앤 드롭 관련 변수
    //private TankCtrl virtualUnitObj = null;
    private GameObject virtualUnitObj = null;          // 아직 배치되지 않은 상태의 유닛 오브젝트
    Vector3 targetPos = Vector3.zero;
    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    void Start()
    {
        //// 유닛 배치 버튼 클릭 감지
        //if (unitPlace_Btn != null && unitPlace_Btn.enabled == true)
        //{
        //    unitPlace_Btn.onClick.AddListener(() =>
        //    {
        //        if (StartEndCtrl.g_GameState != GameState.GS_Playing)
        //            return;

        //        placingState = UnitPlacingState.INSTANCE;
        //        virtualUnitObj = InstanceUnit(unitPlace_Btn);   // 유닛 생성
        //        virtualUnitObj.GetComponent<VirtualObjMove>().objIndex = 0;     // 임시로 인덱스 할당
        //    });
        //}

        MonitorButton();
    }
    //---------------------------------------------------------------------------- Start()

    // 성능 향상을 위한 LateUpdate 사용
    void Update()
    {
        if (StartEndCtrl.g_GameState != GameState.GS_Playing)
            return;

        // 진행 상태를 확인하며 모든 버튼을 꺼주는 함수 실행
        OffAllUnitButton();
    }
    //---------------------------------------------------------------------------- FixedUpdate()
    //======================================================================================== ↑ 유니티 함수 부분

    //======================================================================================== ↓ 사용자 정의 함수 부분

    private void MonitorButton()
    {
        // 인덱스를 위한 변수 선언
        int normal = 0, speed = 1, repair = 2, solid = 3, cannon = 4;

        // 노멀 탱크 버튼 클릭 감지
        if (unitButton[normal] != null && unitButton[normal].enabled == true)
        {
            unitButton[normal].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[normal] >= UnitObjPool.Inst.tankCountLimit[normal])
                    return;

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[normal].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 스피드 탱크 버튼 클릭 감지
        if (unitButton[speed] != null && unitButton[speed].enabled == true)
        {
            unitButton[speed].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[speed] >= UnitObjPool.Inst.tankCountLimit[speed])
                    return;

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[speed].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 리페어 탱크 버튼 클릭 감지
        if (unitButton[repair] != null && unitButton[repair].enabled == true)
        {
            unitButton[repair].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[repair] >= UnitObjPool.Inst.tankCountLimit[repair])
                    return;

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[repair].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 솔리드 탱크 버튼 클릭 감지
        if (unitButton[solid] != null && unitButton[solid].enabled == true)
        {
            unitButton[solid].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[solid] >= UnitObjPool.Inst.tankCountLimit[solid])
                    return;

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[solid].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }

        // 캐논 탱크 버튼 클릭 감지
        if (unitButton[cannon] != null && unitButton[cannon].enabled == true)
        {
            unitButton[cannon].onClick.AddListener(() =>
            {
                // 게임 시작이 아니면 리턴 처리한다.
                if (StartEndCtrl.g_GameState != GameState.GS_Playing)
                    return;

                // 최대 생산 수 이상이면 return
                if (UnitObjPool.Inst.activeTankCount[cannon] >= UnitObjPool.Inst.tankCountLimit[cannon])
                    return;

                placingState = UnitPlacingState.INSTANCE;
                virtualUnitObj = unitButton[cannon].GetComponent<UnitButtonInfo>().InstanceUnit();       // 버튼 내의 가상 Obj 인스턴스
            });
        }
    }

    //---------------------------------------------------------------------------- OffAllUnitButton()
    //--------- 유닛 배치 모드 시 모든 버튼을 꺼주는 함수
    private void OffAllUnitButton()
    {
        if (placingState == UnitPlacingState.PRIMARY)
        {
            // 테스트용 버튼 처리
            //unitPlace_Btn.enabled = true;

            for (int i = 0; i < unitButton.Length; i++)
            {
                if (unitButton[i].enabled == false)
                    unitButton[i].enabled = true;
            }

            return;
        }

        else
        {
            for (int i = 0; i < unitButton.Length; i++)
            {
                unitButton[i].enabled = false;
            }
        }
    }
    //---------------------------------------------------------------------------- OffAllUnitButton()

    //======================================================================================== ↑ 사용자 정의 함수 부분
}
