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
        
        SoundManager.Instance.Load(SOUND.HIT_SHIELD);
        
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

























#region 场上Buff


#region 攻击百分比提高
public class Buff_ATKUP : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.ATK.PutAUL(this, 0.5f);
    }

    public override void Dispose()
    {
        Caster.ATT.ATK.Pop(this);
    }
}
#endregion


#region 攻击百分比下降
public class Buff_ATKDOWN : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.ATK.PutAUL(this, -0.5f);
    }

    public override void Dispose()
    {
        Caster.ATT.ATK.Pop(this);
    }
}
#endregion


#region 攻速提高
public class Buff_ASPUP : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.ASP.PutMUL(this, 0.5f);
        Caster.ASP.SetDuration(Caster.ATT.ASP.ToNumber() / 1000.0f);
    }

    public override void Dispose()
    {
        Caster.ATT.ASP.Pop(this);
        Caster.ASP.SetDuration(Caster.ATT.ASP.ToNumber() / 1000.0f);
    }
}
#endregion


#region 攻速降低
public class Buff_ASPDOWN : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.ASP.PutMUL(this, 1.5f);
        Caster.ASP.SetDuration(Caster.ATT.ASP.ToNumber() / 1000.0f);
    }

    public override void Dispose()
    {
        Caster.ATT.ASP.Pop(this);
        Caster.ASP.SetDuration(Caster.ATT.ASP.ToNumber() / 1000.0f);
    }
}
#endregion


#region 移动速度提高
public class Buff_SPEEDUP : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.SPEED.PutAUL(this, 0.3f);
    }

    public override void Dispose()
    {
        Caster.ATT.SPEED.Pop(this);
    }
}
#endregion


#region 移动速度降低
public class Buff_SPEEDDOWN : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.SPEED.PutAUL(this, -0.3f);
    }

    public override void Dispose()
    {
        Caster.ATT.SPEED.Pop(this);
    }
}
#endregion


#region 暴击率提高
public class Buff_CP : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.CP.PutADD(this, 500);
    }

    public override void Dispose()
    {
        Caster.ATT.CP.Pop(this);
    }
}
#endregion


#region 闪避率提高
public class Buff_DODGEUP : Buff
{
    public override void Init()
    {
        Duration = new CDTimer(5f);

        Caster.ATT.DODGE.PutADD(this, 800);
    }

    public override void Dispose()
    {
        Caster.ATT.DODGE.Pop(this);
    }
}
#endregion


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


        //场上Buff
        {(int)_C.BUFF.ATK_UP,   () => new Buff_ATKUP()},
        {(int)_C.BUFF.ATK_DOWN, () => new Buff_ATKDOWN()},
        {(int)_C.BUFF.ASP_UP,   () => new Buff_ASPUP()},
        {(int)_C.BUFF.ASP_DOWN, () => new Buff_ASPDOWN()},
        {(int)_C.BUFF.SPEED_UP, () => new Buff_SPEEDUP()},
        {(int)_C.BUFF.SPEED_DOWN,   () => new Buff_SPEEDDOWN()},
        {(int)_C.BUFF.CP,       () => new Buff_CP()},
        {(int)_C.BUFF.DODGE_UP, () => new Buff_DODGEUP()},
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
