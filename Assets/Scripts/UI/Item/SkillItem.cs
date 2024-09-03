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
}
