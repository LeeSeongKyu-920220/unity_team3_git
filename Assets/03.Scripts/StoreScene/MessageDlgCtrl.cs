using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageDlgCtrl : MonoBehaviour
{
    public Text InfoText = null;
    public Button m_OKBtn = null;
    public Button m_CancelBtn = null;

    string BuyItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/BuyAttItem.php";
    string UpdateItemUrl = "http://pmaker.dothome.co.kr/TeamProject/StoreScene/UpdateAttItem.php";

    // 구입 관련 받을 맴버 변수들
    // 고정 수치 : 이름, 종류 등등
    internal int buy_ItemNo;
    internal int price = 0;
    internal AttUnitState m_AttUnitState;
    internal string buy_ItemName;
    internal int buy_isBuy;
    internal int buy_KindOfItem;
    internal int buy_isAttack;
    // 증가하는 수치들
    internal int buy_Level;
    internal int buy_ItemUsable;

    bool isTry = false;

    // 메시지 박스 자체가 상점용이여서 우선 그냥 작업
    StoreMgr storeMgrRef;
    AttSelNodeCtrl ASNodeCtrlRef;

    // Start is called before the first frame update
    void Start()
    {
        //구매확인
        if (m_OKBtn != null)
            m_OKBtn.onClick.AddListener(BuyOKFunc);

        //구매취소
        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(BuyCancelFunc);

        storeMgrRef = GameObject.Find("Store_Mgr").GetComponent<StoreMgr>();
        ASNodeCtrlRef = GameObject.Find("AttSelItem").GetComponent<AttSelNodeCtrl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //구매 확인 함수
    void BuyOKFunc()
    {
        if (isTry == false)
        {
            isTry = true;
            if (m_AttUnitState == AttUnitState.BeforeBuy)
            {
                StartCoroutine(BuyOkFunc());    // 구매로 넘어가도록
            }
            else
            {
                StartCoroutine(UpdateFunc());
            }
        }
        else
        {
            InfoText.text = "구매 작업을 진행중입니다. 기다리세요";
        }
    }

    //구매 취소 함수
    void BuyCancelFunc()
    {

        this.gameObject.SetActive(false);
        Debug.Log("구매취소");
    }

    IEnumerator BuyOkFunc()
    {
        // 골드 차감 부분 추후에 추가

        WWWForm form = new WWWForm();
        form.AddField("Input_ItemName", buy_ItemName, System.Text.Encoding.UTF8);                   // 아이템 이름 - 받아와야함
        form.AddField("Input_Level", buy_Level.ToString(), System.Text.Encoding.UTF8);              // 아이템 레벨 - 처음은 1로
        form.AddField("Input_isBuy", "1", System.Text.Encoding.UTF8);                               // 아이템 구매여부 - 구매를 하기에 1로        
        form.AddField("Input_KindOfItem", buy_KindOfItem.ToString(), System.Text.Encoding.UTF8);    // 아이템 No(중요) - 받아와야함
        form.AddField("Input_ItemUsable", buy_ItemUsable.ToString(), System.Text.Encoding.UTF8);    // 아이템 사용 수량 - 받아와야함
        form.AddField("Input_isAttack", buy_isAttack.ToString(), System.Text.Encoding.UTF8);        // 공격용인지 확인 - 받아와야 함
        //form.AddField("Input_ID", MyInfo.m_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_ID", "1", System.Text.Encoding.UTF8);          // 테스트 코드 - UserId 구매부분        
        UnityWebRequest request = UnityWebRequest.Post(BuyItemUrl, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(request.downloadHandler.data);
            if (a_ReStr.Contains("Success"))
            {
                // 메모리 바꿔주는 부분
                GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_isBuy = 1;

                storeMgrRef.ResetAttState();
                ASNodeCtrlRef.ReSetState(buy_Level);
                gameObject.SetActive(false);
                Debug.Log("구매완료");
            }
            else
            {
                InfoText.text = "유닛 아이템 구매에 실패했습니다. DB문제";
                Debug.Log(a_ReStr);
            }
        }
        else
        {
            InfoText.text = "유닛 아이템 구매에 실패했습니다. 통신 문제";
        }

        isTry = false;
    }

    IEnumerator UpdateFunc()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_ItemNo", buy_ItemNo.ToString(), System.Text.Encoding.UTF8);            // 아이템 키 - 받아와야함
        form.AddField("Input_ItemName", buy_ItemName, System.Text.Encoding.UTF8);                   // 아이템 이름 - 받아와야함
        form.AddField("Input_Level", buy_Level.ToString(), System.Text.Encoding.UTF8);              // 아이템 레벨 - 처음은 1로
        form.AddField("Input_isBuy", "1", System.Text.Encoding.UTF8);                               // 아이템 구매여부 - 구매를 하기에 1로        
        form.AddField("Input_KindOfItem", buy_KindOfItem.ToString(), System.Text.Encoding.UTF8);    // 아이템 No(중요) - 받아와야함
        form.AddField("Input_ItemUsable", buy_ItemUsable.ToString(), System.Text.Encoding.UTF8);    // 아이템 사용 수량 - 받아와야함
        form.AddField("Input_isAttack", buy_isAttack.ToString(), System.Text.Encoding.UTF8);        // 공격용인지 확인 - 받아와야 함        
        //form.AddField("Input_ID", MyInfo.m_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_ID", "1", System.Text.Encoding.UTF8);                                  // 테스트 코드 - UserId 구매부분        
        UnityWebRequest request = UnityWebRequest.Post(UpdateItemUrl, form);
        yield return request.SendWebRequest();

        // 업데이트가 성공할 경우, UI 업데이트
        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(request.downloadHandler.data);
            if (a_ReStr.Contains("Success"))
            {
                // 글로벌 메모리 바꿔주는 부분
                GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Level = buy_Level;
                GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Att =
                     GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Att + buy_Level * GlobalValue.UnitIncreValue;

                GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Def =
                     GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Def + buy_Level * GlobalValue.UnitIncreValue;

                GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Hp =
                     GlobalValue.m_AttUnitUserItem[buy_KindOfItem - 1].m_Hp + buy_Level * GlobalValue.UnitIncreValue;

                // UI 초기화
                storeMgrRef.ResetAttState();
                ASNodeCtrlRef.ReSetState(buy_Level);

                gameObject.SetActive(false);
                Debug.Log("업데이트 완료");
            }
            else
            {
                InfoText.text = "유닛 아이템 업데이트에 실패했습니다. DB문제";
                Debug.Log(a_ReStr);
            }
        }
        else
        {
            InfoText.text = "유닛 아이템 구매에 실패했습니다. 통신 문제";
        }

        isTry = false;
    }
}
