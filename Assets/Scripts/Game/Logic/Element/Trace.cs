using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region 抛物线
public class Trace_Parabola : Trace
{
    Vector3 m_BigScale = new Vector3(2f, 2f, 2f);


    public override void GO(Vector2 to_pos, float time)
    {
        base.GO(to_pos, time);

        m_Timer.Reset(time);
    }

    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        transform.localPosition = Vector3.Lerp(m_OPos, m_ToPos, m_Timer.Progress);

        //先放大再缩小
        float t1    = m_Timer.Current;
        float time  = m_Timer.Duration / 2f;

        if (t1 <= time) transform.localScale = Vector3.Lerp(Vector3.one, m_BigScale, t1 / time);
        else transform.localScale = Vector3.Lerp(m_BigScale, Vector3.one, (m_Timer.Current - time) / time);
    }   
}
#endregion

//各种运动轨迹的基类
public class Trace
{
    protected Transform transform;    //物体
    protected Vector2 m_OPos;       //起始位置

    protected Vector2 m_ToPos;      //目标位置
    protected CDTimer m_Timer;


    private static Dictionary<CONST.TRACE, Func<Trace>> m_classDictionary = new Dictionary<CONST.TRACE, Func<Trace>> {
        {CONST.TRACE.PARABOLA,     () => new Trace_Parabola()},
    };

    public static Trace Create(CONST.TRACE trace_type, Transform transform)
    {
        Trace trace;
        if (m_classDictionary.ContainsKey(trace_type)) {
            trace = m_classDictionary[trace_type]();
        }
        else {
            trace = new Trace();
            Debug.LogError("未实现的Trace ：" + trace_type);
        }

        trace.transform = transform;

        return trace;
    }

    public virtual void GO(Vector2 to_pos, float time)
    {
        m_OPos  = transform.localPosition;
        m_ToPos = to_pos;
        m_Timer = new CDTimer(time);
    }

    public virtual bool IsReach()
    {
        return m_Timer.IsFinished();
    }

    public virtual void CustomUpdate(float deltaTime)
    {

    }
}
