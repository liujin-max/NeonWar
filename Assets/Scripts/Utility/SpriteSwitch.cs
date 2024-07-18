using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteSwitch : MonoBehaviour
{

    [SerializeField] private SpriteRenderer m_Sprite1;
    [SerializeField] private SpriteRenderer m_Sprite2;

    
    private SpriteRenderer m_CurSprite;

    void Awake()
    {
        m_Sprite1.gameObject.SetActive(true);
        m_Sprite2.gameObject.SetActive(true);

        m_CurSprite = null;
    }

    SpriteRenderer GetSprite(bool is_current)
    {
        if (m_CurSprite == null) 
        {
            if (is_current == true) return m_Sprite1;

            return m_Sprite2;
        }

        if (is_current) 
        {
            return m_CurSprite;
        }
        else
        {
            if (m_Sprite1 == m_CurSprite) {
                return m_Sprite2;
            }
        }

        return m_Sprite1;
    }

    public void SetSprite(string res, bool is_immediately = false)
    {
        float time  = is_immediately == true ? 0f : 0.6f;

        var cur     = GetSprite(true);
        var op      = GetSprite(false);

        if (m_CurSprite == null)
        {
            cur.sprite  = Resources.Load<Sprite>(res);
            m_CurSprite = cur;

            cur.color   = new Color(1, 1, 1, 1);
            op.color    = new Color(1, 1, 1, 0);
        }
        else
        {
            op.sprite   = Resources.Load<Sprite>(res);
            m_CurSprite = op;

            cur.DOFade(0, time);
            op.DOFade(1, time);
        }
    }
}
