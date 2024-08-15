using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] protected Transform m_SkillPivot;
    [SerializeField] protected Transform m_PearPivot;

    [Header("按钮")]
    [SerializeField] protected Button m_BtnATK;
    [SerializeField] protected Button m_BtnASP;
    
    protected List<SkillSeatItem> m_SkillSeatItems = new List<SkillSeatItem>();
    protected List<PearSeatItem> m_PearSeatItems = new List<PearSeatItem>();


    SkillSeatItem new_skill_seat_item(int order)
    {
        SkillSeatItem item = null;
        if (m_SkillSeatItems.Count > order) {
            item = m_SkillSeatItems[order];
        }
        else {
            item = GameFacade.Instance.UIManager.LoadItem("SkillSeatItem", m_SkillPivot.Find(order.ToString())).GetComponent<SkillSeatItem>();
            m_SkillSeatItems.Add(item);
        }
        item.gameObject.SetActive(true);

        return item;
    }

    PearSeatItem new_pear_seat_item(int order)
    {
        PearSeatItem item = null;
        if (m_PearSeatItems.Count > order) {
            item = m_PearSeatItems[order];
        }
        else {
            item = GameFacade.Instance.UIManager.LoadItem("PearSeatItem", m_PearPivot.GetChild(order)).GetComponent<PearSeatItem>();
            m_PearSeatItems.Add(item);
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
        InitUpgradePivot();
        InitSkills();
        InitPears();
    }

    public void InitUpgradePivot()
    {
        int atk_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;

        m_BtnATK.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = atk_level.ToString();
        m_BtnATK.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetATKCost();

        m_BtnASP.transform.Find("Level").GetComponent<TextMeshProUGUI>().text   = asp_level.ToString();
        m_BtnASP.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text    = "消耗：" + GameFacade.Instance.DataCenter.User.GetASPCost();
    }

    //生成技能
    public void InitSkills()
    {
        m_SkillSeatItems.ForEach(item => {item.gameObject.SetActive(false);});

        SkillSeat[] seats = new SkillSeat[]
        {
            new SkillSeat(){Order = 0, ATK = 3},
            new SkillSeat(){Order = 1, ASP = 8},
            new SkillSeat(){Order = 2, ATK = 15},
            new SkillSeat(){Order = 3, ASP = 25},
            new SkillSeat(){Order = 4, ATK = 30, ASP = 30}
        };


        for (int i = 0; i < seats.Length; i++)
        {
            SkillSlotMsg skill_msg = GameFacade.Instance.DataCenter.User.CurrentPlayer.SkillSlots[i];
            SkillData skill_data = GameFacade.Instance.DataCenter.League.GetSkillData(skill_msg.ID);

            var item = new_skill_seat_item(i);
            item.Init(seats[i], skill_data, skill_msg.Level);
        }
    }

    //生成宝珠
    public void InitPears()
    {
        m_PearSeatItems.ForEach(item => {item.gameObject.SetActive(false);});

        for (int i = 0; i < GameFacade.Instance.DataCenter.User.CurrentPlayer.PearSlots.Count; i++)
        {
            PearSlotMsg slotMsg = GameFacade.Instance.DataCenter.User.CurrentPlayer.PearSlots[i];
            Pear pear = GameFacade.Instance.DataCenter.Backpack.GetPear(slotMsg.ID);

            var item = new_pear_seat_item(i);
            item.Init(pear);
        }
    }

    public void FlushUI()
    {
        InitUpgradePivot();
        InitSkills();
    }
}
