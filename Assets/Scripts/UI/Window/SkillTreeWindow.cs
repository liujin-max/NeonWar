using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeWindow : BaseWindow
{
    [SerializeField] Transform m_Pivot;

    private SkillTreeItem m_SkillTree = null;


    public void Init()
    {
        InitWeapon();
    }

    void InitWeapon()
    {
        if (m_SkillTree != null) Destroy(m_SkillTree.gameObject);

        m_SkillTree    = GameFacade.Instance.UIManager.LoadItem("SkillTreeItem", m_Pivot).GetComponent<SkillTreeItem>();
        m_SkillTree.Init();
    }

    public void FlushUI()
    {
        if (m_SkillTree != null) m_SkillTree.FlushUI();
    }
}
