using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
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

            if (DataCenter.Instance.User.Glass < cost)
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
                return;
            }

            SoundManager.Instance.Load(SOUND.SKILLUP);
            
            DataCenter.Instance.User.UpdateGlass(-cost);

            DataCenter.Instance.User.UpgradeSkill(m_SkillSlot, m_SkillData.ID);

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
        m_Text.text     = m_SkillData.Name;
        m_Icon.sprite   = GameFacade.Instance.AssetManager.LoadSprite("Skills/" + DataCenter.Instance.User.CurrentPlayer.ID, m_SkillData.ID.ToString()); 
        m_Icon.SetNativeSize();

        m_Description.text = Skill.CompareDescription(m_SkillData, m_SkillSlot.Level, m_SkillSlot.Level + 1);

        int cost    = Skill.GetCost(m_SkillData, m_SkillSlot.Level);

        if (DataCenter.Instance.User.Glass >= cost) m_Cost.text = "<sprite=1>" + ToolUtility.FormatNumber(cost);
        else m_Cost.text = _C.COLOR_RED + "<sprite=1>" + ToolUtility.FormatNumber(cost);
    }

    public void ShowBtnUpgrade(bool flag)
    {
        m_BtnUpgrade.gameObject.SetActive(flag);
    }

    public void Focus(bool flag)
    {
        m_Icon.GetComponent<ImageGray>().TurnGray(!flag);

        if (flag == true)
        {
            m_Text.color        = new Color(255/255f, 220/255f, 64/255f, 1);
            m_Description.color = Color.white;
        }
        else
        {
            m_Text.color        = Color.gray;
            m_Description.color = Color.gray;
        }
    }
}
