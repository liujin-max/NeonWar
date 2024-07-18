using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private bool m_IsBulletTime = false;


    // Update is called once per frame
    void Update()
    {
        //子弹时间
        if (m_IsBulletTime) {
            if (Time.timeScale < 1) {
                Time.timeScale += Time.deltaTime * 5f;

                if (Time.timeScale > 1) 
                {
                    Time.timeScale = 1;
                    m_IsBulletTime = false;
                }
            }
        }
    }

    //子弹时间
    public void BulletTime()
    {
        m_IsBulletTime = true;
        //子弹时间
        Time.timeScale = 0.02f;
    }
}
