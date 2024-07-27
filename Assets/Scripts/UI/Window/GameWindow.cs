using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    [SerializeField] GameObject m_Joystick;
    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;
    [SerializeField] GameObject m_BlinkPivot;


    [SerializeField] TextMeshProUGUI m_Glass;

    
    [Header("按钮")]
    [SerializeField] Button m_BtnATK;
    [SerializeField] Button m_BtnASP;


    [SerializeField] private List<CanvasGroup> m_Groups = new List<CanvasGroup>();


    private float LeftPressTime = 0;
    private float RightPressTime= 0;
    private Tweener m_BlinkTweener;



    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONJOYSTICK_SHOW,  OnJoyStickShow);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);


        EventManager.AddHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONJOYSTICK_SHOW,  OnJoyStickShow);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);


        EventManager.DelHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
    }



    //同时按下的时间间隔小于0.05秒
    bool CheckBlink()
    {
        return LeftPressTime > 0 && RightPressTime > 0 && Mathf.Abs(LeftPressTime - RightPressTime) <= 0.05f;
    }

    // Start is called before the first frame update
    void Start()
    {
        //往左
        m_BtnLeft.SetCallback(()=>{
            Field.Instance.LeftBtnPressFlag = true;
            LeftPressTime   = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }, 
        ()=>{Field.Instance.LeftBtnPressFlag = false;}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        //往右
        m_BtnRight.SetCallback(()=>{
            Field.Instance.RightBtnPressFlag = true;
            RightPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }, 
        ()=>{Field.Instance.RightBtnPressFlag = false;}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        });



        //升级攻击力
        m_BtnATK.onClick.AddListener(()=>{
            int cost = GameFacade.Instance.DataCenter.User.GetATKCost();

            if (GameFacade.Instance.DataCenter.User.Glass < cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateATK(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUpgradePivot();

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS, 0));
        });


        //升级攻速
        m_BtnASP.onClick.AddListener(()=>{
            int cost = GameFacade.Instance.DataCenter.User.GetASPCost();

            if (GameFacade.Instance.DataCenter.User.Glass < cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateASP(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUpgradePivot();

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS, 0));
        });
    }   

    public void Init()
    {
        FlushUI();
    }

    void FlushUI()
    {
        FlushUpgradePivot();


        UpdateBlink();
    }

    void FlushUpgradePivot()
    {
        int atk_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;

        m_BtnATK.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = atk_level.ToString();
        m_BtnATK.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetATKCost();

        m_BtnASP.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = asp_level.ToString();
        m_BtnASP.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetASPCost();
    }

    void InitSkills()
    {

    }

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Field.Instance.LeftBtnPressFlag = true;
            LeftPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) { Field.Instance.LeftBtnPressFlag = false;}

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Field.Instance.RightBtnPressFlag = true;
            RightPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) { Field.Instance.RightBtnPressFlag = false;}

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }

        if (CheckBlink() == true)
        {
            LeftPressTime = 0;
            RightPressTime = 0;
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
        }
    }

    void LateUpdate()
    {
        UpdateBlink();
    }

    void UpdateBlink()
    {
        if (Field.Instance.BlinkTimer.IsFinished() == true)
        {
            m_BlinkPivot.SetActive(false);
        }
        else
        {
            m_BlinkPivot.SetActive(true);

            float current   = Field.Instance.BlinkTimer.Current;
            float duration  = Field.Instance.BlinkTimer.Duration;

            m_BlinkPivot.transform.Find("Mask").GetComponent<Image>().fillAmount = 1 - current / duration;
        }
    }



    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        m_Joystick.gameObject.SetActive(true);

        m_Groups.ForEach(group => {
            group.DOFade(0, 0.2f).OnComplete(()=>{
                group.gameObject.SetActive(false);
            });
        });
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        m_Joystick.gameObject.SetActive(false);

        m_Glass.text = GameFacade.Instance.DataCenter.User.Glass.ToString();

        m_Groups.ForEach(group => {
            group.gameObject.SetActive(true);
            group.DOFade(1, 0.1f);
        });
    }

    private void OnJoyStickShow(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        m_Joystick.gameObject.SetActive(flag);
    }

    private void OnUpdateGlass(GameEvent @event)
    {
        int offset  = (int)@event.GetParam(0);

        m_Glass.text = (GameFacade.Instance.DataCenter.User.Glass + offset).ToString();
    }



    private void OnBlinkShake(GameEvent @event)
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.MEDIUM);
        SoundManager.Instance.Load(SOUND.TIP);

        if (m_BlinkTweener != null) {
            m_BlinkTweener.Kill();
            m_BlinkPivot.transform.localPosition = new Vector3(0, -740, 0);
        }

        m_BlinkTweener = m_BlinkPivot.transform.DOShakePosition(0.25f, new Vector3(10, 0, 0), 40, 50);
    }
    #endregion
}
