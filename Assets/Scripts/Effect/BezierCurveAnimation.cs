using UnityEngine;
using DG.Tweening;
using System;

public class BezierCurveAnimation : MonoBehaviour
{
    private Action m_Callback;

    private Vector2 m_StartPos;
    private Vector2 m_ControlPos;
    private Vector2 m_EndPos;

    public float duration = 0.8f; // 动画持续时间
    private float t = 0f; // 用于插值计算

    public void Fly(Vector2 start_pos, Vector2 end_pos, float time = -1, Action callback = null)
    {
        m_Callback  = callback;
        duration    = time == -1 ? duration : time;

        m_StartPos  = start_pos;
        m_EndPos    = end_pos;

        t   = 0;
        
        float distance  = Vector3.Distance(start_pos, end_pos);

        // m_ControlPos    = ToolUtility.FindPointOnCircle(m_StartPos, distance * 0.6f, RandomUtility.Random(-135, 45));
        m_ControlPos    = ToolUtility.FindPointOnCircle(m_StartPos, distance * 0.5f, RandomUtility.Random(-90, 90));
    }



    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime / duration;

            transform.position  = CalculateBezierPoint(t, m_StartPos, m_ControlPos, m_EndPos);
        } else {
            transform.position  = m_EndPos;

            if (m_Callback != null) m_Callback();
            // gameObject.SetActive(false);
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}
