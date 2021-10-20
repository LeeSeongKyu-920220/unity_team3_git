using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemCtrl : MonoBehaviour
{
    public Transform tr = null;



    // Start is called before the first frame update
    void Start()
    {
        tr = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        tr.Rotate(0, 1, 0);
    }
}
