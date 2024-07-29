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
    [SerializeField] NumberTransition m_Glass;
    [SerializeField] Transform m_SkillPivot;

    
    [Header("按钮")]
    [SerializeField] Button m_BtnATK;
    [SerializeField] Button m_BtnASP;


    [SerializeField] private List<CanvasGroup> m_Groups = new List<CanvasGroup>();
    private List<SkillSeatItem> m_SkillSeatItems = new List<SkillSeatItem>();


    private float LeftPressTime = 0;
    private float RightPressTime= 0;
    private Tweener m_BlinkTweener;


    SkillSeatItem new_seat_item(int order)
    {
        SkillSeatItem skill_item = null;
        if (m_SkillSeatItems.Count > order)
        {
            skill_item = m_SkillSeatItems[order];
        }
        else
        {
            skill_item = GameFacade.Instance.UIManager.LoadItem("SkillSeatItem", m_SkillPivot.Find(order.ToString())).GetComponent<SkillSeatItem>();
            m_SkillSeatItems.Add(skill_item);
        }
        skill_item.gameObject.SetActive(true);

        return skill_item;
    }


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONJOYSTICK_SHOW,  OnJoyStickShow);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        

        EventManager.AddHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
        EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONJOYSTICK_SHOW,  OnJoyStickShow);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);


        EventManager.DelHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
        EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
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

            FlushUI();
            

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

            FlushUI();

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS, 0));
        });
    }   

    public void Init()
    {
        m_Glass.ForceValue(GameFacade.Instance.DataCenter.User.Glass);

        FlushUI();
        FlushBlink();
    }

    void FlushUI()
    {
        InitUpgradePivot();
        InitSkills();
    }

    void InitUpgradePivot()
    {
        int atk_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;

        m_BtnATK.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = atk_level.ToString();
        m_BtnATK.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetATKCost();

        m_BtnASP.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = asp_level.ToString();
        m_BtnASP.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetASPCost();
    }

    //生成技能
    void InitSkills()
    {
        m_SkillSeatItems.ForEach(item => {item.gameObject.SetActive(false); });

        SkillSeat[] seats = new SkillSeat[]
        {
            new SkillSeat(){Order = 0, ATK = 3},
            new SkillSeat(){Order = 1, ASP = 8},
            new SkillSeat(){Order = 2, ATK = 15},
            new SkillSeat(){Order = 3, ASP = 25},
            new SkillSeat(){Order = 4, ATK = 30, ASP = 30}
        };


        for (int i = 0; i < seats.Length; i++)
        {
            SkillMsg skill_msg = GameFacade.Instance.DataCenter.User.CurrentPlayer.Skills[i];
            SkillData skill_data = GameFacade.Instance.DataCenter.League.GetSkillData(skill_msg.ID);

            var item = new_seat_item(i);
            item.Init(seats[i], skill_data, skill_msg.Level);
        }
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
        FlushBlink();
    }

    void FlushBlink()
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

    void FlushGlass()
    {
        m_Glass.SetValue(GameFacade.Instance.DataCenter.User.Glass);
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

        FlushGlass();

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

        m_Glass.SetValue(GameFacade.Instance.DataCenter.User.Glass + offset);
    }



    void OnBlinkShake(GameEvent @event)
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.MEDIUM);
        SoundManager.Instance.Load(SOUND.TIP);

        if (m_BlinkTweener != null) {
            m_BlinkTweener.Kill();
            m_BlinkPivot.transform.localPosition = new Vector3(0, -740, 0);
        }

        m_BlinkTweener = m_BlinkPivot.transform.DOShakePosition(0.25f, new Vector3(10, 0, 0), 40, 50);
    }

    void OnSkillUpgrade(GameEvent @event)
    {
        FlushUI();
    }
    
    #endregion
}
