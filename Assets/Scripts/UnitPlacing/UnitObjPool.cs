using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObjPool : MonoBehaviour
{
    // static으로 메모리 풀 클래스 선언
    public static UnitObjPool unitObjPool;

    // 생성할 유닛 오브젝트
    public GameObject[] unitObj;

    // 탱크 오브젝트 풀
    private Queue<TankCtrl>[] tankPool = new Queue<TankCtrl>[5];

    private void Awake()
    {
        unitObjPool = this;
    }

    public TankCtrl CreateNewObj(int objKind)
    {
        var newObj = Instantiate(unitObj[objKind], transform).GetComponent<TankCtrl>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    public static TankCtrl GetObj(int objKind)
    {
        if (unitObjPool.tankPool[objKind].Count > 0)
        {
            var obj = unitObjPool.tankPool[objKind].Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
              
        else
        {
            var newObj = unitObjPool.CreateNewObj(objKind);
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public static void ReturnObj(TankCtrl tank, int objKind)
    {
        tank.gameObject.SetActive(false);
        tank.transform.SetParent(unitObjPool.transform);
        unitObjPool.tankPool[objKind].Enqueue(tank);
    }
}
