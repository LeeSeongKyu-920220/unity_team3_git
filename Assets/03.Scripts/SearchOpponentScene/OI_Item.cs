using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OI_Item : MonoBehaviour
{
    [HideInInspector] public string OpponentId;                      //유저ID
    public Text OpponentNick_txt;           //'닉네임' :
    public Text OpponentWin_txt;            //'승수'승
    public Text OpponentDefeat_txt;         //'패수'패
    public int U_Count;
    public int U_Power;

    // Start is called before the first frame update
    void Start()
    {
        //SO_Mgr에서 값 가져오기
        //SO_Mgr에서 값 가져오기

        if (this.GetComponent<Button>() != null)
            this.GetComponent<Button>().onClick.AddListener(Opponenet_Select);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void Opponenet_Select()
    {
        //값 전달
        SO_Mgr.Inst.OI.OI_Nick_txt.text = OpponentNick_txt.text;
        SO_Mgr.Inst.OI.OI_Win_txt.text = OpponentWin_txt.text;
        SO_Mgr.Inst.OI.OI_Defeat_txt.text = OpponentDefeat_txt.text;
        SO_Mgr.Inst.OI.OI_UserItemCount_txt.text = U_Count.ToString()+"개";
        SO_Mgr.Inst.OI.OI_UnitPower_txt.text = U_Power.ToString();
        //값 전달
        SO_Mgr.OI_Panel.OI_OnOff = true;
    }
}
