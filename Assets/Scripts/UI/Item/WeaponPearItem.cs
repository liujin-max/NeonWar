using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPearItem : MonoBehaviour
{
    [SerializeField] protected Transform m_PearPivot;

    protected List<PearSeatItem> m_PearSeatItems = new List<PearSeatItem>();
    PearSeatItem new_pear_seat_item(int order)
    {
        PearSeatItem item = null;
        if (m_PearSeatItems.Count > order) {
            item = m_PearSeatItems[order];
        }
        else {
            item = GameFacade.Instance.UIManager.LoadItem("PearSeatItem", m_PearPivot.GetChild(order)).GetComponent<PearSeatItem>();
            m_PearSeatItems.Add(item);
        }
        item.gameObject.SetActive(true);

        return item;
    }


    private void OnEnable() {
        Event_PearChange.OnEvent += OnPearChange;
    }

    private void OnDisable() {
        Event_PearChange.OnEvent -= OnPearChange;
    }


    public void Init()
    {
        InitPears();
    }

    //生成宝珠
    public void InitPears()
    {
        m_PearSeatItems.ForEach(item => {item.gameObject.SetActive(false);});

        for (int i = 0; i < DataCenter.Instance.User.CurrentPlayer.PearSlots.Count; i++)
        {
            PearSlotMsg slotMsg = DataCenter.Instance.User.CurrentPlayer.PearSlots[i];

            var item = new_pear_seat_item(i);
            item.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            item.Init(slotMsg.Pear);
        }
    }

    public void FlushUI()
    {
        InitPears();
    }

    #region 监听事件
    private void OnPearChange(Event_PearChange e)
    {
        bool is_equip = e.IsEquip;
        PearSlotMsg slot = e.Slot;

        for (int i = 0; i < DataCenter.Instance.User.CurrentPlayer.PearSlots.Count; i++)
        {
            PearSlotMsg slotMsg = DataCenter.Instance.User.CurrentPlayer.PearSlots[i];
            if (slotMsg == slot)
            {
                var item = m_PearSeatItems[i];
                item.Init(slotMsg.Pear);
                if (is_equip == true) item.OnEquip();
                break;
            }
        }
    }
    #endregion
}
