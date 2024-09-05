using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//负责怪物Sprite的各种变化
public class Affected : MonoBehaviour
{
    private SpriteRenderer m_Sprite;
    public Material m_MatTemplate;
    private MaterialPropertyBlock m_Mpb;
   


    //打击相关
    private int m_HitID;
    private int m_ColorID;
    private bool m_IsHiting = false;
    private float m_Value;
    private Hit m_Hit;

    //冻结
    private int m_FrozenID;


    void Awake()
    {
        m_Sprite= GetComponent<SpriteRenderer>();
        m_Sprite.material = m_MatTemplate;


        m_Mpb   = new MaterialPropertyBlock();
        m_Mpb.SetTexture("_MainTex", m_Sprite.sprite.texture);

        m_HitID     = Shader.PropertyToID("_HitEffectBlend");
        m_ColorID   = Shader.PropertyToID("_HitEffectColor");

        m_FrozenID  = Shader.PropertyToID("_InnerOutlineThickness");
    }

    //受击
    public void DoHit(Hit hit)
    {
        m_Hit = hit;
        if (m_IsHiting) return;

        m_IsHiting   = true;
        m_Value     = 1;

        m_Mpb.SetColor(m_ColorID, m_Hit.HitColor); 
    }

    //冻结
    public void Frozen(bool flag)
    {
        m_Mpb.SetFloat(m_FrozenID, flag ? 2 : 0);
        m_Sprite.SetPropertyBlock(m_Mpb);
    }




    void Update()
    {
        if (!m_IsHiting) return;

        m_Value -= Time.deltaTime * 5;

        m_Mpb.SetFloat(m_HitID, m_Value);
        m_Sprite.SetPropertyBlock(m_Mpb);
        
        if (m_Value <= 0) {
            m_IsHiting = false;
        }
    }
}