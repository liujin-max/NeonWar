using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : BaseWindow
{
    [SerializeField] TextMeshProUGUI m_Level;
    [SerializeField] Transform m_HPPivot;
    [SerializeField] Transform m_BuffPivot;
    [SerializeField] BarTransition m_ProgressBar;
    [SerializeField] Transform m_FingerPivot;


    [Header("按钮")]
    [SerializeField] Button m_BtnSetting;
    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;



    private PlayerHPItem m_HPITEM = null;
    private BuffListItem m_BUFFLISTITEM = null;


    void Awake()
    {
        m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);
        m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);

        //往左
        m_BtnLeft.SetCallback((eventData)=>{
            m_FingerPivot.gameObject.SetActive(false);

            new Event_JoystickPress(){Direction = -1}.Notify();

            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(true);
            m_BtnLeft.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);}, 
        ()=>{ new Event_JoystickPress(){Direction = -1}.Notify();});

        //往右
        m_BtnRight.SetCallback((eventData)=>{
            m_FingerPivot.gameObject.SetActive(false);

            new Event_JoystickPress(){Direction = 1}.Notify();

            m_BtnRight.transform.Find("Wave").gameObject.SetActive(true);
            m_BtnRight.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);}, 
        ()=>{ new Event_JoystickPress(){Direction = 1}.Notify();});


        //设置
        m_BtnSetting.onClick.AddListener(()=>{
            Time.timeScale = 0;

            UICtrl_SettingWindow.Instance.Enter((window)=>{
                window.SetCallback(()=>{
                    Time.timeScale = 1;
                });
            });
        });



        
    }


    public void Init()
    {
        InitPlayerHP();
        InitBuffPivot();
        InitProgress();

        FlushUI();

        StartCoroutine(FingerAnim());
    }

    //玩家血条
    void InitPlayerHP()
    {
        if (m_HPITEM == null) {
            m_HPITEM = GameFacade.Instance.UIManager.LoadItem("PlayerHPItem", m_HPPivot).GetComponent<PlayerHPItem>();
        }

        m_HPITEM.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        m_HPITEM.Show(true);

        m_HPITEM.Init(Field.Instance.Player);
    }

    void InitBuffPivot()
    {
        if (m_BUFFLISTITEM == null) {
            m_BUFFLISTITEM = GameFacade.Instance.UIManager.LoadItem("BuffListItem", m_BuffPivot).GetComponent<BuffListItem>();
        }
        m_BUFFLISTITEM.gameObject.SetActive(true);

        m_BUFFLISTITEM.Init();
    }

    void InitProgress()
    {
        m_ProgressBar.Init(Field.Instance.Spawn.CurrentWave.Progress.Current, Field.Instance.Spawn.CurrentWave.Progress.Total);
    }

    void FlushUI()
    {
        m_Level.text    = DataCenter.Instance.GetLevelString();
    }

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            m_FingerPivot.gameObject.SetActive(false);

            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            new Event_JoystickPress(){Direction = -1}.Notify();
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);
        }

        //向右
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            m_FingerPivot.gameObject.SetActive(false);
            
            m_BtnRight.transform.Find("Wave").gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            new Event_JoystickPress(){Direction = 1}.Notify();
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);
        }
    }

    #region 协程
    IEnumerator FingerAnim()
    {
        m_FingerPivot.gameObject.SetActive(true);

        var origin = m_FingerPivot.localPosition;
        m_FingerPivot.localPosition = new Vector3(9999, 9999, 0);
        
        m_FingerPivot.Find("Left").gameObject.SetActive(true);
        m_FingerPivot.Find("Right").gameObject.SetActive(false);

        yield return new WaitForSeconds(0.4f);

        m_FingerPivot.Find("Right").gameObject.SetActive(true);
        m_FingerPivot.localPosition = origin;


        yield return null;
    }
    #endregion


    // #region 监听事件
    public void OnUpdateHP()
    {
        if (m_HPITEM == null) return;

        m_HPITEM.FlushHP();
    }

    public void OnNextWave()
    {
        InitProgress();
    }

    public void OnEnemyDead(int count)
    {
        m_ProgressBar.SetValue(count);
    }

    // #endregion
}
