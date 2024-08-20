using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeItem : MonoBehaviour
{
    [SerializeField] Transform m_LinePivot;
    [SerializeField] Transform m_SkillPivot;

    [Header("按钮")]
    [SerializeField] Button m_BtnATK;
    [SerializeField] Button m_BtnASP;


    Dictionary<_C.PROPERTY, List<BarTransition>> m_Bars = new Dictionary<_C.PROPERTY, List<BarTransition>>()
    {
        [_C.PROPERTY.ATK] = new List<BarTransition>(),
        [_C.PROPERTY.ASP] = new List<BarTransition>()
    };

    List<SkillSeatItem> m_SkillSeatItems = new List<SkillSeatItem>();

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
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateATK(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUI();
            FlushBars(_C.PROPERTY.ATK);

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });


        //升级攻速
        m_BtnASP.onClick.AddListener(()=>{
            int cost = GameFacade.Instance.DataCenter.User.GetASPCost();

            if (GameFacade.Instance.DataCenter.User.Glass < cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateASP(1);
            GameFacade.Instance.DataCenter.User.UpdateGlass(-cost);

            FlushUI();
            FlushBars(_C.PROPERTY.ASP);

            EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });
    }


    public void Init()
    {
        FlushUI();
        InitLines();
    }

    void InitUpgradePivot()
    {
        int atk_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level   = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;

        int atk_cost    = GameFacade.Instance.DataCenter.User.GetATKCost();
        int asp_cost    = GameFacade.Instance.DataCenter.User.GetASPCost();

        string atk_color= GameFacade.Instance.DataCenter.User.Glass >= atk_cost ? _C.COLOR_GREEN : _C.COLOR_RED;
        string asp_color= GameFacade.Instance.DataCenter.User.Glass >= asp_cost ? _C.COLOR_GREEN : _C.COLOR_RED;

        m_BtnATK.transform.Find("Name").GetComponent<TextMeshProUGUI>().text            = "攻击";
        m_BtnATK.transform.Find("LevelPivot/Text").GetComponent<TextMeshProUGUI>().text = atk_level.ToString();
        m_BtnATK.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text  = string.Format("<sprite=1>{0}{1}", atk_color, ToolUtility.FormatNumber(atk_cost));

        m_BtnASP.transform.Find("Name").GetComponent<TextMeshProUGUI>().text            = "攻速";
        m_BtnASP.transform.Find("LevelPivot/Text").GetComponent<TextMeshProUGUI>().text = asp_level.ToString();
        m_BtnASP.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text  = string.Format("<sprite=1>{0}{1}", asp_color, ToolUtility.FormatNumber(asp_cost));
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


    void InitLines()
    {
        PlayerMsg player =  GameFacade.Instance.DataCenter.User.CurrentPlayer;

        //攻击
        {
            var bar1 = m_LinePivot.Find("ATK1/Bar").GetComponent<BarTransition>();
            bar1.Init(player.ATK, player.SkillSlots[0].ATK);
            m_Bars[_C.PROPERTY.ATK].Add(bar1);

            var bar2 = m_LinePivot.Find("ATK2/Bar").GetComponent<BarTransition>();
            bar2.Init(player.ATK, player.SkillSlots[1].ATK);
            m_Bars[_C.PROPERTY.ATK].Add(bar2);
        }

        //攻速
        {
            var bar1 = m_LinePivot.Find("ASP1/Bar").GetComponent<BarTransition>();
            bar1.Init(player.ATK, player.SkillSlots[2].ASP);
            m_Bars[_C.PROPERTY.ASP].Add(bar1);

            var bar2 = m_LinePivot.Find("ASP2/Bar").GetComponent<BarTransition>();
            bar2.Init(player.ATK, player.SkillSlots[3].ASP);
            m_Bars[_C.PROPERTY.ASP].Add(bar2);
        }
    }

    public void FlushUI()
    {
        InitUpgradePivot();
        InitSkills();
    }

    void FlushBars(_C.PROPERTY property)
    {
        if (m_Bars.ContainsKey(property))
        {
            int level = GameFacade.Instance.DataCenter.User.GetPropertyLevel(property);
            m_Bars[property].ForEach(bar_trans => {
                bar_trans.SetValue(level);
            });
        }
    }
}
