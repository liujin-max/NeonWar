using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskWindow : BaseWindow
{
    [SerializeField] private GameObject m_Mask;


    private int m_MaskCount = 0;
    void Awake()
    {
        m_Mask.SetActive(false);
    }

    void OnEnable()
    {
        Event_PopupMask.OnEvent += OnPopUpMask;
    }

    void OnDisable()
    {
        Event_PopupMask.OnEvent -= OnPopUpMask;
    }


    private void OnPopUpMask(Event_PopupMask e)
    {
        m_MaskCount += e.IsShow ? 1 : -1;

        m_Mask.SetActive(m_MaskCount > 0);
    }
}
