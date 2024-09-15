using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeWindow : BaseWindow
{
    [SerializeField] Transform m_Pivot;


    private SkillTreeItem m_SkillTree = null;


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

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

    void FlushUI()
    {
        if (m_SkillTree != null) m_SkillTree.FlushUI();
    }



    #region 监听事件
    private void OnUpdateGlass(GameEvent @event)
    {
        FlushUI();
    }

    #endregion
}
