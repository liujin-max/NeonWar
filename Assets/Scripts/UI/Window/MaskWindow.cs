using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskWindow : MonoBehaviour
{
    [SerializeField] private GameObject m_Mask;


    private int m_MaskCount = 0;
    void Awake()
    {
        m_Mask.SetActive(false);
        
        EventManager.AddHandler(EVENT.UI_POPUPMASK,     OnPopUpMask);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_POPUPMASK,     OnPopUpMask);
    }

    private void OnPopUpMask(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        m_MaskCount += flag ? 1 : -1;

        m_Mask.SetActive(m_MaskCount > 0);
    }
}
