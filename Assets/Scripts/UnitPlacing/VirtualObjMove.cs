using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjMove : MonoBehaviour
{
    //======================================================================================== ↓ 변수 선언부
    // 리얼 유닛 오브젝트
    public GameObject realObj = null;                  // 임시 오브젝트 (나중에 진짜 유닛 할당 필요)

    // 오브젝트의 관련 변수
    [HideInInspector] public int objIndex = -1;                          // 어떤 탱크인지 알려주는 인덱스
    private Vector3 targetObjPos = Vector3.zero;       // 생성할 오브젝트의 위치 변수    

    // 메테리얼 관련 변수   
    public Material correctMtrl = null;     // 설치가 가능하면 보여줄 메테리얼
    public Material denyMtrl = null;        // 설치가 안되면 보여줄 메테리얼
    private new MeshRenderer[] renderer;   // 메테리얼을 바꿔주기 위한 매쉬랜더러    

    // 위치 조정 및 상태 변화를 위한 변수
    Ray ray = new Ray();
    RaycastHit hit = new RaycastHit();    
    bool isOccupied = false;                // 다른 물체가 있는지 확인을 위한 bool

    // UnitPlacing 의 상태 변화를 위한 변수
    UnitPlacing unitPlacing = null;
    //======================================================================================== ↑ 변수 선언부


    //======================================================================================== ↓ 유니티 함수 부분
    //---------------------------------------------------------------------------- Start()
    private void Start()
    {
        // 캐싱 부분
        renderer = this.GetComponentsInChildren<MeshRenderer>();       // 매쉬 랜더러 캐싱
        unitPlacing = GameObject.FindObjectOfType<UnitPlacing>();   // unitPlacing 캐싱
    }
    //---------------------------------------------------------------------------- Start()

    //---------------------------------------------------------------------------- Update()
    private void Update()
    {
        // 오브젝트가 마우스를 따라가도록 함
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {            
            targetObjPos = hit.point;
            targetObjPos.y = 1.55f;
            this.transform.position = targetObjPos;

            // 배치 가능 구역으로 들어간다면 메테리얼을 초록색으로
            if (hit.collider.gameObject.CompareTag("AbleZone") == true 
                || hit.collider.gameObject.CompareTag("Tank") == true
                && isOccupied == false)
            {
                for(int ii = 0; ii < renderer.Length; ii++)
                {
                    renderer[ii].material = correctMtrl;
                }
                

                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("virtual : " + this.gameObject.transform.position);
                    MakeRealObj();
                    unitPlacing.placingState = UnitPlacingState.PRIMARY;        // 상태를 다시 원래대로
                }
            }            

            else
            {
                for (int ii = 0; ii < renderer.Length; ii++)
                {
                    renderer[ii].material = denyMtrl;
                }
            }
        }
    }
    //---------------------------------------------------------------------------- Update()

    private void OnTriggerEnter(Collider col)
    {
        // 배치 가능 구역에 다른 물체가 있다면 설치를 못하게 해준다.
        if (col.gameObject.CompareTag("AbleZone") == false)
            isOccupied = true;
        else
            isOccupied = false;

        if (col.gameObject.CompareTag("Tank") == true)
            if (col == col.GetComponent<SphereCollider>())
                isOccupied = false;
            else
                isOccupied = true;

    }

    private void OnTriggerExit(Collider col)
    {
        isOccupied = false;
    }

    //---------------------------------------------------------------------------- OnCollisionEnter()
    //private void OnCollisionEnter(Collision col)
    //{
    //    // 배치 가능 구역에 다른 물체가 있다면 설치를 못하게 해준다.
    //    if (col.gameObject.CompareTag("AbleZone") == false)
    //        isOccupied = true;
    //    else
    //        isOccupied = false;
    //}
    //---------------------------------------------------------------------------- OnCollisionEnter()

    //---------------------------------------------------------------------------- OnCollisionExit()
    //private void OnCollisionExit(Collision col)
    //{
    //    isOccupied = false;
    //}
    //---------------------------------------------------------------------------- OnCollisionExit()

    //======================================================================================== ↑ 유니티 함수 부분


    //======================================================================================== ↓ 사용자 정의 함수 부분
    //---------------------------------------------------------------------------- MakeRealObj()
    //--------- 클릭시 진짜 오브젝트를 생성해주고 이 오브젝트는 파괴한다.
    private void MakeRealObj()
    {
        //UnitObjPool.GetObj(objIndex, this.transform.position);             // 진짜 오브젝트 생산 (풀에서 꺼내옴)
        
        Destroy(this.gameObject.GetComponent<Rigidbody>());     // 원래꺼가 밀어내는 거 방지를 위해 리지드 바디 삭제        
        Vector3 pos = this.transform.position;
        pos.y = 1.0f;
        Instantiate(realObj, pos, this.gameObject.transform.rotation);   // 임시 오브젝트 생산
        Destroy(this.gameObject, 0.08f);            // 약간 딜레이 주고 배치용 오브젝트 삭제        
    }

}
