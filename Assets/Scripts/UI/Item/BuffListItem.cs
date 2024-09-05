using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffListItem : MonoBehaviour
{
    [SerializeField] Transform m_Pivot;


    private List<BuffItem> m_BuffItems = new List<BuffItem>();

    void Awake()
    {
        EventManager.AddHandler(EVENT.ONBUFFADD,    OnBuffAdd);
        EventManager.AddHandler(EVENT.ONBUFFREMOVE, OnBuffRemove);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONBUFFADD,    OnBuffAdd);
        EventManager.DelHandler(EVENT.ONBUFFREMOVE, OnBuffRemove);
    }


    public void Init()
    {
        m_BuffItems.ForEach(item => item.gameObject.SetActive(false));
    }


    public void AddBuff(Buff buff)
    {
        var item = GameFacade.Instance.UIManager.LoadItem("BuffItem", m_Pivot).GetComponent<BuffItem>();
        item.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        item.Init(buff);

        m_BuffItems.Add(item);
    }

    public void RemoveBuff(Buff buff)
    {
        var item = m_BuffItems.Find(item => item.Buff == buff);

        if (item == null) return;

        m_BuffItems.Remove(item);
        Destroy(item.gameObject);
    }



    #region 监听事件
    private void OnBuffAdd(GameEvent @event)
    {
        var buff = @event.GetParam(0) as Buff;

        if (buff.Belong != Field.Instance.Player) return;

        AddBuff(buff);
    }

    private void OnBuffRemove(GameEvent @event)
    {
        var buff = @event.GetParam(0) as Buff;

        if (buff.Belong != Field.Instance.Player) return;

        RemoveBuff(buff);
    }
    
    #endregion
}
