using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;

public class GameMgr : MonoBehaviour
{
    Ray a_MousePos;
    RaycastHit hit;
    TankCtrl tankCtrl = null;
    float skill_Delay = 0.0f;
    float skill_Time = 0.0f;

    public Button skill_Btn = null;  // 폭격 스킬 버튼
    public Image skill_Img = null;
    public Text skill_Txt = null;
    public GameObject boom_Obj = null;  // 폭격기 오브젝트
    public GameObject pick_Obj = null;  // 범위 표시 오브젝트

    GameObject target_Pick;
    Vector3 mouse_Pos = Vector3.zero;

    public static int[] tankLevel = {1, 1, 1, 1, 1};

    string UpgradeUrl = "wjst5959.dothome.co.kr/Team/UpgradeInfo.php";
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(UpgradeInfoCo());
    }
    void Start()
    {
        if (skill_Btn != null)
            skill_Btn.onClick.AddListener(() =>
            {
                if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
                    return;

                if (skill_Delay > 0.0f)
                    return;

                SkillPickFunc();
            });
    }

    void Update()
    {
        if (StartEndCtrl.Inst.g_GameState != GameState.GS_Playing)
            return;

        if (target_Pick != null)
        {
            mouse_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_Pos.y = 1.0f;
            target_Pick.transform.position = mouse_Pos;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 start_Pos = target_Pick.transform.position;
                start_Pos += new Vector3(-20, 0, -20);
                start_Pos.y = 20;
                GameObject obj = Instantiate(boom_Obj, start_Pos, transform.rotation);
                obj.GetComponent<SkillBoomCtrl>().TargetSetting(target_Pick.transform.position);
                Instantiate(pick_Obj, target_Pick.transform.position, Quaternion.Euler(90, 0, 0));
                Destroy(target_Pick);
                target_Pick = null;
                skill_Delay = skill_Time;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(target_Pick);
                target_Pick = null;
            }
        }

        if (skill_Delay > 0.0f)
        {
            skill_Delay -= Time.deltaTime;
            skill_Txt.text = skill_Delay.ToString("F1");
            skill_Img.fillAmount = skill_Delay / skill_Time;
        }

        if (skill_Delay <= 0.0f)
        {
            skill_Delay = 0.0f;
            skill_Txt.text = "";
        }
            
    }
        
    int g_UniqueID = 1;
    IEnumerator UpgradeInfoCo()
    {
        if (g_UniqueID == -1)
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", g_UniqueID);

        UnityWebRequest a_www = UnityWebRequest.Post(UpgradeUrl, form);
        yield return a_www.SendWebRequest(); // 응답이 올때까지 대기

        if(a_www.error == null) // 에러가 나지 않는다면
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("UpgradeInfo_Receive") == false )
            {
                yield break;
            }

            var N = JSON.Parse(sz);
            if (N == null)
                yield break;

            for(int i = 0; i < N["kind"].Count; i++)
            {
                if (N["kind"] != null)
                {
                    tankLevel[N["kind"][i]] = N["level"][i];
                }
            }

            //for(int i =0; i< tankLevel.Length; i++)
            //{
            //    Debug.Log(i+ "번째 탱크 레벨 : "+ tankLevel[i]);
            //}
        }
    }

    void SkillPickFunc()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target_Pick = Instantiate(pick_Obj, pos, Quaternion.Euler(90, 0, 0));
        target_Pick.GetComponent<EffDeathCtrl>().enabled = false;
    }
}
