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
    private float m_TimeCount = 0;
    
    private float m_TargetNumber = 0;
    private float m_CurrentNumber = 0;
    public float Current {get { return m_CurrentNumber; } }


    [HideInInspector] public float Value {get { return m_CurrentNumber;}}


    public ValueTransition(float speed, float time)
    {
        m_SpeedMin  = speed;
        m_Time      = time;
        m_TimeCount = 0;
    }

    public void SetValue(float value)
    {
        m_TargetNumber  = value;
        m_TimeCount     = 0;
    }

    public void ForceValue(float value)
    {
        m_TargetNumber  = value;
        m_CurrentNumber = value;
    }

    public bool Update(float deltaTime)
    {
        if (m_TargetNumber == m_CurrentNumber) return false;

        m_TimeCount += deltaTime;


        m_CurrentNumber = Mathf.Lerp(m_CurrentNumber, m_TargetNumber, m_TimeCount / m_Time);

        return true;
    }
}
