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
    [SerializeField] Button m_BtnWORTH;


    Dictionary<_C.PROPERTY, List<BarTransition>> m_Bars = new Dictionary<_C.PROPERTY, List<BarTransition>>()
    {
        [_C.PROPERTY.ATK]   = new List<BarTransition>(),
        [_C.PROPERTY.ASP]   = new List<BarTransition>(),
        [_C.PROPERTY.WORTH] = new List<BarTransition>()
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
            if (UpgradeProperty(_C.PROPERTY.ATK)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.ATKUP, m_BtnATK.transform.position, new Vector3(100, 100, 100));
        });

        //升级攻速
        m_BtnASP.onClick.AddListener(()=>{
            if (UpgradeProperty(_C.PROPERTY.ASP)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.ASPUP, m_BtnASP.transform.position, new Vector3(100, 100, 100));

        });

        //升级价值
        m_BtnWORTH.onClick.AddListener(()=>{
            if (UpgradeProperty(_C.PROPERTY.WORTH)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.WORUP, m_BtnWORTH.transform.position, new Vector3(100, 100, 100));
        });
    }

    bool UpgradeProperty(_C.PROPERTY property)
    {
        int cost = DataCenter.Instance.User.GetPropertyCost(property);
        if (DataCenter.Instance.User.Glass < cost) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
            return false;
        }

        DataCenter.Instance.User.UpdateProperty(property, 1);
        DataCenter.Instance.User.UpdateGlass(-cost);

        FlushUI();
        FlushBars(property);

        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));

        return true;
    }


    public void Init()
    {
        FlushUI();
        InitLines();
    }

    void InitUpgradePivot()
    {
        int atk_level   = DataCenter.Instance.User.CurrentPlayer.ATK;
        int asp_level   = DataCenter.Instance.User.CurrentPlayer.ASP;
        int wor_level   = DataCenter.Instance.User.CurrentPlayer.WORTH;

        int atk_cost    = DataCenter.Instance.User.GetPropertyCost(_C.PROPERTY.ATK);
        int asp_cost    = DataCenter.Instance.User.GetPropertyCost(_C.PROPERTY.ASP);
        int wor_cost    = DataCenter.Instance.User.GetPropertyCost(_C.PROPERTY.WORTH);

        string atk_color= DataCenter.Instance.User.Glass >= atk_cost ? _C.COLOR_GREEN : _C.COLOR_RED;
        string asp_color= DataCenter.Instance.User.Glass >= asp_cost ? _C.COLOR_GREEN : _C.COLOR_RED;
        string wor_color= DataCenter.Instance.User.Glass >= wor_cost ? _C.COLOR_GREEN : _C.COLOR_RED;

        m_BtnATK.transform.Find("Name").GetComponent<TextMeshProUGUI>().text            = "攻击";
        m_BtnATK.transform.Find("LevelPivot/Text").GetComponent<TextMeshProUGUI>().text = atk_level.ToString();
        m_BtnATK.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text  = string.Format("<sprite=1>{0}{1}", atk_color, ToolUtility.FormatNumber(atk_cost));

        m_BtnASP.transform.Find("Name").GetComponent<TextMeshProUGUI>().text            = "攻速";
        m_BtnASP.transform.Find("LevelPivot/Text").GetComponent<TextMeshProUGUI>().text = asp_level.ToString();
        m_BtnASP.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text  = string.Format("<sprite=1>{0}{1}", asp_color, ToolUtility.FormatNumber(asp_cost));

        m_BtnWORTH.transform.Find("Name").GetComponent<TextMeshProUGUI>().text              = "价值";
        m_BtnWORTH.transform.Find("LevelPivot/Text").GetComponent<TextMeshProUGUI>().text   = wor_level.ToString();
        m_BtnWORTH.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text    = string.Format("<sprite=1>{0}{1}", wor_color, ToolUtility.FormatNumber(wor_cost));
    }

    void InitSkills()
    {
        m_SkillSeatItems.ForEach(item => {item.gameObject.SetActive(false);});


        List<SkillSlotMsg> slots = DataCenter.Instance.User.CurrentPlayer.SkillSlots;

        for (int i = 0; i < slots.Count; i++)
        {
            SkillSlotMsg skill_msg = slots[i];
            SkillData skill_data = DataCenter.Instance.League.GetSkillData(skill_msg.ID);

            var item = new_skill_seat_item(i, m_SkillPivot.Find(skill_msg.POS));
            item.Init(skill_msg, skill_data);
            item.SetTextUp(skill_msg.POS.Equals("1"));
        }
    }


    void InitLines()
    {
        PlayerMsg player =  DataCenter.Instance.User.CurrentPlayer;

        //攻击
        {
            float last_value = 1;

            var bar1 = m_LinePivot.Find("ATK1/Bar").GetComponent<BarTransition>();
            bar1.Init(player.ATK, player.SkillSlots[0].ATK, last_value);
            m_Bars[_C.PROPERTY.ATK].Add(bar1);

            last_value = player.SkillSlots[0].ATK;

            var bar2 = m_LinePivot.Find("ATK2/Bar").GetComponent<BarTransition>();
            bar2.Init(player.ATK , player.SkillSlots[1].ATK , last_value);
            m_Bars[_C.PROPERTY.ATK].Add(bar2);
        }

        //攻速
        {
            float last_value = 1;

            var bar1 = m_LinePivot.Find("ASP1/Bar").GetComponent<BarTransition>();
            bar1.Init(player.ASP, player.SkillSlots[2].ASP, last_value);
            m_Bars[_C.PROPERTY.ASP].Add(bar1);

            last_value = player.SkillSlots[2].ASP;

            var bar2 = m_LinePivot.Find("ASP2/Bar").GetComponent<BarTransition>();
            bar2.Init(player.ASP, player.SkillSlots[3].ASP, last_value);
            m_Bars[_C.PROPERTY.ASP].Add(bar2);
        }

        //价值
        {
            float last_value = 1;

            var bar1 = m_LinePivot.Find("WOR1/Bar").GetComponent<BarTransition>();
            bar1.Init(player.WORTH, player.SkillSlots[4].WOR, last_value);
            m_Bars[_C.PROPERTY.WORTH].Add(bar1);

            last_value = player.SkillSlots[4].WOR;

            var bar2 = m_LinePivot.Find("WOR2/Bar").GetComponent<BarTransition>();
            bar2.Init(player.WORTH, player.SkillSlots[5].WOR, last_value);
            m_Bars[_C.PROPERTY.WORTH].Add(bar2);
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
            int level = DataCenter.Instance.User.GetPropertyLevel(property);
            m_Bars[property].ForEach(bar_trans => {
                bar_trans.SetValue(level);
            });
        }
    }
}
