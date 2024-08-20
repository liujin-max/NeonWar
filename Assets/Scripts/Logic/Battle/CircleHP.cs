using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CircleHP : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_White;
    [SerializeField] private SpriteRenderer m_Red;
    [SerializeField] private NumberTransition m_HPText;
    [SerializeField] private Transform m_BarPivot;
    [SerializeField] private Transform m_ShakePivot;
    // [SerializeField] private Transform m_ShadowPivot;
    [SerializeField] private float m_Height;


    private Enemy m_Enemy;
    private ValueTransition m_WhiteValue = new ValueTransition(0.3f, 0.5f);
    private ValueTransition m_RedValue = new ValueTransition(1f, 0.15f);

    private Color GREEN     = new Color(83/255f, 202/255f, 0/255f, 1);
    private Color YELLOW    = new Color(255/255f, 151/255f, 4/255f, 1);
    private Color RED       = new Color(247/255f, 66/255f, 11/255f, 1);


    private Tweener m_HPTweener;
    private List<Tweener> m_ShakeTweeners = new List<Tweener>();
    private bool m_HitShaking = false;

    public void Init(Enemy enemy)
    {
        m_Enemy = enemy;

        int hp  = m_Enemy.ATT.HP;
        m_HPText.ForceValue(hp);
        m_WhiteValue.ForceValue(hp);
        m_RedValue.ForceValue(hp);

        m_Red.color = GREEN;

        for (int i = 0; i < m_ShakePivot.childCount; i++)
        {
            m_ShakePivot.GetChild(i).GetComponent<TextMeshPro>().text = m_HPText.GetText();
        }
    }


    public void FlushHP()
    {
        int hp  = m_Enemy.ATT.HP;
        m_HPText.SetValue(hp);
        m_WhiteValue.SetValue(hp);
        m_RedValue.SetValue(hp);

        if (m_HPTweener != null) {
            m_HPText.transform.localPosition  = Vector3.zero;
            m_HPTweener.Kill();
        }
        
        m_HPTweener = m_HPText.transform.DOShakePosition(0.2f, new Vector3(0.03f, 0.03f, 0), 50);


        for (int i = 0; i < m_ShakePivot.childCount; i++)
        {
            var hp_text = m_ShakePivot.GetChild(i).GetComponent<TextMeshPro>();
            hp_text.transform.localPosition  = Vector3.zero;
            hp_text.text= m_HPText.GetText();

            var tweener = hp_text.transform.DOShakePosition(0.4f, new Vector3(0.1f, 0.1f, 0), 50);

            if (m_ShakeTweeners.Count > i) 
            {
                m_ShakeTweeners[i].Kill();
                m_ShakeTweeners[i] = tweener;
            }
            else m_ShakeTweeners.Add(tweener);
        }
    }

    public void Affected()
    {
        if (m_HitShaking) return;
        m_HitShaking = true;
        m_BarPivot.transform.localScale = Vector3.one;
        m_BarPivot.transform.DOShakeScale(0.3f, 0.3f, vibrato: 35, randomness: 50, fadeOut: true).OnComplete(()=>{
            m_HitShaking = false;
        });
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        m_WhiteValue.Update(deltaTime);
        m_RedValue.Update(deltaTime);
    }

    void LateUpdate()
    {
        float hp_rate   = m_RedValue.Value / (float)m_Enemy.ATT.HPMAX;
        m_White.size    = new Vector2(m_White.size.x, m_Height * m_WhiteValue.Value / (float)m_Enemy.ATT.HPMAX);
        m_Red.size      = new Vector2(m_Red.size.x, m_Height * hp_rate);

        if (hp_rate >= 0.8f)
        {
            m_Red.color = GREEN;
        }
        else if (hp_rate >=0.3f)
        {
            m_Red.color = YELLOW;
        }
        else
        {
            m_Red.color = RED;
        }

        for (int i = 0; i < m_ShakePivot.childCount; i++) {
            m_ShakePivot.GetChild(i).GetComponent<TextMeshPro>().text= m_HPText.GetText();
        }

        // m_ShadowPivot.transform.localPosition = m_Enemy.transform.localPosition / 25f;
    }
}
