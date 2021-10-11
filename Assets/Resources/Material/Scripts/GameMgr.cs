using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private LayerMask layerMask = -1;
    Ray a_MousePos;
    RaycastHit hit;
    TankCtrl tankCtrl = null;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Ground");

        // ****************************************************************************************************
        GameObject.Find("TankRoot"); // 일단 임시로 이름으로 오브젝트 찾아 놓았습니다. 추후 수정하면 됩니다.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
