using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillSeat
{
    public int Order;
    public int ATK;
    public int ASP;

    public bool IsUnlock()
    {
        PlayerMsg playerMsg = GameFacade.Instance.DataCenter.User.CurrentPlayer;
        
        return playerMsg.ATK >= ATK && playerMsg.ASP >= ASP;
    }
}



public class SkillSeatItem : MonoBehaviour
{
    [SerializeField] private Button m_Touch;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Text;


    private SkillSeat m_SkillSeat;
    private SkillData m_SkillData;
    private int m_Level;


    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            if (!m_SkillSeat.IsUnlock()) return;

            var window = GameFacade.Instance.UIManager.LoadWindow("SkillWindow", UIManager.BOARD).GetComponent<SkillWindow>();
            window.Init(m_SkillSeat);
            if (m_SkillData == null) window.InitSkills();
            else window.InitUpgradeSkill(m_SkillData, m_Level);
        });
    }

    void OnDestroy()
    {
        AssetsManager.Unload(m_Icon.gameObject);
    }
    

    public void Init(SkillSeat skill_seat, SkillData skill_data, int level)
    {
        m_SkillSeat = skill_seat;
        m_SkillData = skill_data;
        m_Level     = level;

        FlushUI();
    }

    void FlushUI()
    {
        if (m_SkillData != null)
        {
            m_Text.text = m_SkillData.Name;

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = AssetsManager.LoadSprite("Skill" , m_SkillData.ID.ToString(), m_Icon.gameObject);
            m_Icon.SetNativeSize();
        }
        else
        {
            m_Icon.gameObject.SetActive(false);
            AssetsManager.Unload(m_Icon.gameObject);
            
            if (!m_SkillSeat.IsUnlock())
            {
                StringBuilder sb = new StringBuilder();

                if (m_SkillSeat.ATK > 0)
                {
                    sb.Append(string.Format("攻击达到{0}级", m_SkillSeat.ATK));
                }

                if (m_SkillSeat.ASP > 0)
                {
                    if (m_SkillSeat.ATK > 0) sb.Append("\n");

                    sb.Append(string.Format("攻速达到{0}级", m_SkillSeat.ASP));
                }

                m_Text.text = sb.ToString();
            }
            else m_Text.text = _C.COLOR_GREEN + "可使用";
        }
    }

}
