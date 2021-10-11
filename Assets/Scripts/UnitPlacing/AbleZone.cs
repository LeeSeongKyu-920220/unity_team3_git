using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbleZone : MonoBehaviour
{
    UnitPlacing unitPlacing = null;

    private void Awake()
    {
        unitPlacing = GameObject.FindObjectOfType<UnitPlacing>();

        if (unitPlacing != null)
            InvokeRepeating("StateUpdate", 0.2f, 0.2f);
    }

    void StateUpdate()
    {
        if (unitPlacing != null && unitPlacing.placingState == UnitPlacingState.INSTANCE)
        {
            this.gameObject.SetActive(true);
        }

        else
            this.gameObject.SetActive(false);
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "TestObj")
        { Debug.Log("충돌중!"); }
            
        //if (col.gameObject.CompareTag(""))
    }
}
