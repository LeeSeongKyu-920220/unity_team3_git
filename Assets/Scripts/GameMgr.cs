using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    //Ray a_MousePos;
    //RaycastHit hit;
    //TankCtrl tankCtrl = null;

    public Button boom_Btn = null;  // 폭격 스킬 버튼
    public GameObject boom_Obj = null;  // 폭격기 오브젝트
    public GameObject pick_Obj = null;  // 범위 표시 오브젝트

    GameObject target_Pick;
    Vector3 mouse_Pos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // ****************************************************************************************************
        GameObject.Find("TankRoot"); // 일단 임시로 이름으로 오브젝트 찾아 놓았습니다. 추후 수정하면 됩니다.

        // -----------------

        if (boom_Btn != null)
            boom_Btn.onClick.AddListener(() =>
            {
                SkillPickFunc();
            });
    }

    void Update()
    {
        if(target_Pick != null)
        {
            mouse_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_Pos.y = 1.0f;
            target_Pick.transform.position = mouse_Pos;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 start_Pos = target_Pick.transform.position;
                start_Pos += new Vector3(-20, 0, -20);
                start_Pos.y = 20;
                GameObject obj = Instantiate(boom_Obj, start_Pos, transform.rotation);
                obj.GetComponent<SkillBoomCtrl>().TargetSetting(target_Pick.transform.position);
                Instantiate(pick_Obj, target_Pick.transform.position, Quaternion.Euler(90, 0, 0));
                Destroy(target_Pick);
                target_Pick = null;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                Destroy(target_Pick);
                target_Pick = null;
            }
        }
        
    }

    void SkillPickFunc()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target_Pick = Instantiate(pick_Obj, pos, Quaternion.Euler(90, 0, 0));
        target_Pick.GetComponent<EffDeathCtrl>().enabled = false;
    }
}
