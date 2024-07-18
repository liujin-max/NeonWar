//计时器

public class CDTimer
{
    private float m_duration;
    public float Duration
    {
        get { 
            return m_duration; 
        }
    }

    private float m_current;
    public float Current
    {
        get { 
            return m_current; 
        }
    }

    //倒计时
    private bool m_IsCountDown = false;

    public CDTimer(float duration, bool is_countdown = false)
    {
        m_IsCountDown   = is_countdown;

        
        m_duration  = duration;
        m_current   = m_IsCountDown ? duration : 0;
    }

    public void Full()
    {
        m_current = m_duration;
    }

    public void Reset()
    {
        m_current -= m_duration;
    }

    public void ForceReset()
    {
        m_current = 0;
    }

    public void Reset(float duration)
    {
        m_current = 0;
        m_duration = duration;
    }

    public void SetDuration(float duration)
    {
        m_duration = duration;
    }

    public void SetCurrent(float value)
    {
        m_current = value;
    }

    public bool IsFinished()
    {
        if (m_IsCountDown) return m_current <= 0;

        return m_current >= m_duration;
    }

    public void Update(float deltatime) 
    {
        m_current += deltatime;
    }
}
