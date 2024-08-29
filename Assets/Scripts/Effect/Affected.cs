// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;

// public class Affected : MonoBehaviour
// {
//     private SpriteRenderer m_Sprite;
//     private Material m_Mat = null;


//     private bool m_IsDoing = false;
//     private float m_Value;


//     void Awake()
//     {
//         m_Sprite = GetComponent<SpriteRenderer>();
//     }

//     public void DoAnimation()
//     {
//         if (m_IsDoing) return;

//         m_IsDoing = true;

//         m_Sprite.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.2f, 50).OnComplete(()=>{m_IsDoing = false;});
//         m_Sprite.DOColor(Color.red, 0.12f).OnComplete(()=>{
//             m_IsDoing = false;
//             m_Sprite.DOColor(Color.white, 0.08f);
//         });

        
//     }
// }



using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Affected : MonoBehaviour
{
    private SpriteRenderer m_Sprite;
    public Material m_MatTemplate;
    private Material m_Mat = null;
    private MaterialPropertyBlock m_Mpb;
    private int m_BlendID;
    private int m_ColorID;

    private bool m_IsDoing = false;
    private float m_Value;
    private Hit m_Hit;

    void Awake()
    {
        m_Sprite= GetComponent<SpriteRenderer>();
        m_Sprite.material = m_MatTemplate;


        m_Mpb   = new MaterialPropertyBlock();
        m_Mpb.SetTexture("_MainTex", m_Sprite.sprite.texture);

        m_BlendID = Shader.PropertyToID("_HitEffectBlend");
        m_ColorID = Shader.PropertyToID("_HitEffectColor");
    }

    public void DO(Hit hit)
    {
        m_Hit = hit;
        if (m_IsDoing) return;

        m_IsDoing   = true;
        m_Value     = 1;

        m_Mpb.SetColor(m_ColorID, m_Hit.HitColor);
    }


    void Update()
    {
        if (!m_IsDoing) return;

        m_Value -= Time.deltaTime * 5;

        m_Mpb.SetFloat(m_BlendID, m_Value);
        m_Sprite.SetPropertyBlock(m_Mpb);
        
        if (m_Value <= 0) {
            m_IsDoing = false;
        }
    }
}