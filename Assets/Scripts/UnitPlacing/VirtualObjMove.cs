using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjMove : MonoBehaviour
{

    // 진행도 표시 오브젝트
    public GameObject preObj = null;

    public Material correctMtrl = null;     // 설치가 가능하면 보여줄 메테리얼
    public Material denyMtrl = null;        // 설치가 안되면 보여줄 메테리얼

    public float moveSpeed = 28.0f;
    private Vector3 targetObjPos = Vector3.zero;       // 생성할 오브젝트의 위치 변수

    private bool isDragNow = false;

    Ray ray = new Ray();
    RaycastHit hit = new RaycastHit();


    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            targetObjPos = hit.point;
            targetObjPos.y = 1.5f;
            this.transform.position = targetObjPos;
        }
    }




    //// 드래그로 움직여준다.
    //private void OnMouseDrag()
    //{
    //    isDragNow = true;
    //    targetObjPos.x += Input.GetAxisRaw("Mouse X") * Time.deltaTime * moveSpeed;
    //    targetObjPos.z += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * moveSpeed;
    //    targetObjPos.y = 1.5f;

    //    transform.position = targetObjPos;
    //}

    //// 더이상 드래그 하지 않을 때 확인
    //private void OnMouseUp()
    //{
    //    isDragNow = false;
    //}

}
