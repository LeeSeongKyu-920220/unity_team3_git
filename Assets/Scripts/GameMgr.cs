using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    Ray a_MousePos;
    RaycastHit hit;
    TankCtrl tankCtrl = null;

    public Button boom_Btn = null;  // 폭격 스킬 버튼
    public GameObject boom_Obj = null;  // 폭격기 오브젝트
    public GameObject boomS_Pos = null; // 폭격기 생성 위치
    public GameObject boomT_Pos = null; // 폭격 위치

    // Start is called before the first frame update
    void Start()
    {
        // ****************************************************************************************************
        GameObject.Find("TankRoot"); // 일단 임시로 이름으로 오브젝트 찾아 놓았습니다. 추후 수정하면 됩니다.

        // -----------------

        if (boom_Btn != null)
            boom_Btn.onClick.AddListener(() =>
            {
                GameObject obj = Instantiate(boom_Obj, boomS_Pos.transform.position, transform.rotation);
                obj.GetComponent<SkillBoomCtrl>().TargetSetting(boomT_Pos.transform.position);
            });
    }
}
