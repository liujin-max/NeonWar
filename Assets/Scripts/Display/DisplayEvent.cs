using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;



#region 空白 单纯的等待时间
public class DisplayEvent_Wait : DisplayEvent
{
    private CDTimer m_Timer = new CDTimer(1);
    public DisplayEvent_Wait(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        m_Timer = new CDTimer((float)m_Params[0]);
    }

    public override void Update(float dt)
    {
        m_Timer.Update(dt);
        if (m_Timer.IsFinished()) {
           m_State = _C.DISPLAY_STATE.END; 
        }
    }

}
#endregion
























//动画节点
public class DisplayEvent
{
    protected _C.DISPLAY_STATE m_State = _C.DISPLAY_STATE.IDLE;

    protected object[] m_Params;

    public DisplayEvent(params object[] values)
    {
        m_Params = values;
    }

    public virtual void Start()
    {
        m_State = _C.DISPLAY_STATE.PLAYING;
    }

    public virtual void Update(float dealta_time)
    {

    }

    public virtual void Terminate()
    {

    }

    public bool IsIdle()
    {
        return m_State == _C.DISPLAY_STATE.IDLE;
    }

    public bool IsPlaying()
    {
        return m_State == _C.DISPLAY_STATE.PLAYING;
    }

    public bool IsFinished()
    {
        return m_State == _C.DISPLAY_STATE.END;
    }
}
