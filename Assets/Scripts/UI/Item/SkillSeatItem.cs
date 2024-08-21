using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class SkillSeatItem : MonoBehaviour
{
    [SerializeField] Image m_Frame;
    [SerializeField] Button m_Touch;
    [SerializeField] Image m_Icon;
    [SerializeField] GameObject m_LockTag;
    [SerializeField] TextMeshProUGUI m_Text;


    private SkillSlotMsg m_SkillSlot;
    private SkillData m_SkillData;

    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            if (!m_SkillSlot.IsUnlock()) {
                StringBuilder sb = new StringBuilder();

                if (m_SkillSlot.ATK == m_SkillSlot.ASP)
                {
                    sb.Append(string.Format("攻击、攻速达到{0}级", m_SkillSlot.ATK));
                }
                else
                {
                    if (m_SkillSlot.ATK > 0)
                    {
                        sb.Append(string.Format("攻击达到{0}级", m_SkillSlot.ATK));
                    }

                    if (m_SkillSlot.ASP > 0)
                    {
                        if (m_SkillSlot.ATK > 0) sb.Append("、");

                        sb.Append(string.Format("攻速达到{0}级", m_SkillSlot.ASP));
                    }
                }

                sb.Append("后解锁");

                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, sb.ToString()));

                return;
            }

            var window = GameFacade.Instance.UIManager.LoadWindow("SkillWindow", UIManager.BOARD).GetComponent<SkillWindow>();
            window.Init(m_SkillSlot, m_SkillData);
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
        m_LockTag.SetActive(false);

        if (m_SkillData != null)
        {
            m_Frame.color   = Color.white;
            m_Text.text = m_SkillData.Name;

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = AssetManager.LoadSprite("Skills/" + DataCenter.Instance.User.CurrentPlayer.ID, m_SkillData.ID.ToString());
            m_Icon.SetNativeSize();
        }
        else
        {
            m_Icon.gameObject.SetActive(false);

            if (!m_SkillSlot.IsUnlock())
            {
                m_LockTag.SetActive(true);

                m_Frame.color   = Color.gray;
                m_Text.text     = "";
            }
            else
            {
                m_Frame.color   = Color.green;
                m_Text.text     = _C.COLOR_GREEN + "可使用";
            }
        }
    }

    public void SetTextUp(bool flag)
    {
        m_Text.transform.localPosition = new Vector3(0, 89 * (flag ? 1 : -1), 0);
    }
}
