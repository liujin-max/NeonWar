using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Level;
    [SerializeField] TextMeshProUGUI m_SubLevel;
    [SerializeField] NumberTransition m_Coin;
    [SerializeField] NumberTransition m_Glass;


    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;
    [SerializeField] GameObject m_BlinkPivot;
    [SerializeField] Transform m_WeaponPivot;
    [SerializeField] Transform m_HPPivot;


    [SerializeField] private List<CanvasGroup> m_Groups = new List<CanvasGroup>();

    private PlayerHPItem m_HPITEM = null;
    private WeaponItem m_WEAPONITEM = null;


    private float LeftPressTime = 0;
    private float RightPressTime= 0;
    private Tweener m_BlinkTweener;




    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        EventManager.AddHandler(EVENT.ONHPUPDATE,       OnUpdateHP);
        

        EventManager.AddHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
        EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        EventManager.DelHandler(EVENT.ONHPUPDATE,       OnUpdateHP);


        EventManager.DelHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
        EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
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
            LeftPressTime   = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }, 
        ()=>{}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        //往右
        m_BtnRight.SetCallback(()=>{
            RightPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }, 
        ()=>{}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        });

    }   

    public void Init()
    {
        m_Glass.ForceValue(GameFacade.Instance.DataCenter.User.Glass);

        FlushUI();

        InitWeapon();
        FlushBlink();
    }

    void InitWeapon()
    {
        if (m_WEAPONITEM != null) Destroy(m_WEAPONITEM.gameObject);

        m_WEAPONITEM    = GameFacade.Instance.UIManager.LoadItem(GameFacade.Instance.DataCenter.User.CurrentPlayer.UI, m_WeaponPivot).GetComponent<WeaponItem>();
        m_WEAPONITEM.Init();
    } 

    void FlushUI()
    {
        var str         = GameFacade.Instance.DataCenter.GetLevelString();
        m_Level.text    = str;
        m_SubLevel.text = str;

        m_Coin.SetValue(GameFacade.Instance.DataCenter.User.Coin);
        m_Glass.SetValue(GameFacade.Instance.DataCenter.User.Glass);

        if (m_WEAPONITEM != null) m_WEAPONITEM.FlushUI();
    }


    //玩家血条
    void InitPlayerHP()
    {
        if (m_HPITEM == null) {
            m_HPITEM = GameFacade.Instance.UIManager.LoadItem("PlayerHPItem", m_HPPivot).GetComponent<PlayerHPItem>();
        }

        m_HPITEM.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        m_HPITEM.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        m_HPITEM.Show(true);

        m_HPITEM.Init(Field.Instance.Player);
    }

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            LeftPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {}

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            RightPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) {}

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




    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        InitPlayerHP();

        m_Groups.ForEach(group => {
            group.DOFade(0, 0.2f).OnComplete(()=>{
                group.gameObject.SetActive(false);
            });
        });
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        FlushUI();

        m_Groups.ForEach(group => {
            group.gameObject.SetActive(true);
            group.DOFade(1, 0.1f);
        });

        if (m_HPITEM != null) {
            m_HPITEM.Show(false);
        }
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        m_Glass.SetValue(GameFacade.Instance.DataCenter.User.Glass);
    }

    private void OnUpdateHP(GameEvent @event)
    {
        if (m_HPITEM == null) return;

        m_HPITEM.FlushHP();
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
    
    private void OnPearChange(GameEvent @event)
    {
        if (m_WEAPONITEM == null) return;

        m_WEAPONITEM.InitPears();
    }

    #endregion
}
