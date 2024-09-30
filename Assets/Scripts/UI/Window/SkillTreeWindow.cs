using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeWindow : BaseWindow
{
    [SerializeField] Transform m_Pivot;

    [SerializeField] Button m_BtnReset;

    private SkillTreeItem m_SkillTree = null;


    void Awake()
    {
        //重置所有养成及技能
        m_BtnReset.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                DataCenter.Instance.League.ResetWeapon();
            });
        });
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

    public void FlushUI()
    {
        if (m_SkillTree != null) m_SkillTree.FlushUI();

        m_BtnReset.gameObject.SetActive(DataCenter.Instance.User.CurrentPlayer.IsDevelop() == true);
    }
}
