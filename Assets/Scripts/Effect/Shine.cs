using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shine : MonoBehaviour
{
    public float m_Interval = 2;
    public bool m_IsLoop = true;

    private Image m_Image;
    private CDTimer m_ShineTimer = new CDTimer(2);
    private float m_ShineDuration = 0;

    private int m_LoopCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_Image = transform.GetComponent<Image>();

        m_ShineTimer = new CDTimer(m_Interval);
        m_ShineTimer.Full();
    }

    public void Restart()
    {
        m_LoopCount = 0;
        m_ShineTimer.Full();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsLoop == false && m_LoopCount > 0) return;

        m_ShineTimer.Update(Time.deltaTime);
        if (m_ShineTimer.IsFinished() == true)
        {
            m_ShineDuration += Time.deltaTime;
            m_Image.material.SetFloat("_ShineLocation", Mathf.Lerp(0, 1, m_ShineDuration));

            if (m_ShineDuration >= 1) {
                m_ShineTimer.Reset();
                m_ShineDuration = 0;
                m_LoopCount++;
            }
        }
    }
}
