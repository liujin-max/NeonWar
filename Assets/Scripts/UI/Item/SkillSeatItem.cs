using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class SkillSeatItem : MonoBehaviour
{
    [SerializeField] private Button m_Touch;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Text;


    private SkillSlotMsg m_SkillSlot;
    private SkillData m_SkillData;



    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            if (!m_SkillSlot.IsUnlock()) return;

            var window = GameFacade.Instance.UIManager.LoadWindow("SkillWindow", UIManager.BOARD).GetComponent<SkillWindow>();
            window.Init(m_SkillSlot);
            if (m_SkillData == null) window.InitSkills();
            else window.InitUpgradeSkill(m_SkillData, m_SkillSlot.Level);
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
        if (m_SkillData != null)
        {
            m_Text.text = m_SkillData.Name;

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = Resources.Load<Sprite>("UI/Skill/" + m_SkillData.ID);
            m_Icon.SetNativeSize();
        }
        else
        {
            m_Icon.gameObject.SetActive(false);

            if (!m_SkillSlot.IsUnlock())
            {
                StringBuilder sb = new StringBuilder();

                if (m_SkillSlot.ATK > 0)
                {
                    sb.Append(string.Format("攻击达到{0}级", m_SkillSlot.ATK));
                }

                if (m_SkillSlot.ASP > 0)
                {
                    if (m_SkillSlot.ATK > 0) sb.Append("\n");

                    sb.Append(string.Format("攻速达到{0}级", m_SkillSlot.ASP));
                }

                m_Text.text = sb.ToString();
            }
            else m_Text.text = _C.COLOR_GREEN + "可使用";
        }
    }

}
