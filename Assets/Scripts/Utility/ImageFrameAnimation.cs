using UnityEngine;
using UnityEngine.UI;

public class ImageFrameAnimation : MonoBehaviour
{
    private Image m_Image;              // 需要播放帧动画的Image组件
    public Sprite[] m_Frames;           // 动画帧的Sprite数组
    public float m_FrameRate = 0.1f;    // 每帧显示时间（秒）

    public int m_InitFrame = 0;         //初始帧 从0开始
    private int m_CurrentFrame;        // 当前帧的索引
    private float m_Timer;             // 帧动画计时器

    void Awake()
    {
        m_Image = GetComponent<Image>();

        m_CurrentFrame  = m_InitFrame;
    }

    void Update()
    {
        if (m_Frames.Length == 0) return;

        m_Timer += Time.deltaTime;

        if (m_Timer >= m_FrameRate)
        {
            m_Timer -= m_FrameRate;
            m_CurrentFrame = (m_CurrentFrame + 1) % m_Frames.Length;
            m_Image.sprite = m_Frames[m_CurrentFrame]; // 切换到下一帧
        }
    }
}
