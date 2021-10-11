using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    public Image hp_Img = null;
    float max_Hp = 100.0f;
    float now_Hp = 0.0f;

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
            Destroy(this.gameObject);
        }
    }
}
