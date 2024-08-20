using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeItem : MonoBehaviour
{
    [SerializeField] protected Transform m_SkillPivot;

    [Header("按钮")]
    [SerializeField] protected Button m_BtnATK;
    [SerializeField] protected Button m_BtnASP;


    protected List<SkillSeatItem> m_SkillSeatItems = new List<SkillSeatItem>();

    SkillSeatItem new_skill_seat_item(int order, Transform pivot)
    {
        SkillSeatItem item = null;
        if (m_SkillSeatItems.Count > order) {
            item = m_SkillSeatItems[order];
        }
        else {
            item = GameFacade.Instance.UIManager.LoadItem("SkillSeatItem", pivot).GetComponent<SkillSeatItem>();
            m_SkillSeatItems.Add(item);
        }
        item.gameObject.SetActive(true);

        return item;
    }

    void Start()
    {
        //升级攻击力
        m_BtnATK.onClick.AddListener(()=>{
            int cost = GameFacade.Instance.DataCenter.User.GetATKCost();

            if (GameFacade.Instance.DataCenter.User.Glass < cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateATK(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUI();
            

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });


        //升级攻速
        m_BtnASP.onClick.AddListener(()=>{
            int cost = GameFacade.Instance.DataCenter.User.GetASPCost();

            if (GameFacade.Instance.DataCenter.User.Glass < cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateASP(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUI();

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });
    }


    public void Init()
    {
        FlushUI();
    }

    void InitUpgradePivot()
    {
        int atk_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;

        m_BtnATK.transform.Find("Name").GetComponent<TextMeshProUGUI>().text   = string.Format("攻击 {0}级", atk_level);
        m_BtnATK.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = GameFacade.Instance.DataCenter.User.GetATKCost().ToString();

        m_BtnASP.transform.Find("Name").GetComponent<TextMeshProUGUI>().text   = string.Format("攻速 {0}级", asp_level);
        m_BtnASP.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = GameFacade.Instance.DataCenter.User.GetASPCost().ToString();
    }

    void InitSkills()
    {
        m_SkillSeatItems.ForEach(item => {item.gameObject.SetActive(false);});


        List<SkillSlotMsg> slots = GameFacade.Instance.DataCenter.User.CurrentPlayer.SkillSlots;

        for (int i = 0; i < slots.Count; i++)
        {
            SkillSlotMsg skill_msg = slots[i];
            SkillData skill_data = GameFacade.Instance.DataCenter.League.GetSkillData(skill_msg.ID);

            var item = new_skill_seat_item(i, m_SkillPivot.Find(skill_msg.POS));
            item.Init(skill_msg, skill_data);
        }
    }

    public void FlushUI()
    {
        InitUpgradePivot();
        InitSkills();
    }
}
