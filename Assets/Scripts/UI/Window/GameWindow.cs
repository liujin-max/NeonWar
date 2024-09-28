using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : BaseWindow
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
        m_BtnFight.onClick.AddListener(()=>{
            NavigationController.GotoBattle();
        });
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


    //战斗开始
    public void OnBattleStart()
    {
        transform.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //战斗结束
    public void OnBattleEnd()
    {
        FlushUI();

        transform.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnUpdateGlass()
    {
        m_Glass.SetValue(DataCenter.Instance.User.Glass);
    }

    public void OnShowFight(bool flag)
    {
        m_BtnFight.gameObject.SetActive(flag);
    }
}
