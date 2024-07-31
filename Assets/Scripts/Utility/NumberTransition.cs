using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberTransition : MonoBehaviour
{
    public float m_SpeedMin = 0.3f;
    public bool m_IsFormat = false;


    private int m_TargetNumber = 0;
    private float m_CurrentNumber = 0;
    private float m_OriginNumber = 0;
    private float m_Offset = 0;



    private TextMeshProUGUI m_TextUGUI;
    private TextMeshPro m_TextMesh;
    private Text m_Text;

    void Awake()
    {
        m_TextUGUI  = transform.GetComponent<TextMeshProUGUI>();
        m_TextMesh  = transform.GetComponent<TextMeshPro>();
        m_Text      = transform.GetComponent<Text>();
    }

    void SetText(int value)
    {
        if (m_IsFormat == true)
        {
            if (m_TextUGUI != null) m_TextUGUI.text = ToolUtility.FormatNumber(value);
            if (m_TextMesh != null) m_TextMesh.text = ToolUtility.FormatNumber(value);
            if (m_Text != null) m_Text.text = ToolUtility.FormatNumber(value);
        }
        else
        {
            if (m_TextUGUI != null) m_TextUGUI.text = value.ToString();
            if (m_TextMesh != null) m_TextMesh.text = value.ToString();
            if (m_Text != null) m_Text.text = value.ToString();
        }
    }

    public void SetValue(int value)
    {
        m_TargetNumber  = value;
        m_Offset        = m_TargetNumber - m_CurrentNumber;
        m_OriginNumber  = m_CurrentNumber;

        SetText((int)m_CurrentNumber);
    }

    public void ForceValue(int value)
    {
        m_TargetNumber  = value;
        m_CurrentNumber = value;
        m_OriginNumber  = value;

        SetText((int)m_CurrentNumber);
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

        SetText((int)m_CurrentNumber);
    }
}
