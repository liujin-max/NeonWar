using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CircleHP : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Yellow;
    [SerializeField] private SpriteRenderer m_Red;
    [SerializeField] private NumberTransition m_HPText;
    [SerializeField] private Transform m_ShakePivot;
    [SerializeField] private float m_Height;


    private Enemy m_Enemy;
    private ValueTransition m_YellowValue = new ValueTransition(0.3f, 0.4f);
    private ValueTransition m_RedValue = new ValueTransition(1f, 0.15f);

    private Tweener m_HPTweener;
    private List<Tweener> m_ShakeTweeners = new List<Tweener>();

    public void Init(Enemy enemy)
    {
        m_Enemy = enemy;

        int hp  = m_Enemy.ATT.HP;
        m_HPText.ForceValue(hp);
        m_YellowValue.ForceValue(hp);
        m_RedValue.ForceValue(hp);

        for (int i = 0; i < m_ShakePivot.childCount; i++)
        {
            m_ShakePivot.GetChild(i).GetComponent<TextMeshPro>().text = m_HPText.GetText();
        }
    }


    public void FlushHP()
    {
        int hp  = m_Enemy.ATT.HP;
        m_HPText.SetValue(hp);
        m_YellowValue.SetValue(hp);
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

            var tweener = hp_text.transform.DOShakePosition(0.4f, new Vector3(0.03f, 0.03f, 0), 50);

            if (m_ShakeTweeners.Count > i) 
            {
                m_ShakeTweeners[i].Kill();
                m_ShakeTweeners[i] = tweener;
            }
            else m_ShakeTweeners.Add(tweener);
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        m_YellowValue.Update(deltaTime);
        m_RedValue.Update(deltaTime);
    }

    void LateUpdate()
    {
        m_Yellow.size   = new Vector2(m_Yellow.size.x, m_Height * m_YellowValue.Value / (float)m_Enemy.ATT.HPMAX);
        m_Red.size      = new Vector2(m_Red.size.x, m_Height * m_RedValue.Value / (float)m_Enemy.ATT.HPMAX);

        for (int i = 0; i < m_ShakePivot.childCount; i++) {
            m_ShakePivot.GetChild(i).GetComponent<TextMeshPro>().text= m_HPText.GetText();
        }
    }
}
