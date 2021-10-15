/*
 ===========================================================================================
  ******** 버튼에 붙여주는 스크립트 ********
  1. 버튼에 유닛 프리펩을 붙여주세요
  2. 버튼은 해당 프리펩을 생산합니다.
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



    public void InstanceUnit()
    {
        // 유닛 클래스 오브젝트 인스턴스 하기
    }
}
