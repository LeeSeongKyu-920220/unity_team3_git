using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCtrl : MonoBehaviour
{
    [HideInInspector] public GameObject target_Obj = null;
    public GameObject explo_Obj = null;
    float speed = 20.0f;
    Vector3 target_Pos = Vector3.zero;
    Vector3 dir = Vector3.zero;
    public CannonExplosion cannonExplosion = null;

    private void Start()
    {
        target_Pos = target_Obj.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        dir = target_Pos - this.transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.LookRotation(dir.normalized);
        
        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.name.Contains("Enemy") == true)
        {
            cannonExplosion.Explosion();
            Instantiate(explo_Obj, this.transform.position, Quaternion.identity);
            coll.GetComponent<EnemyCtrl>().Damage(25.0f);
            Destroy(this.gameObject);
        }
    }
}
