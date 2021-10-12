using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBoomCtrl : MonoBehaviour
{
    public GameObject sky_Obj = null;
    public GameObject boom_Obj = null;
    float boom_Delay = 0.0f;
    float target_dist = 0.0f;
    float end_dist = 0.0f;
    Vector3 target_Pos = Vector3.zero;
    Vector3 start_Pos = Vector3.zero;
    Vector3 end_Pos = Vector3.zero;


    void Start()
    {
    }

    void Update()
    {
        if (boom_Delay > 0.0f)
            boom_Delay -= Time.deltaTime;

        Vector3 pos = end_Pos - start_Pos;
        this.transform.Translate(pos * 0.2f * Time.deltaTime);

        target_dist = Vector3.Distance(target_Pos, this.transform.position);
        end_dist = Vector3.Distance(end_Pos, this.transform.position);

        if (target_dist < 3.5)
            BoomCreate();

        if (end_dist < 2.0f)
            Destroy(this.gameObject);
    }

    void BoomCreate()
    {
        if (boom_Delay > 0.0f)
            return;

        int randX = Random.Range(-2, 2);
        int randZ = Random.Range(-2, 4);
        Vector3 pos = this.transform.position;
        pos.x += randX;
        pos.z += randZ;
        Instantiate(boom_Obj, pos, sky_Obj.transform.rotation);
        boom_Delay = 0.02f;
    }

    public void TargetSetting(Vector3 a_Target_Pos)
    {
        start_Pos = this.transform.position;
        Debug.Log(start_Pos);
        target_Pos = a_Target_Pos;
        Debug.Log(target_Pos);
        target_Pos.y = start_Pos.y;
        end_Pos = target_Pos + (start_Pos - target_Pos) * -1;
        end_Pos.y = start_Pos.y;
        Debug.Log(end_Pos);

        Quaternion rotation = Quaternion.LookRotation(end_Pos);
        rotation.x = 0.0f;
        rotation.z = 0.0f;
        sky_Obj.transform.rotation = rotation;
    }
}
