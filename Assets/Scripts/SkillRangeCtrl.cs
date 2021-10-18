using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeCtrl : MonoBehaviour
{
    List<GameObject> target_List = new List<GameObject>();
    SphereCollider range_Coll;
    float damage_Time = 0.5f;

    void Start()
    {
        range_Coll = this.GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (damage_Time > 0.0f)
        {
            damage_Time -= Time.deltaTime;
            return;
        }

        SkillDamage();
    }

    void SkillDamage()
    {
        range_Coll.enabled = false;

        for(int ii = 0; ii < target_List.Count; ii++)
        {
            if (target_List[ii] == null)
                return;

            if (target_List[ii].name.Contains("Enemy") == true)
                target_List[ii].GetComponent<EnemyCtrl>().Damage(50);
            else
            {
                TankCtrl ctrl = target_List[ii].GetComponent<TankCtrl>();
                if(ctrl != null)
                    target_List[ii].GetComponent<TankCtrl>().TakeDamage(50);
                    
            }
                
        }

        Destroy(this.gameObject);
    }

    #region ---------- 사정거리 충돌 체크

    public void OnTriggerEnter(Collider coll)
    {
        target_List.Add(coll.gameObject);
        //if (coll.name.Contains("Enemy") == true)
        //    target_List.Add(coll.gameObject);
    }

    public void OnTriggerExit(Collider coll)
    {
        target_List.Remove(coll.gameObject);
        //if (coll.name.Contains("Enemy") == true)
        //    target_List.Remove(coll.gameObject);
    }

    #endregion
}
