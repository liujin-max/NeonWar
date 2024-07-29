using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


#region 晕眩
//晕眩0.5秒，无法移动
public class Buff_Stun : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(0.5f);

        Caster.Stop();
    }

    public override void Dispose()
    {
        base.Dispose();

        Caster.Resume();
    }
}
#endregion


#region 易伤
//5秒内受到的伤害提高#%
public class Buff_YiShang : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.VUN_INC.PutADD(this, Value / 100.0f);
    }

    public override void Dispose()
    {
        base.Dispose();

        Caster.ATT.VUN_INC.Pop(this);
    }
}
#endregion


#region 护盾
public class Buff_Shield : Buff
{
    private Tweener m_Tweener;

    public Buff_Shield()
    {
        EventManager.AddHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    public override void Init()
    {
        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.SHIELD, Vector3.zero, Caster.gameObject);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        EventManager.DelHandler(EVENT.ONBULLETHIT,  OnBulletHit);

        GameFacade.Instance.EffectManager.Load(EFFECT.SHIELD_BROKEN, Vector3.zero, Caster.gameObject);
    }

    private void OnBulletHit(GameEvent @event)
    {
        Value--;

        if (m_Tweener != null) {
            m_Tweener.Kill();
            m_Tweener = null;
        }
        
        // 创建抖动和缩放效果
        m_Tweener = m_Effect.transform.DOShakeRotation(0.2f, 25f, vibrato: 15, randomness: 50);

        if (Value <= 0) Caster.RemoveBuff(this);
    }
}
#endregion








public class Buff
{
    public Unit Caster;

    public int ID;
    public int Value = 0;   //参数
    public CDTimer Duration = new CDTimer(999999);  //持续时间
    

    protected Effect m_Effect = null;


    private static Dictionary<int, Func<Buff>> m_classDictionary = new Dictionary<int, Func<Buff>> {
        {(int)_C.BUFF.STUN,     () => new Buff_Stun()},
        {(int)_C.BUFF.YISHANG,  () => new Buff_YiShang()},
        {(int)_C.BUFF.SHIELD,   () => new Buff_Shield()},
    };


    public static Buff Create(int buff_id, int value, Unit caster)
    {
        Buff buff;
        if (m_classDictionary.ContainsKey(buff_id)) buff = m_classDictionary[buff_id]();
        else buff = new Buff();

        buff.ID     = buff_id;
        buff.Caster = caster;
        buff.Value  = value;

        return buff;
    }


    public virtual void Init() {}

    public virtual void Update(float deltaTime) 
    {
        Duration.Update(deltaTime);
    }

    public virtual void Flush()
    {
        Duration.ForceReset();
    }

    public bool IsEnd()
    {
        return Duration.IsFinished();
    }

    public virtual void Dispose() 
    {
        if (m_Effect != null) GameFacade.Instance.EffectManager.RemoveEffect(m_Effect);
    }
}
