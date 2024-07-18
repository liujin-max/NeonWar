


public class Pair 
{
    private int m_Curret;
    public int Current{ get {return m_Curret ;}}

    private int m_Total;
    public int Total{ get {return m_Total ;}}

    public Pair(int current, int total)
    {
        m_Curret    = current;
        m_Total     = total;
    }

    public void UpdateCurrent(int offset)
    {
        m_Curret += offset;
    }

    public void SetCurrent(int value)
    {
        m_Curret = value;
    }

    public bool IsFull()
    {
        return m_Curret >= m_Total;
    }

    public bool IsClear()
    {
        return m_Curret <= 0;
    }

    public void Clear()
    {
        m_Curret = 0;
    }

    public void Reset(int current, int total)
    {
        m_Curret = current;
        m_Total = total;
    }

}
