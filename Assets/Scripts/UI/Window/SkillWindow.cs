using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    [SerializeField] Transform m_SkillPivot;
    [SerializeField] Button m_BtnReset;
    [SerializeField] Button m_BtnClose;


    private SkillSlotMsg m_SkillSlot;
    private List<SkillItem> m_SkillItems = new List<SkillItem>();

    SkillItem new_skill_item(int order)
    {
        SkillItem skill_item = null;
        if (m_SkillItems.Count > order)
        {
            skill_item = m_SkillItems[order];
        }
        else
        {
            skill_item = GameFacade.Instance.UIManager.LoadItem("SkillItem", m_SkillPivot).GetComponent<SkillItem>();
            m_SkillItems.Add(skill_item);
        }
        skill_item.gameObject.SetActive(true);

        return skill_item;
    }

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }

    void Start()
    {
        m_BtnClose.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }


    public void Init(SkillSlotMsg skill_slot)
    {
        m_SkillSlot = skill_slot;
    }

    public void InitUpgradeSkill(SkillData skill_data, int level)
    {
        m_SkillItems.ForEach(item => {item.gameObject.SetActive(false);});

        var item = new_skill_item(0);
        item.Init(m_SkillSlot, skill_data);
        item.ShowBtnUpgrade(level < skill_data.LevelMax);

        //重置技能
        m_BtnReset.gameObject.SetActive(true);
        m_BtnReset.onClick.AddListener(()=>{
            GameFacade.Instance.DataCenter.League.ResetSkill(m_SkillSlot);
        });
    }

    public void InitSkills()
    {
        m_BtnReset.gameObject.SetActive(false);

        m_SkillItems.ForEach(item => {item.gameObject.SetActive(false); });

        int[] skill_ids = m_SkillSlot.SkillPool;

        for (int i = 0; i < skill_ids.Length; i++)
        {
            int sk_id = skill_ids[i];
            SkillData skill_data = GameFacade.Instance.DataCenter.League.GetSkillData(sk_id);
            var item = new_skill_item(i);
            item.Init(m_SkillSlot, skill_data);
        }
    }

    


    #region 监听事件
    void OnSkillUpgrade(GameEvent @event)
    {
        GameFacade.Instance.UIManager.UnloadWindow(gameObject);
    }
    #endregion
}
