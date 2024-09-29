using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Floating : IconAnim
{
    public float Height = 0.5f;
    public float Time   = 1.5f;


    private Vector3 m_Origin;
    private Tweener m_Tweener;

    void Awake()
    {
        m_Origin = transform.localPosition;
    }


    public override void Play()
    {
        if (m_Tweener == null)
        {
            // 创建浮动动画
            m_Tweener = transform.DOLocalMoveY(m_Origin.y + Height, Time);
            m_Tweener.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        
        m_Tweener.Play();
    }

    public override void Stop()
    {
        if (m_Tweener != null) m_Tweener.Pause();
        transform.localPosition = m_Origin;
    }
}
