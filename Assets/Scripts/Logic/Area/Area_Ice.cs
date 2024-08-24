using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//冰冻陷阱
//陷阱范围内的目标行动缓慢，持续#秒
public class Area_Ice : Area
{
    [SerializeField] ParticleSystem m_ParticleSystem;

    void Start()
    {
        m_ParticleSystem.Stop();
        ResetParticleSystemDuration(m_ParticleSystem.transform, m_Timer.Duration - 0.7f);
        m_ParticleSystem.Play();
    }

    void ResetParticleSystemDuration(Transform parentTransform, float duration)
    {
        ParticleSystem particleSystem = parentTransform.GetComponent<ParticleSystem>();

        if (particleSystem != null) {
            var main = particleSystem.main;
            main.duration = duration;
        }

        foreach (Transform child in parentTransform) {
            ResetParticleSystemDuration(child, duration);
        }
    }


    //进入区域
    //进入区域后要做的逻辑写这里面
    public override void Enter(Unit unit)
    {
        unit.ATT.SPEED.PutMUL(this, 0.4f);
        unit.SyncSpeed();
    }

    //离开区域时
    //重写此方法，把离开区域后要做的逻辑写这里面
    public override void Exit(Unit unit)
    {
        unit.ATT.SPEED.Pop(this);
        unit.SyncSpeed();
    }
}
