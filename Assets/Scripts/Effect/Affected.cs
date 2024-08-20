using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Affected : MonoBehaviour
{
    private SpriteRenderer m_Sprite;
    private Material m_Mat = null;


    private bool m_IsDoing = false;
    private float m_Value;

    void Awake()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
    }

    public void DoAnimation()
    {
        if (m_IsDoing) return;

        m_IsDoing = true;

        m_Sprite.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.2f, 50).OnComplete(()=>{m_IsDoing = false;});
        m_Sprite.DOColor(Color.red, 0.12f).OnComplete(()=>{
            m_IsDoing = false;
            m_Sprite.DOColor(Color.white, 0.08f);
        });
    }
}
