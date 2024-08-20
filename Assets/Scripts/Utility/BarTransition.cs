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


    void Awake()
    {
        m_Image = transform.GetComponent<Image>();
    }

    public void Init(float current, float max)
    {
        m_ValueMax = max;
        m_Transition.ForceValue(current);

        FlushBar();
    }

    public void SetValue(float value)
    {
        m_Transition.SetValue(value);
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
