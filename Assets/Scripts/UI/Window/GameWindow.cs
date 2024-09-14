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
    [Header("按钮")]
    [SerializeField] Button m_BtnFight;



    private WeaponItem m_WeaponItem = null;


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.AddHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
        

        m_BtnFight.onClick.AddListener(()=>{
            NavigationController.GotoBattle();
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBATTLESTART,    OnBattleStart);
        EventManager.DelHandler(EVENT.ONBATTLEEND,      OnBattleEnd);
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);

    }



 

    public void Init()
    {
        m_Glass.ForceValue(DataCenter.Instance.User.Glass);

        InitWeapon();
        FlushUI();
    }

    void InitWeapon()
    {
        m_WeaponItem = GameFacade.Instance.UIManager.LoadItem(DataCenter.Instance.User.CurrentPlayer.UI + "Item", m_WeaponPivot).GetComponent<WeaponItem>();
        m_WeaponItem.Init();
    }
 

    void FlushUI()
    {
        m_Level.text    = DataCenter.Instance.GetLevelString();

        m_Coin.SetValue(DataCenter.Instance.User.Coin);
        m_Glass.SetValue(DataCenter.Instance.User.Glass);
    }




    #region 监听事件
    //战斗开始
    private void OnBattleStart(GameEvent @event)
    {
        transform.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //战斗结束
    private void OnBattleEnd(GameEvent @event)
    {
        FlushUI();

        transform.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


    private void OnUpdateGlass(GameEvent @event)
    {
        m_Glass.SetValue(DataCenter.Instance.User.Glass);

        // FlushUI();
    }


    #endregion
}
