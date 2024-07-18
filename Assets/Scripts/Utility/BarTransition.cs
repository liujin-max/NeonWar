using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarTransition : MonoBehaviour
{
    public float m_SpeedMin = 0.03f;


    private float m_TargetNumber = 0;
    private float m_CurrentNumber = 0;
    private float m_OriginNumber = 0;
    private float m_Offset = 0;



    private Image m_Image;


    void Awake()
    {
        m_Image = transform.GetComponent<Image>();

    }


    public void SetValue(float value)
    {
        m_TargetNumber  = value;
        m_Offset        = m_TargetNumber - m_CurrentNumber;
        m_OriginNumber  = m_CurrentNumber;

        m_Image.fillAmount = m_CurrentNumber;
    }

    public void ForceValue(int value)
    {
        m_TargetNumber  = value;
        m_CurrentNumber = value;
        m_OriginNumber  = value;

        m_Image.fillAmount = m_CurrentNumber;
    }

    void Update()
    {
        if (m_TargetNumber == (int)m_CurrentNumber) return;

        
        var speed   = m_Offset * Time.deltaTime / 0.2f;
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

        m_Image.fillAmount = m_CurrentNumber;
    }
}
