using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueTransition
{
    public float m_SpeedMin = 0.03f;
    public float m_Time = 0.2f;
    
    private float m_TargetNumber = 0;
    private float m_CurrentNumber = 0;
    private float m_OriginNumber = 0;
    private float m_Offset = 0;


    [HideInInspector] public float Value {get { return m_CurrentNumber;}}


    public ValueTransition(float speed, float time)
    {
        m_SpeedMin = speed;
        m_Time = time;
    }

    public void SetValue(float value)
    {
        m_TargetNumber  = value;
        m_Offset        = m_TargetNumber - m_CurrentNumber;
        m_OriginNumber  = m_CurrentNumber;
    }

    public void ForceValue(int value)
    {
        m_TargetNumber  = value;
        m_CurrentNumber = value;
        m_OriginNumber  = value;
    }

    public void Update(float deltaTime)
    {
        if (m_TargetNumber == (int)m_CurrentNumber) return;

        
        var speed   = m_Offset * deltaTime / m_Time;
        if (speed > 0) {
            speed   = Math.Max(m_SpeedMin, speed);
        } else {
            speed   = Math.Min(-m_SpeedMin, speed);
        }

        m_CurrentNumber += speed;

        var offset  = m_TargetNumber - m_OriginNumber;
        if (offset > 0) {
            if (m_CurrentNumber >= m_TargetNumber) {
                m_CurrentNumber = m_TargetNumber;
            }
        } else {
            if (m_CurrentNumber <= m_TargetNumber) {
                m_CurrentNumber = m_TargetNumber;
            }
        }
    }
}
