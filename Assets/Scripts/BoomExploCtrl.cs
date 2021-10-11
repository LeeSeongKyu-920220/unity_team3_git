using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomExploCtrl : MonoBehaviour
{
    public GameObject explo_Obj = null;
    Vector3 boom_Pos = Vector3.zero;
    Vector3 target_Pos = Vector3.zero;
    float explo_Delay = 1.0f;

    void Start()
    {
        boom_Pos = this.transform.position;
        target_Pos = this.transform.position;
        target_Pos.y = 1.2f;
    }

    void Update()
    {
        Vector3 pos = target_Pos - boom_Pos;
        this.transform.Translate(pos * 1.0f * Time.deltaTime);

        if (explo_Delay > 0.0f)
            explo_Delay -= Time.deltaTime;

        BoomExplosion();
    }

    void BoomExplosion()
    {
        if (explo_Delay > 0.0f)
            return;

        Vector3 pos = this.transform.position;
        pos.y = 1.0f;
        Instantiate(explo_Obj, pos, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
