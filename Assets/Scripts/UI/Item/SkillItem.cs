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

    private SkillData m_SkillData;
    private int m_Level;


    void Start()
    {
        m_BtnUpgrade.onClick.AddListener(()=>{
            int cost = Skill.GetCost(m_SkillData, m_Level);

            if (GameFacade.Instance.DataCenter.User.Glass < cost)
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "不足"));
                return;
            }
            
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            GameFacade.Instance.DataCenter.User.UpgradeSkill(m_SkillData.Order, m_SkillData.ID, m_Level);

            EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });
    }

    void OnDestroy()
    {
        AssetsManager.Unload(m_Icon.gameObject);
    }

    public void Init(SkillData skill_data, int level)
    {
        m_SkillData = skill_data;
        m_Level     = level;

        FlushUI();
    }

    void FlushUI()
    {
        m_Text.text = m_SkillData.Name;

        m_Icon.sprite = AssetsManager.LoadSprite("Skill" , m_SkillData.ID.ToString(), m_Icon.gameObject);
        m_Icon.SetNativeSize();

        m_Description.text = Skill.GetDescription(m_SkillData, m_Level);

        int cost    = Skill.GetCost(m_SkillData, m_Level);

        if (GameFacade.Instance.DataCenter.User.Glass >= cost) m_Cost.text = cost.ToString();
        else m_Cost.text = _C.COLOR_RED + cost.ToString();
    }

    public void ShowBtnUpgrade(bool flag)
    {
        m_BtnUpgrade.gameObject.SetActive(flag);
    }
}
