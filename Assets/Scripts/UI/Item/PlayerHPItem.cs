using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPItem : MonoBehaviour
{
    [SerializeField] Transform m_HPPivot;


    Player m_Caster;

    private List<PlayerHPPointItem> m_PointItems = new List<PlayerHPPointItem>();

    PlayerHPPointItem new_point_item(int order)
    {
        PlayerHPPointItem item = null;
        if (m_PointItems.Count > order)
        {
            item = m_PointItems[order];
        }
        else
        {
            item = GameFacade.Instance.UIManager.LoadItem("PlayerHPPointItem", m_HPPivot).GetComponent<PlayerHPPointItem>();
            m_PointItems.Add(item);
        }
        item.gameObject.SetActive(true);

        return item;
    }

    public void Init(Player caster)
    {
        m_Caster = caster;

        InitPoints();
    }

    void InitPoints()
    {
        m_PointItems.ForEach(item => item.gameObject.SetActive(false));

        for (int i = 0; i < m_Caster.ATT.HPMAX; i++)
        {
            var item = new_point_item(i);
            item.Init(i + 1, m_Caster.ATT.HPMAX);
            item.Green();
        }
    }

    public void FlushHP()
    {   
        float hp_rate = m_Caster.ATT.HP / (float)m_Caster.ATT.HPMAX;

        for (int i = 0; i < m_Caster.ATT.HPMAX; i++)
        {
            var item = new_point_item(i);
            item.Init(i + 1, m_Caster.ATT.HPMAX);
            item.ShowPoint((i + 1) <= m_Caster.ATT.HP);

            if (hp_rate >= 0.8f) item.Green();
            else if (hp_rate >= 0.4f) item.Yellow();
            else item.Red();
        }
    }

    public void Show(bool flag)
    {
        gameObject.SetActive(flag);
    }
}
