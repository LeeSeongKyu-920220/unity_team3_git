﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonExplosion : MonoBehaviour
{
    List<GameObject> targetList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            for(int i =0; i < targetList.Count; i++)
            {
                if (targetList[i] == other.gameObject)
                    return;
            }
            targetList.Add(other.gameObject);
        }
    }

    public void Explosion()
    {
        if (targetList.Count < 1)
            return;

        for(int i =0; i<targetList.Count; i++)
        {
            if (targetList[i] == null)
                continue;
            EnemyCtrl a_EnemyNode = null;
            a_EnemyNode = targetList[i].GetComponent<EnemyCtrl>();
            a_EnemyNode.Damage(25.0f);
        }
    }
}
