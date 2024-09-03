using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPPointItem : MonoBehaviour
{
    [SerializeField] GameObject m_Frame_Center;
    [SerializeField] GameObject m_Frame_Left;
    [SerializeField] GameObject m_Frame_Right;

    [SerializeField] Image m_Point;



    public void Init(int order, int total_order)
    {
        m_Frame_Center.SetActive(order > 1 && order < total_order);
        m_Frame_Left.SetActive(order == 1);
        m_Frame_Right.SetActive(order == total_order);
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
        m_Point.color = new Color(255/255f, 199/255f, 0/255f, 1);
    }

    public void Red()
    {
        m_Point.color = new Color(236/255f, 15/255f, 60/255f, 1);
    }
}
