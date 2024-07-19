using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float longPressDuration = 0.01f; // 长按触发时间（秒）
    private bool isPointerDown = false;
    private float pointerDownTimer = 0;

    private Action m_DownCallback;
    private Action m_UpCallback;
    private Action m_PressCallback;

    public void SetCallback(Action down_callback, Action up_callback, Action press_callback)
    {
        m_DownCallback  = down_callback;
        m_UpCallback    = up_callback;
        m_PressCallback = press_callback;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0;

        if (m_DownCallback != null) m_DownCallback();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        Reset();

        if (m_UpCallback != null) m_UpCallback();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
        Reset();
    }

    void Update()
    {
        if (isPointerDown) {
            pointerDownTimer += Time.deltaTime;

            if (pointerDownTimer >= longPressDuration)
            {
                OnLongPress();
            }
        }
    }

    private void Reset()
    {
        isPointerDown = false;
        pointerDownTimer = 0;
    }

    private void OnLongPress()
    {
        if (m_PressCallback != null) m_PressCallback();
    }
}
