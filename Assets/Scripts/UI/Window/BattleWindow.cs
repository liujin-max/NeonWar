using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Level;
    [SerializeField] Transform m_HPPivot;
    [SerializeField] Transform m_BuffPivot;

    [Header("按钮")]
    [SerializeField] Button m_BtnSetting;
    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;



    private PlayerHPItem m_HPITEM = null;
    private BuffListItem m_BUFFLISTITEM = null;


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONHPUPDATE,       OnUpdateHP);

        m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);
        m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);

        //往左
        m_BtnLeft.SetCallback((eventData)=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));

            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(true);
            m_BtnLeft.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ 
            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);
        }, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        //往右
        m_BtnRight.SetCallback((eventData)=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));

            m_BtnRight.transform.Find("Wave").gameObject.SetActive(true);
            m_BtnRight.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ 
            m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);
        }, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        });


        //设置
        m_BtnSetting.onClick.AddListener(()=>{
            Time.timeScale = 0;
            GameFacade.Instance.UIManager.LoadWindowAsync("SettingWindow", UIManager.BOARD, (obj)=>{
                obj.GetComponent<SettingWindow>().SetCallback(()=>{
                    Time.timeScale = 1;
                });
            });
        });



        
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONHPUPDATE,       OnUpdateHP);
    }

    public void Init()
    {
        InitPlayerHP();
        InitBuffPivot();
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

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            m_BtnLeft.transform.Find("Wave").gameObject.SetActive(false);
        }

        //向右
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            m_BtnRight.transform.Find("Wave").gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);
        }
    }


    #region 监听事件
    private void OnUpdateHP(GameEvent @event)
    {
        if (m_HPITEM == null) return;

        m_HPITEM.FlushHP();
    }

    #endregion
}
