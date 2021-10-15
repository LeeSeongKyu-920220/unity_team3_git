using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObjPool : MonoBehaviour
{
    // static으로 메모리 풀 클래스 선언
    public static UnitObjPool unitObjPool;             // 싱글톤 패턴을 위한 static 선언

    // 생성할 유닛 오브젝트
    public GameObject[] unitObjPrefab = new GameObject[5];                // 탱크 오브젝트의 순서와 일치해야한다.

    // 유닛 생성 제한 변수
    public static int[] tankCountLimit =  { 0,0,0,0,0 };        // 각 0~4번까지 해당 버튼의 인덱스와 일치시켜야한다. (인덱스 0 == 1번유닛)
    public static int[] activeTankCount = { 0,0,0,0,0 };        // 활성화 되어 있는 탱크 수

    // 탱크 오브젝트 풀 배열
    Queue<GameObject>[] tankPool = new Queue<GameObject>[5];    // 큐 배열 .... 탱크의 인덱스와 일치 해야한다.
    Queue<GameObject> tempTankPool = new Queue<GameObject>();   // 임시 큐 (큐 배열 선언시, 메모리 공간은 할당되어도 객체 할당이 없어서 필요)

    private void Awake()
    {        
        unitObjPool = this;     // 전역변수처럼 사용하기 위한 캐싱       
    }

    // 테스트를 위한 디버그용 Start 추가
    private void Start()
    {
        // 각 유닛 갯수만큼 미리 생산해서 풀에 추가
        for (int i = 0; i < unitObjPrefab.Length; i++)
        {
            InitQueue(i, tankCountLimit[i]);
        }
    }

    void InitQueue(int objKind, int countLimit)
    {
        // 임시 탱크 큐 초기화
        tempTankPool.Clear();

        // 리밋만큼 생산
        for (int i = 0; i < countLimit; i++)
        {
            tempTankPool.Enqueue(CreateNewObj(objKind));            
        }

        // 임시 큐를 배열 큐에 할당
        tankPool[objKind] = tempTankPool;
    }

    // 새 오브젝트 생성
    public GameObject CreateNewObj(int objKind)
    {
        GameObject newObj = Instantiate(unitObjPrefab[objKind], this.transform);
        newObj.SetActive(false);
        return newObj;
    }

    /// <summary>
    /// 유닛 오브젝트를 풀에서 불러오는 함수
    /// </summary>
    /// <param name="objKind">오브젝트의 종류</param>
    /// <param name="setPos">오브젝트의 위치</param>
    /// <returns></returns>
    public static GameObject GetObj(int objKind, Vector3 setPos)
    {        
        // 풀에 유닛이 존재할 경우
        if (unitObjPool.tankPool[objKind].Count > 0)
        {
            var obj = unitObjPool.tankPool[objKind].Dequeue();
            obj.transform.SetParent(null);
            obj.transform.position = setPos;
            obj.gameObject.SetActive(true);
            activeTankCount[objKind]++;
            return obj;
        }
        
        // 풀에 유닛이 존재하지 않을 경우 새로 생산한다.
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


    /// <summary>
    /// 유닛을 유닛 오브젝트 풀로 반환시켜주는 함수
    /// </summary>
    /// <param name="tank">반환시킬 탱크</param>
    /// <param name="objKind">반환시킬 풀 인덱스</param>
    public static void ReturnObj(GameObject tank, int objKind)
    {
        tank.gameObject.SetActive(false);
        activeTankCount[objKind]--;
        tank.transform.SetParent(unitObjPool.transform);
        unitObjPool.tankPool[objKind].Enqueue(tank);
    }
}
