using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPPointItem : MonoBehaviour
{
    [SerializeField] GameObject m_FrameLeft;
    [SerializeField] GameObject m_FrameCenter;
    [SerializeField] GameObject m_FrameRight;


    [SerializeField] Image m_Point;

    public void Init(int order, int total_order)
    {
        m_FrameLeft.SetActive(false);
        m_FrameCenter.SetActive(false);
        m_FrameRight.SetActive(false);

        if (order == 1) m_FrameLeft.SetActive(true);
        else if (order == total_order) m_FrameRight.SetActive(true);
        else m_FrameCenter.SetActive(true);

        ShowPoint(true);
        Green();
    }

    public void ShowPoint(bool flag)
    {
        m_Point.gameObject.SetActive(flag);
    }

    public void Green()
    {
        m_Point.color = new Color(7/255f, 255/255f, 7/255f, 1);
    }

    public void Yellow()
    {
        m_Point.color = new Color(1, 1, 1, 1);
    }

    public void Red()
    {
        m_Point.color = new Color(148/255f, 72/255f, 103/255f, 1);
    }
}
