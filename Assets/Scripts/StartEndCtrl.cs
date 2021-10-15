using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    GS_Ready,
    GS_Playing,
    GS_GameEnd,
}

public class StartEndCtrl : MonoBehaviour
{
    public static StartEndCtrl Inst = null;

    public GameState g_GameState = GameState.GS_Ready;

    public Text m_StartCountTxt = null;
    float m_WaitTime = 4.0f;

    public Text m_PlayTimeTxt = null;
    float m_PlayTime = 120.1f;

    [Header("게임 종료 판넬")]
    public GameObject m_GameEndPanel = null;
    public Text m_UITitleTxt = null;
    public Text m_RemainingTimeTxt = null;
    public Text m_ScoreTxt = null;
    public Text m_GoldTxt = null;
    public Button m_RetryBtn = null;
    public Button m_GoToLobbyBtn = null;


    void Start()
    {
        Inst = this;

        m_PlayTimeTxt.gameObject.SetActive(false);
        m_GameEndPanel.SetActive(false);

        if (g_GameState == GameState.GS_Ready && m_StartCountTxt != null)
        {
            m_StartCountTxt.gameObject.SetActive(true);
            m_StartCountTxt.text = m_WaitTime.ToString();
        }

        if (m_RetryBtn != null)
            m_RetryBtn.onClick.AddListener(ReTry);

        if (m_GoToLobbyBtn != null)
            m_GoToLobbyBtn.onClick.AddListener(GotoLobby);
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 준비시간일때
        if (g_GameState == GameState.GS_Ready && m_StartCountTxt !=null)
            ReadyStateFunc();        
        // 게임중일때
        if (g_GameState == GameState.GS_Playing && m_PlayTimeTxt != null)
            PlayingStateFunc();
        // 게임이 끝났을 때
        if (g_GameState == GameState.GS_GameEnd)
        {
            m_PlayTimeTxt.gameObject.SetActive(false) ;

            if (0 < m_PlayTime)
                WinAndLose("승리");
            else
                WinAndLose("패배");
        }

        // Debug.Log(g_GameState);

    }

    // 게임 준비시 카운트 함수
    void ReadyStateFunc()
    {
        if (0 < m_WaitTime)
        {
            if (m_StartCountTxt != null)
                m_StartCountTxt.text = ((int)m_WaitTime).ToString();

            m_WaitTime = m_WaitTime - Time.deltaTime;
        }
        else
        {
            m_StartCountTxt.gameObject.SetActive(false);
            g_GameState = GameState.GS_Playing;
        }
    }

    // 게임중 상태 함수
    void PlayingStateFunc()
    {
        if (0 < m_PlayTime)
        {
            m_PlayTimeTxt.gameObject.SetActive(true);
            m_PlayTimeTxt.text = m_PlayTime.ToString("F1");
            m_PlayTime = m_PlayTime - Time.deltaTime;
        }
        if (m_PlayTime <= 0)  // 시간초과로 인한 패배
        {
            g_GameState = GameState.GS_GameEnd;
        }
    }

    void WinAndLose(string a_WL) // 승리시 함수
    {
        m_GameEndPanel.SetActive(true);

        if (a_WL == "승리")
        {
            m_UITitleTxt.color = Color.blue;
            m_UITitleTxt.text = a_WL;
        }
        else if (a_WL == "패배")
        {
            m_UITitleTxt.color = Color.red;
            m_UITitleTxt.text = a_WL;
        }

        m_RemainingTimeTxt.text = "남은시간 : " + m_PlayTime.ToString("F1");
        m_ScoreTxt.text = "점수 : " + "00";
        m_GoldTxt.text = "골드 : " + "00";
    }

    void ReTry()
    {
        Debug.Log("다시하기");
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGame2");
    }
    void GotoLobby()
    {
        Debug.Log("로비로 돌아가기");
    }
}
