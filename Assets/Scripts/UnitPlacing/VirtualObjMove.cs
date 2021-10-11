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

    private void Start()
    {
        targetObjPos = this.transform.position;
    }

    // 드래그로 움직여준다.
    private void OnMouseDrag()
    {
        isDragNow = true;
        targetObjPos.x += Input.GetAxisRaw("Mouse X") * Time.deltaTime * moveSpeed;
        targetObjPos.z += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * moveSpeed;
        targetObjPos.y = 1.5f;

        transform.position = targetObjPos;
    }

    // 더이상 드래그 하지 않을 때 확인
    private void OnMouseUp()
    {
        isDragNow = false;
    }

    private void OnCollisionStay(Collision col)
    {
        Debug.Log("충돌중");
        //// 만약 이게 가능 존으로 간 경우
        //if (col.gameObject.CompareTag("AbleZone"))
        //{
        //    Debug.Log("충돌중!");
        //    // 머터리얼을 초록색으로 교체
        //    this.gameObject.GetComponent<MeshRenderer>().material = correctMtrl;

        //    // 마우스를 뗀다면
        //    if (isDragNow == false)
        //    {
        //        Instantiate(preObj);
        //        Destroy(this.gameObject, 0.007f);        // 약간의 딜레이를 두고 삭제
        //    }
        //}

        //else
        //{
        //    Debug.Log("이상한거랑 충돌중");
        //    // 머터리얼을 빨강으로 교체
        //    this.gameObject.GetComponent<MeshRenderer>().material = denyMtrl;
        //}
    }

}
