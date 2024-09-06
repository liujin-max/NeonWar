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

    [SerializeField] Transform m_WeaponPivot;
    [SerializeField] Transform m_FingerPivot;


    [Header("按钮")]
    [SerializeField] Button m_BtnFight;



    private WeaponItem m_WEAPONITEM = null;




    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        

        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
        EventManager.AddHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);


        m_BtnFight.onClick.AddListener(()=>{
            NavigationController.GotoBattle();
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);


        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
        EventManager.DelHandler(EVENT.UI_BACKPACKOPEN,  OnBackpackOpen);
    }



 

    public void Init()
    {
        m_Glass.ForceValue(DataCenter.Instance.User.Glass);

        FlushUI();

        InitWeapon();
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




    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        transform.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;

        m_FingerPivot.localPosition = new Vector3(9999, 9999, 0);
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        FlushUI();

        transform.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;

        m_FingerPivot.localPosition = Vector3.zero;
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        m_Glass.SetValue(DataCenter.Instance.User.Glass);

        FlushUI();
    }
    
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
