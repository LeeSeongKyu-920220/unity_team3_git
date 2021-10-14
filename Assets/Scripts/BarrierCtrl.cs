using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierCtrl : MonoBehaviour
{
    float duration = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        
        if(duration < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyBullet")
        {
            Destroy(other.gameObject);
        }
    }
}
