using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarTransition : MonoBehaviour
{
    private Image m_Image;
    private ValueTransition m_Transition = new ValueTransition(0.03f, 0.3f);
    private float m_ValueMax;


    private float LowerValue;

    void Awake()
    {
        m_Image = transform.GetComponent<Image>();
    }

    public void Init(float current, float max, float lowervalue = 0)
    {
        m_ValueMax = max - lowervalue;
        m_Transition.ForceValue(Mathf.Max(0, current - lowervalue));

        LowerValue = lowervalue;

        FlushBar();
    }

    public void SetValue(float value)
    {
        m_Transition.SetValue(Mathf.Max(0, value - LowerValue));
    }

    void FlushBar()
    {
        m_Image.fillAmount = m_Transition.Current / m_ValueMax;
    }

    void Update()
    {
        if (m_Transition.Update(Time.deltaTime))
        {
            FlushBar();
        }
    }
}
