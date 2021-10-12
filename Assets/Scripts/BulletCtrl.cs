using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [HideInInspector] public GameObject target_Obj = null;
    public GameObject explo_Obj = null;
    float speed = 20.0f;
    Vector3 target_Pos = Vector3.zero;
    Vector3 dir = Vector3.zero;

    void Start()
    {
    }

    void Update()
    {
        if (target_Obj == null)
            return;

        target_Pos = target_Obj.transform.position;
        dir = target_Pos - this.transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        Destroy(gameObject, 2.0f);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.name.Contains("Enemy") == true)
        {
            Instantiate(explo_Obj, this.transform.position, Quaternion.identity);
            coll.GetComponent<EnemyCtrl>().Damage(25.0f);
            Destroy(this.gameObject);
        }
            
    }
}
