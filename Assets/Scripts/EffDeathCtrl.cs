using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffDeathCtrl : MonoBehaviour
{
    public float life_Time = 0.0f;

    void Start()
    {
        Destroy(this.gameObject, life_Time);
    }

    void Update()
    {
        
    }
}
