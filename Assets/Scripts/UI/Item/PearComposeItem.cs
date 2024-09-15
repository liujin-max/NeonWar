using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearComposeItem : MonoBehaviour
{
    [SerializeField] Transform m_PearPivot;
    [SerializeField] TextMeshProUGUI m_Rate;
    [SerializeField] Transform m_ComposePivot;
    [SerializeField] GameObject m_Halo;

    [SerializeField] Button m_BtnSynthesis;
    [SerializeField] Button m_BtnCancel;


    private Action<Pear> m_Callback;
    private Dictionary<int, PearSeatItem> m_ComposeSeats = new Dictionary<int, PearSeatItem>();
    private PearItem m_ComposeItem;

    void Awake()
    {
        m_BtnCancel.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback(null);
        });

        m_BtnSynthesis.onClick.AddListener(()=>{
            List<Pear> pears = new List<Pear>();
            foreach (var seat_item in m_ComposeSeats.Values) {
                if (seat_item.Pear != null) pears.Add(seat_item.Pear);
            }

            if (pears.Count <= 1) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "请放入宝珠"));
                return;
            }

            //合成
            Pear final = DataCenter.Instance.Synthesis(pears);
            if (final != null)
            {
                List<Pear> rewards = new List<Pear>();
                rewards.Add(final);

                GameFacade.Instance.UIManager.LoadWindowAsync(UI.PEARREWARDWINDOW, UIManager.BOARD, (obj)=>{
                    obj.GetComponent<PearRewardWindow>().Init(rewards);
                });

            } 

            if (m_Callback != null) m_Callback(final);
        });

        EventManager.AddHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_COMPOSECHANGE, OnComposeChange);
    }


    int ComposeFillCount()
    {
        int count = 0;
        foreach (var seat_item in m_ComposeSeats.Values) {
            if (seat_item.Pear != null) count++;
        }
        return count;
    }


    public void Init(Action<Pear> callback)
    {
        m_Callback = callback;

        InitSeats();
        FlushUI();
    }

    void InitSeats()
    {
        foreach (var item in m_ComposeSeats.Values) Destroy(item.gameObject);
        m_ComposeSeats.Clear();

        for (int i = 0; i < CONST.COMPOSE_COUNTMAX; i++)
        {
            var item = GameFacade.Instance.UIManager.LoadItem("PearSeatItem", m_PearPivot.Find(i.ToString())).GetComponent<PearSeatItem>();
            item.Init(null);
            item.RegisterTouchListener(()=>{
                if (item.Pear == null) return;

                RemoveComposeItems(item.Pear, true);
            });

            m_ComposeSeats[i] = item;
        }
    }

    void FlushUI()
    {
        int rate = Mathf.Max(0, (ComposeFillCount() - 1) * 50);
        string color = CONST.COLOR_GREEN;
        if (rate <= 30) color = CONST.COLOR_RED;
        else if (rate <= 60) color = CONST.COLOR_ORANGE;

        m_Rate.text = color + rate + "%";

        FlushResult();
    }

    void FlushResult()
    {
        bool is_same    = true;
        int last_id     = -1;
        int count       = 0;

        foreach (var seat_item in m_ComposeSeats.Values)
        {
            if (seat_item.Pear != null) {
                count++;

                if (last_id == -1) last_id = seat_item.Pear.ID;
                else {
                    if (last_id != seat_item.Pear.ID) {
                        is_same = false;
                        break;
                    }
                }
            }
        }

        if (is_same == true && count > 1)
        {
            m_Halo.SetActive(false);

            if (m_ComposeItem == null) {
                m_ComposeItem = GameFacade.Instance.UIManager.LoadItem("PearItem", m_ComposePivot).GetComponent<PearItem>();
            }
            m_ComposeItem.Init(Pear.Create(last_id + 1, 1));
            m_ComposeItem.ShowTagEquip(false);
            m_ComposeItem.ShowName(false);

            m_ComposeItem.gameObject.SetActive(true);
        }
        else
        {
            m_Halo.SetActive(true);
            if (m_ComposeItem != null) m_ComposeItem.gameObject.SetActive(false);
        }
    }

    void PushComposeItems(Pear pear)
    {
        if (ComposeFillCount() >= CONST.COMPOSE_COUNTMAX) return;

        for (int i = 0; i < CONST.COMPOSE_COUNTMAX; i++)
        {
            var item = m_ComposeSeats[i];

            if (item.Pear == null) {   
                item.Init(pear);
                break;
            }
        }

        FlushUI();
    }

    void RemoveComposeItems(Pear pear, bool is_manual)
    {
        foreach (var item in m_ComposeSeats)
        {
            if (item.Value.Pear == pear) {
                item.Value.Init(null);

                if (is_manual) EventManager.SendEvent(new GameEvent(EVENT.UI_COMPOSECHANGE, false, pear));
                break;
            }
        }

        FlushUI();
    }


    #region 监听事件
    void OnComposeChange(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);
        Pear pear = @event.GetParam(1) as Pear;

        if (flag == true) 
        {
            PushComposeItems(pear);
        } 
        else
        {
            RemoveComposeItems(pear, false);
        }
    }
    #endregion
}
