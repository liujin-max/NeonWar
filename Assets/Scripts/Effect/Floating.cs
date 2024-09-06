using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public bool AutoStart = true;
    public float Height = 0.5f;
    public float Time   = 1.5f;


    private Vector3 m_Origin;
    private Tweener m_Tweener;

    void Awake()
    {
        m_Origin = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AutoStart) Play();
    }

    public void Play()
    {
        if (m_Tweener == null)
        {
            // 计算从startTime开始时的进度
            float progress = 0; //RandomUtility.Random(0, 100) / 100.0f;
            // 创建浮动动画
            m_Tweener = transform.DOMoveY(m_Origin.y + Height, Time);
            m_Tweener.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Goto(progress, true);
        }
        
        m_Tweener.Play();
    }

    public void Stop()
    {
        if (m_Tweener != null) m_Tweener.Pause();
        transform.position = m_Origin;
    }
}
