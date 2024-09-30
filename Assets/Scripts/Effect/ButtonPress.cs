using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//按钮按下时，会切换成按下后压扁的图片，此时按钮内部的一些子物体为了不穿帮，也要调整Y坐标
public class ButtonPress : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Transform m_MovePivot;
    [SerializeField] float m_MoveY;
    [SerializeField] bool m_PlaySound = true;

    float m_OriginY;

    void Start()
    {
        m_OriginY = m_MovePivot.transform.localPosition.y;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_PlaySound) SoundManager.Instance.Load(SOUND.CLICK);

        m_MovePivot.transform.localPosition = new Vector3(m_MovePivot.transform.localPosition.x, m_MoveY, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_MovePivot.transform.localPosition = new Vector3(m_MovePivot.transform.localPosition.x, m_OriginY, 0);
    }

}
