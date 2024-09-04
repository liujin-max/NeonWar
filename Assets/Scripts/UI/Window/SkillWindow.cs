using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    [SerializeField] Transform m_SkillPivot;
    [SerializeField] Button m_BtnReset;



    private List<SkillItem> m_SkillItems = new List<SkillItem>();

    SkillItem new_skill_item(int order)
    {
        SkillItem skill_item = null;
        if (m_SkillItems.Count > order)
        {
            skill_item = m_SkillItems[order];
        }
        else
        {
            skill_item = GameFacade.Instance.UIManager.LoadItem("SkillItem", m_SkillPivot).GetComponent<SkillItem>();
            m_SkillItems.Add(skill_item);
        }
        skill_item.gameObject.SetActive(true);

        return skill_item;
    }

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);


        //刷新技能
        m_BtnReset.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                int[] skill_ids = State_Upgrade<Field>.GenerateSkillsPool();

                Init(skill_ids);
            });
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_SKILLUPGRADE,  OnSkillUpgrade);
    }



    public void Init(int[] skill_ids)
    {
        SoundManager.Instance.Load(SOUND.SHINE);
        
        InitSkills(skill_ids);
    }

    public void InitSkills(int[] skill_ids)
    {
        m_SkillItems.ForEach(item => {item.gameObject.SetActive(false); });


        for (int i = 0; i < skill_ids.Length; i++)
        {
            int sk_id   = skill_ids[i];
            if (sk_id == 0) continue;
            
            var skill   = Field.Instance.Player.GetSkill(sk_id) ;
            int level   = skill != null ? skill.Level : 0;

            SkillData skill_data = DataCenter.Instance.League.GetSkillData(sk_id);
            var item = new_skill_item(i);
            item.Init(skill_data, level, ()=>{
                Field.Instance.Player.AddSkill(skill_data, level + 1);
                Field.Instance.NextWave();

                GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.SKILLUP, item.transform.position);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }
    }

    


    #region 监听事件
    void OnSkillUpgrade(GameEvent @event)
    {
        // var data = DataCenter.Instance.League.GetSkillData(m_SkillSlot.ID);
        // this.Init(m_SkillSlot, data);
    }
    #endregion
}
