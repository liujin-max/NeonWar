using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Description;
    [SerializeField] Button m_BtnUpgrade;
    [SerializeField] TextMeshProUGUI m_Cost;


    private SkillSlotMsg m_SkillSlot;
    private SkillData m_SkillData;



    void Start()
    {
        m_BtnUpgrade.onClick.AddListener(()=>{
            int cost = Skill.GetCost(m_SkillData, m_SkillSlot.Level);

            if (GameFacade.Instance.DataCenter.User.Glass < cost)
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
                return;
            }
            
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            GameFacade.Instance.DataCenter.User.UpgradeSkill(m_SkillSlot, m_SkillData.ID);

            EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });
    }

    public void Init(SkillSlotMsg skill_slot, SkillData skill_data)
    {
        m_SkillSlot = skill_slot;
        m_SkillData = skill_data;

        FlushUI();
    }

    void FlushUI()
    {
        m_Text.text = m_SkillData.Name;

        m_Icon.sprite = Resources.Load<Sprite>("UI/Skill/" + m_SkillData.ID);
        m_Icon.SetNativeSize();

        m_Description.text = Skill.GetDescription(m_SkillData, m_SkillSlot.Level);

        int cost    = Skill.GetCost(m_SkillData, m_SkillSlot.Level);

        if (GameFacade.Instance.DataCenter.User.Glass >= cost) m_Cost.text = cost.ToString();
        else m_Cost.text = _C.COLOR_RED + cost.ToString();
    }

    public void ShowBtnUpgrade(bool flag)
    {
        m_BtnUpgrade.gameObject.SetActive(flag);
    }
}
