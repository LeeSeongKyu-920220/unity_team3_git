using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObjPool : MonoBehaviour
{
    // static으로 메모리 풀 클래스 선언
    public static UnitObjPool unitObjPool;             // 싱글톤 패턴을 위한 static 선언

    // 생성할 유닛 오브젝트
    public GameObject[] unitObjPrefab;                // 탱크 오브젝트의 순서와 일치해야한다.

    // 유닛 생성 제한 변수
    public static int[] tankCountLimit =  { 0,0,0,0,0 };        // 각 0~4번까지 해당 버튼의 인덱스와 일치시켜야한다. (인덱스 0 == 1번유닛)
    public static int[] activeTankCount = { 0,0,0,0,0 };        // 활성화 되어 있는 탱크 수

    // 탱크 오브젝트 풀
    Queue<TankCtrl>[] tankPool = new Queue<TankCtrl>[5];    // 큐 배열 .... 탱크의 인덱스와 일치 해야한다.

    private void Awake()
    {
        unitObjPool = this;     // 전역변수처럼 사용하기 위한 캐싱
        
        // ↓↓↓↓↓↓ 유닛 프리펩 추가 후 풀어줄 주석
        //// 각 유닛 갯수만큼 미리 생산해서 풀에 추가
        //for (int i = 0; i < unitObjPrefab.Length; i++)
        //{
        //    InitQueue(i, tankCountLimit[i]);
        //}
    }

    void InitQueue(int objKind, int countLimit)
    {
        // 리밋만큼 생산
        for (int i = 0; i < countLimit; i++)
        {
            tankPool[objKind].Enqueue(CreateNewObj(objKind));
        }
    }


    // 새 오브젝트 생성
    public TankCtrl CreateNewObj(int objKind)
    {
        // 제한 갯수 이상이면 생성되지 않는다.
        if (tankCountLimit[objKind] <= tankPool[objKind].Count)
            return null;

        var newObj = Instantiate(unitObjPrefab[objKind], transform).GetComponent<TankCtrl>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    public static TankCtrl GetObj(int objKind, Vector3 setPos)
    {        
        if (unitObjPool.tankPool[objKind].Count > 0)
        {
            var obj = unitObjPool.tankPool[objKind].Dequeue();
            obj.transform.SetParent(null);
            obj.transform.position = setPos;
            obj.gameObject.SetActive(true);
            activeTankCount[objKind]++;
            return obj;
        }
              
        else
        {
            var newObj = unitObjPool.CreateNewObj(objKind);
            newObj.transform.SetParent(null);
            newObj.transform.position = setPos;
            newObj.gameObject.SetActive(true);
            activeTankCount[objKind]++;
            return newObj;
        }
    }

    public static void ReturnObj(TankCtrl tank, int objKind)
    {
        tank.gameObject.SetActive(false);
        activeTankCount[objKind]--;
        tank.transform.SetParent(unitObjPool.transform);
        unitObjPool.tankPool[objKind].Enqueue(tank);
    }
}
