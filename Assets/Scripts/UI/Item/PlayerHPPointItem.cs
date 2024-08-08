using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPPointItem : MonoBehaviour
{
    [SerializeField] Image m_Frame;
    [SerializeField] Image m_Point;



    public void Init(int order, int total_order)
    {
        if (order == 1) m_Frame.sprite = Resources.Load<Sprite>("UI/Game/Game_PlayerHP_Left");
        else if (order == total_order) m_Frame.sprite = Resources.Load<Sprite>("UI/Game/Game_PlayerHP_Right");
        else m_Frame.sprite = Resources.Load<Sprite>("UI/Game/Game_PlayerHP");

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
