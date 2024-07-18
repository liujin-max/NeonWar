using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClickHandler : MonoBehaviour, IPointerDownHandler
{
    private Action m_DownCallback;
    
    public void Init(Action down_callback)
    {
        m_DownCallback = down_callback;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_DownCallback != null)
        {
            m_DownCallback.Invoke();
        }
    }
}
