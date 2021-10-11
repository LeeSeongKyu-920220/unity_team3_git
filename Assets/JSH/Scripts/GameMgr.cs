using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    Ray a_MousePos;
    RaycastHit hit;
    TankCtrl tankCtrl = null;

    // Start is called before the first frame update
    void Start()
    {
        // ****************************************************************************************************
        GameObject.Find("TankRoot"); // 일단 임시로 이름으로 오브젝트 찾아 놓았습니다. 추후 수정하면 됩니다.
    }
}
