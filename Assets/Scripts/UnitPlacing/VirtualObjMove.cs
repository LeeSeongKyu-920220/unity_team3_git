using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjMove : MonoBehaviour
{
    //======================================================================================== ↓ 변수 선언부
    // 진행도 표시 오브젝트
    public GameObject preObj = null;

    // 메테리얼 관련 변수   
    public Material correctMtrl = null;     // 설치가 가능하면 보여줄 메테리얼
    public Material denyMtrl = null;        // 설치가 안되면 보여줄 메테리얼
    private new MeshRenderer renderer = new MeshRenderer();   // 메테리얼을 바꿔주기 위한 매쉬랜더러

    private Vector3 targetObjPos = Vector3.zero;       // 생성할 오브젝트의 위치 변수    

    // 위치 조정 및 상태 변화를 위한 변수
    Ray ray = new Ray();
    RaycastHit hit = new RaycastHit();    
    bool isOccupied = false;                // 다른 물체가 있는지 확인을 위한 bool
    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    private void Start()
    {
        // 캐싱 부분
        renderer = this.GetComponent<MeshRenderer>();       // 매쉬 랜더러 캐싱
    }
    //---------------------------------------------------------------------------- Start()

    //---------------------------------------------------------------------------- Update()
    private void Update()
    {
        Debug.Log("isOccupied = " + isOccupied);

        // 오브젝트가 마우스를 따라가도록 함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {            
            targetObjPos = hit.point;
            targetObjPos.y = 1.5f;
            this.transform.position = targetObjPos;

            // 배치 가능 구역으로 들어간다면 메테리얼을 초록색으로
            if (hit.collider.gameObject.CompareTag("AbleZone") == true && isOccupied == false)
            {
                renderer.material = correctMtrl;
            }            

            else
            {
                renderer.material = denyMtrl;
            }
        }
    }
    //---------------------------------------------------------------------------- Update()

    //---------------------------------------------------------------------------- OnCollisionEnter()
    private void OnCollisionEnter(Collision col)
    {
        // 배치 가능 구역에 다른 물체가 있다면 설치를 못하게 해준다.
        if (col.gameObject.CompareTag("AbleZone") == false)
            isOccupied = true;
    }
    //---------------------------------------------------------------------------- OnCollisionEnter()

    //---------------------------------------------------------------------------- OnCollisionExit()
    private void OnCollisionExit(Collision col)
    {
        isOccupied = false;
    }
    //---------------------------------------------------------------------------- OnCollisionExit()

    //======================================================================================== ↑ 유니티 함수 부분


    //======================================================================================== ↓ 사용자 정의 함수 부분
    //---------------------------------------------------------------------------- ()
    //--------- 클릭시 진행도 오브젝트를 생성해주고 이 오브젝트는 파괴한다.
    
}
