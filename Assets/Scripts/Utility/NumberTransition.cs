using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberTransition : MonoBehaviour
{
    public bool m_IsFormat = false;

    private ValueTransition m_ValueTransition = new ValueTransition(1f, 0.3f);



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

    public string GetText()
    {
        if (m_TextUGUI != null) return m_TextUGUI.text;
        if (m_TextMesh != null) return m_TextMesh.text;
        if (m_Text != null) return m_Text.text;

        return "";
    }

    public void SetValue(int value)
    {
        m_ValueTransition.SetValue(value);

        SetText((int)m_ValueTransition.Value);
    }

    public void ForceValue(int value)
    {
        m_ValueTransition.ForceValue(value);

        SetText((int)m_ValueTransition.Value);
    }

    void Update()
    {
        if (!m_ValueTransition.Update(Time.deltaTime)) return;

        SetText((int)m_ValueTransition.Value);
    }
}
