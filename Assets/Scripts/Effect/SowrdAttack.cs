using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SowrdAttack : IconAnim
{
    private Animation m_Animation;


    void Awake()
    {
        m_Animation = transform.GetComponent<Animation>();
    }


    public override void Play()
    {
        m_Animation.Play();
    }

    public override void Stop()
    {
        m_Animation.clip.SampleAnimation(gameObject, 0);
        m_Animation.Stop();
    }
}
