using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Description;
    [SerializeField] Button m_BtnUpgrade;
    [SerializeField] TextMeshProUGUI m_Cost;


    private SkillData m_SkillData;
    private int m_SkillLevel;



    void Start()
    {
        m_BtnUpgrade.onClick.AddListener(()=>{
            // int cost = Skill.GetCost(m_SkillData, m_SkillSlot.Level);

            // if (DataCenter.Instance.User.Glass < cost)
            // {
            //     EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=1>不足"));
            //     return;
            // }

            // GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.SKILLUP, transform.position, new Vector3(100, 100, 100));
            
            // DataCenter.Instance.User.UpdateGlass(-cost);

            // DataCenter.Instance.User.UpgradeSkill(m_SkillSlot, m_SkillData.ID);

            // EventManager.SendEvent(new GameEvent(EVENT.UI_SKILLUPGRADE));
            // EventManager.SendEvent(new GameEvent(EVENT.ONUPDATEGLASS));
        });
    }

    public void Init(SkillData skill_data, int level, UnityAction callback)
    {
        m_SkillData     = skill_data;
        m_SkillLevel    = level;

        m_BtnUpgrade.onClick.AddListener(callback);

        FlushUI();
    }

    void FlushUI()
    {
        m_Text.text     = m_SkillData.Name;
        m_Icon.sprite   = GameFacade.Instance.AssetManager.LoadSprite("Skills/" + DataCenter.Instance.User.CurrentPlayer.ID, m_SkillData.ID.ToString()); 
        m_Icon.SetNativeSize();

        m_Description.text = Skill.CompareDescription(m_SkillData, m_SkillLevel, m_SkillLevel + 1);
    }

    public void ShowBtnUpgrade(bool flag)
    {
        m_BtnUpgrade.gameObject.SetActive(flag);
    }

    public void Focus(bool flag)
    {
        // m_Icon.GetComponent<ImageGray>().TurnGray(!flag);

        // if (flag == true)
        // {
        //     m_Text.color        = new Color(255/255f, 220/255f, 64/255f, 1);
        //     m_Description.color = Color.white;
        // }
        // else
        // {
        //     m_Text.color        = Color.gray;
        //     m_Description.color = Color.gray;
        // }
    }
}
