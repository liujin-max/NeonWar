using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//冲锋预瞄线
public class DetectLineCharge : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_Bottom;
    [SerializeField] SpriteRenderer m_Progress;


    private Vector3 m_ProgressScale;

    public void Focus(Vector2 start_pos, Vector2 end_pos)
    {
        float angle = ToolUtility.VectorToAngle(end_pos - start_pos);
        float dis = Vector2.Distance(start_pos, end_pos);

        m_Bottom.transform.localEulerAngles = new Vector3(0, 0, angle);
        m_Bottom.transform.localScale = new Vector3(dis, 1, 1);

        m_ProgressScale = new Vector3(0, 1, 1);
        m_Progress.transform.localScale = m_ProgressScale;
    }

    public void UpdateProgress(float progress)
    {
        m_ProgressScale.Set(progress, 1, 1);
        m_Progress.transform.localScale = m_ProgressScale;
    }
}
