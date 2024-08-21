using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] protected Transform m_SkillPivot;
    [SerializeField] protected Transform m_PearPivot;


    protected SkillTreeItem m_SkillTreeItem = null;
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


    public void Init()
    {
        InitSkillTree();
        InitPears();
    }

    void InitSkillTree()
    {
        if (m_SkillTreeItem == null) {
            m_SkillTreeItem = GameFacade.Instance.UIManager.LoadItem("SkillTreeItem", m_SkillPivot).GetComponent<SkillTreeItem>();
        }
        m_SkillTreeItem.Init();
    }

    //生成宝珠
    public void InitPears()
    {
        m_PearSeatItems.ForEach(item => {item.gameObject.SetActive(false);});

        for (int i = 0; i < DataCenter.Instance.User.CurrentPlayer.PearSlots.Count; i++)
        {
            PearSlotMsg slotMsg = DataCenter.Instance.User.CurrentPlayer.PearSlots[i];
            Pear pear = DataCenter.Instance.Backpack.GetPear(slotMsg.ID);

            var item = new_pear_seat_item(i);
            item.Init(pear);
        }
    }

    public void FlushUI()
    {
        if (m_SkillTreeItem != null) m_SkillTreeItem.FlushUI();
        InitPears();
    }
}
