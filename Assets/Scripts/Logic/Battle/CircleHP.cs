using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHP : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Yellow;
    [SerializeField] private SpriteRenderer m_Red;
    [SerializeField] private NumberTransition m_HPText;
    [SerializeField] private float m_Height;


    private Enemy m_Enemy;
    private ValueTransition m_YellowValue = new ValueTransition(0.3f, 0.4f);
    private ValueTransition m_RedValue = new ValueTransition(1f, 0.15f);


    public void Init(Enemy enemy)
    {
        m_Enemy = enemy;

        int hp  = m_Enemy.ATT.HP;
        m_HPText.ForceValue(hp);
        m_YellowValue.ForceValue(hp);
        m_RedValue.ForceValue(hp);
    }

    public void FlushHP()
    {
        int hp  = m_Enemy.ATT.HP;
        
        m_HPText.SetValue(hp);
        m_YellowValue.SetValue(hp);
        m_RedValue.SetValue(hp);
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
    }
}
