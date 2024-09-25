using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb
{
    public Unit Caster;

    private Vector2 m_Center;
    private float m_Radius;
    private float m_ATKInc;
    private string m_EffectRes;

    public Bomb(Unit caster, Vector2 center, float radius, float atk_inc, string effect_res)
    {
        Caster      = caster;

        m_Center    = center;
        m_Radius    = radius;
        m_ATKInc    = atk_inc;
        m_EffectRes = effect_res;
    }

    public void Do()
    {
        if (!string.IsNullOrEmpty(m_EffectRes))
        {
            var e = GameFacade.Instance.EffectManager.Load(m_EffectRes, m_Center, Field.Instance.Land.ELEMENT_ROOT.gameObject);
            e.transform.localScale = new Vector3(m_Radius, m_Radius, 1);
        }
        
        foreach (var e in Field.Instance.Spawn.Enemys)
        {
            if (!e.IsValid) continue;

            if (Vector2.Distance(m_Center, e.transform.localPosition) <= m_Radius)
            {
                var hit = new Hit(Caster);
                hit.ATK_INC.PutMUL(this, m_ATKInc);
                hit.HitColor = Color.red;
                hit.CP.SetBase(0);

                Field.Instance.SettleHit(hit, e);
            }
        }
    }
}
