using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//毒气陷阱
//陷阱范围内的目标持续受到伤害，持续#秒
public class Area_Poison : Area
{
    private CDTimer m_CountDown = new CDTimer(0.2f);
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

    //自定义的Update函数
    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime);

        m_CountDown.Update(deltaTime);
        if (!m_CountDown.IsFinished()) return;

        m_CountDown.Reset();
        //毒气伤害无法暴击
        m_Units.ForEach(u => {
            var hit = new Hit(Caster);
            hit.CP.SetBase(0);
            hit.ATK_INC.PutMUL(this, 0.1f);

            Field.Instance.SettleHit(hit, u);
        });
    }
}
