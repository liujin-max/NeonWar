using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeItem : MonoBehaviour
{
    [Header("按钮")]
    [SerializeField] Button m_BtnATK;
    [SerializeField] Button m_BtnASP;
    [SerializeField] Button m_BtnWORTH;




    void Awake()
    {
        //升级攻击力
        m_BtnATK.onClick.AddListener(()=>{
            if (UpgradeProperty(_C.PROPERTY.ATK)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.ATKUP, m_BtnATK.transform.position);
        });

        //升级攻速
        m_BtnASP.onClick.AddListener(()=>{
            if (UpgradeProperty(_C.PROPERTY.ASP)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.ASPUP, m_BtnASP.transform.position);

        });

        //升级价值
        m_BtnWORTH.onClick.AddListener(()=>{
            if (UpgradeProperty(_C.PROPERTY.WORTH)) GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.WORUP, m_BtnWORTH.transform.position);
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

        EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));

        return true;
    }


    public void Init()
    {
        FlushUI();
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

    public void FlushUI()
    {
        InitUpgradePivot();
    }
}
