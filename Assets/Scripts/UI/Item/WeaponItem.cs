using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] protected Transform m_SkillPivot;

    protected SkillTreeItem m_SkillTreeItem = null;


    public void Init()
    {
        InitSkillTree();
    }

    void InitSkillTree()
    {
        if (m_SkillTreeItem == null) {
            m_SkillTreeItem = GameFacade.Instance.UIManager.LoadItem("SkillTreeItem", m_SkillPivot).GetComponent<SkillTreeItem>();
        }
        m_SkillTreeItem.Init();
    }


    public void FlushUI()
    {
        if (m_SkillTreeItem != null) m_SkillTreeItem.FlushUI();
    }

}
