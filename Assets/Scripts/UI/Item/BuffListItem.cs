using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffListItem : MonoBehaviour
{
    [SerializeField] Transform m_Pivot;


    private List<BuffItem> m_BuffItems = new List<BuffItem>();


    private void OnEnable() {
        Event_BuffADD.OnEvent += OnBuffAdd;
        Event_BuffRemove.OnEvent += OnBuffRemove;
    }

    private void OnDisable() {
        Event_BuffADD.OnEvent += OnBuffAdd;
        Event_BuffRemove.OnEvent -= OnBuffRemove;
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
    private void OnBuffAdd(Event_BuffADD e)
    {
        var buff = e.Buff;

        if (buff.Belong != Field.Instance.Player) return;

        AddBuff(buff);
    }

    private void OnBuffRemove(Event_BuffRemove e)
    {
        var buff = e.Buff;

        if (buff.Belong != Field.Instance.Player) return;

        RemoveBuff(buff);
    }
    
    #endregion
}
