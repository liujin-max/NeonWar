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
    [SerializeField] NumberTransition m_Coin;
    [SerializeField] NumberTransition m_Glass;


    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;
    [SerializeField] GameObject m_BlinkPivot;
    [SerializeField] Transform m_WeaponPivot;
    [SerializeField] Transform m_HPPivot;
    [SerializeField] Transform m_BuffPivot;
    [SerializeField] Transform m_FingerPivot;

    [SerializeField] Button m_BtnSetting;


    [SerializeField] private List<CanvasGroup> m_Groups = new List<CanvasGroup>();

    private PlayerHPItem m_HPITEM = null;
    private BuffListItem m_BUFFLISTITEM = null;
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
        // EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
        EventManager.AddHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        EventManager.DelHandler(EVENT.ONHPUPDATE,       OnUpdateHP);


        EventManager.DelHandler(EVENT.UI_BLINKSHAKE,    OnBlinkShake);
        // EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
        EventManager.DelHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }





    //同时按下的时间间隔小于0.05秒
    bool CheckBlink()
    {
        // return LeftPressTime > 0 && RightPressTime > 0 && Mathf.Abs(LeftPressTime - RightPressTime) <= 0.05f;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //往左
        m_BtnLeft.SetCallback((eventData)=>{
            LeftPressTime   = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));

            m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
            m_BtnLeft.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ 
            m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0; 
            m_BtnLeft.transform.Find("Wave").transform.localPosition = new Vector3(0, 100, 0);
        }, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        //往右
        m_BtnRight.SetCallback((eventData)=>{
            RightPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));

            m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
            m_BtnRight.transform.Find("Wave").transform.position = eventData.position;
        }, 
        ()=>{ 
            m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0; 
            m_BtnRight.transform.Find("Wave").transform.localPosition = new Vector3(0, 100, 0);
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

    public void Init()
    {
        m_Glass.ForceValue(DataCenter.Instance.User.Glass);

        FlushUI();

        InitWeapon();
        FlushBlink();

        StartCoroutine(FingerAnim());
    }

    void InitWeapon()
    {
        if (m_WEAPONITEM != null) Destroy(m_WEAPONITEM.gameObject);

        m_WEAPONITEM    = GameFacade.Instance.UIManager.LoadItem(DataCenter.Instance.User.CurrentPlayer.UI, m_WeaponPivot).GetComponent<WeaponItem>();
        m_WEAPONITEM.Init();
    } 

    void FlushUI()
    {
        var str         = DataCenter.Instance.GetLevelString();
        m_Level.text    = str;

        m_Coin.SetValue(DataCenter.Instance.User.Coin);
        m_Glass.SetValue(DataCenter.Instance.User.Glass);

        if (m_WEAPONITEM != null) m_WEAPONITEM.FlushUI();
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

    //Buff区域
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
            LeftPressTime = Time.time;

            m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0;
        }

        //向右
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            RightPressTime = Time.time;

            m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0;
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


    #region 协程
    IEnumerator FingerAnim()
    {
        var origin = m_FingerPivot.localPosition;
        m_FingerPivot.localPosition = new Vector3(9999, 9999, 0);
        
        m_FingerPivot.Find("Left").gameObject.SetActive(true);
        m_FingerPivot.Find("Right").gameObject.SetActive(false);

        var left_origin = m_BtnLeft.transform.localPosition;
        m_BtnLeft.transform.localPosition = new Vector3(9999, 9999, 0);

        var right_origin = m_BtnRight.transform.localPosition;
        m_BtnRight.transform.localPosition = new Vector3(9999, 9999, 0);

        m_BtnLeft.transform.Find("Wave").gameObject.SetActive(true);
        m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;

        m_BtnRight.transform.Find("Wave").gameObject.SetActive(false);
        m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0;

        

        yield return new WaitForSeconds(0.4f);

        m_FingerPivot.Find("Right").gameObject.SetActive(true);
        m_FingerPivot.localPosition = origin;


        m_BtnLeft.transform.localPosition = left_origin;
        m_BtnRight.transform.localPosition = right_origin;

        m_BtnRight.transform.Find("Wave").gameObject.SetActive(true);
        m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
  


        yield return null;
    }
    #endregion


    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        InitPlayerHP();
        InitBuffPivot();

        m_Groups.ForEach(group => {
            group.DOFade(0, 0.2f).OnComplete(()=>{
                group.blocksRaycasts = false;
            });
        });

        m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0;
        m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 0;
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        FlushUI();

        m_Groups.ForEach(group => {
            // group.gameObject.SetActive(true);
            group.blocksRaycasts = true;
            group.DOFade(1, 0.1f);
        });

        if (m_HPITEM != null) {
            m_HPITEM.Show(false);
        }

        m_BtnLeft.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
        m_BtnRight.transform.Find("Wave").GetComponent<CanvasGroup>().alpha = 1;
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        m_Glass.SetValue(DataCenter.Instance.User.Glass);

        FlushUI();
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

    // void OnSkillUpgrade(GameEvent @event)
    // {
    //     FlushUI();
    // }
    
    private void OnPearChange(GameEvent @event)
    {
        if (m_WEAPONITEM == null) return;

        m_WEAPONITEM.InitPears();
    }

    private void OnBackpackOpen(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        // m_WEAPONITEM.OnBackpackOpen(flag);

        if (flag) m_WEAPONITEM.ResetPearParent(@event.GetParam(1) as Transform);
        else m_WEAPONITEM.ResetPearParent(m_WEAPONITEM.transform);

    }

    #endregion
}
