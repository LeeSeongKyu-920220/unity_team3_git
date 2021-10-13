using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EnemyType
{
    Enemy,
    EnemyBase,
}

public class EnemyCtrl : MonoBehaviour
{
    public Image hp_Img = null;
    float max_Hp = 100.0f;
    float now_Hp = 0.0f;

    public EnemyType m_EnemyType = EnemyType.Enemy;
    void Start()
    {
        now_Hp = max_Hp;
    }

    void Update()
    {
        
    }

    public void Damage(float a_Damage)
    {
        now_Hp -= a_Damage;
        hp_Img.fillAmount = now_Hp / max_Hp;

        if (now_Hp <= 0.0f)
        {
            if (m_EnemyType == EnemyType.EnemyBase)
                StartEndCtrl.g_GameState = GameState.GS_GameEnd;

            Destroy(this.gameObject);
        }
    }

    #region ---------- 사정거리 충돌 체크

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.name.Contains("Tank") == true)
        {
            if (coll != coll.GetComponent<SphereCollider>())
                return;

            coll.GetComponent<TankCtrl>().target_List.Add(this.gameObject);
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.name.Contains("Tank") == true)
        {
            if (coll != coll.GetComponent<SphereCollider>())
                return;

            coll.GetComponent<TankCtrl>().target_List.Remove(this.gameObject);

            if (coll.GetComponent<TankCtrl>().target_Obj == this.gameObject)
                coll.GetComponent<TankCtrl>().target_Obj = null;

        }
    }
    #endregion
}
