using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Fly : MonoBehaviour
{
    private Vector2 m_StartPositon;
    private Vector2 m_EndPosition;

    public float m_Time = 1f;



    public void GO(Vector2 start_pos, Vector2 end_pos, float time = -1)
    {
        if (time != -1) m_Time = time;
        
        transform.localPosition = start_pos;

        transform.DOLocalMove(end_pos, m_Time);
    }
}
