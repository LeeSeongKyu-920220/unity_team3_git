/*
 ===========================================================================================
  ******** 버튼에 붙여주는 스크립트 ********
  1. 버튼에 유닛 프리펩을 붙여주세요
  2. 버튼은 해당 프리펩을 생산합니다.
  3. 텍스트는 자동으로 변화됩니다.
 ===========================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButtonInfo : MonoBehaviour
{
    public GameObject unitPrefab = null;    // 생산할 유닛 프리펩
    public Image unit_Img = null;           // 유닛 이미지 (스프라이트 파일)    
    public Text buttonTxt = null;

    int index = -1;     // 해당 유닛의 인덱스 (0 == 노멀탱크 ..... TankCtrl의 TankType과 같음)

    private void Start()
    {
        index = (int)unitPrefab.GetComponent<TankCtrl>().m_Type;
    }

    private void Update()
    {
        // 텍스트 == "탱크의 타입.ToString()"\n + 해당 탱크에 맞는 탱크의 숫자 표시 (현재 활성화된 탱크 / 그 탱크의 최대 숫자) 
        if (buttonTxt != null)
            buttonTxt.text
            = unitPrefab.GetComponent<TankCtrl>().m_Type.ToString() + "\n" +
            "(" + UnitObjPool.Inst.activeTankCount[index].ToString() + " / " + UnitObjPool.Inst.tankCountLimit[index] + ")";
    }

    public void InstanceUnit()
    {
        // 유닛 오브젝트 인스턴스 하기

    }
}
