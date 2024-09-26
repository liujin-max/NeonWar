using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


#region 晕眩
//晕眩0.5秒，无法移动
public class Buff_Stun : Buff, IDisposable
{
    public Buff_Stun()
    {
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(0.8f);
    }

    public override void Init()
    {
        Belong.StunReference++;
        Belong.Stop();

        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.STUN, Vector3.zero, Belong.HeadPivot.gameObject);
    }

    public override void Dispose()
    {
        base.Dispose();

        Belong.StunReference--;
        Belong.Resume();
    }
}
#endregion


#region 易伤
//3秒内受到的伤害提高#%
public class Buff_YiShang : Buff
{
    public Buff_YiShang()
    {
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(3f);
    }

    public override void Init()
    {
        Belong.ATT.VUN_INC.PutADD(this, Value / 100.0f);

        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.POJIA, Vector3.zero, Belong.HeadPivot.gameObject);
    }

    public override void Dispose()
    {
        base.Dispose();

        Belong.ATT.VUN_INC.Pop(this);
    }
}
#endregion


#region 蛛网护盾
public class Buff_Shield : Buff
{
    public Buff_Shield()
    {
        EventManager.AddHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    public override void Init()
    {
        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.SHIELD, Vector3.zero, Belong.gameObject);
        m_Effect.transform.GetComponent<Animation>().Play("Shield_Show");
    }

    public override void Dispose()
    {
        // SoundManager.Instance.Load(SOUND.HIT_SHIELD);

        m_Effect.transform.GetComponent<Animation>().Play("Shield_Broken");
        
        EventManager.DelHandler(EVENT.ONBULLETHIT,  OnBulletHit);
    }

    private void OnBulletHit(GameEvent @event)
    {
        var target = @event.GetParam(1) as Unit;
        if (target != Belong) return;

        Value--;
        
        // SoundManager.Instance.Load(SOUND.HIT_SHIELD);
        
        m_Effect.transform.GetComponent<Animation>().Play("Shield_Hit");

        if (Value <= 0) Belong.RemoveBuff(this);
    }
}
#endregion


#region 混乱
//针对玩家，使左右键的方向相反
public class Buff_Chaos : Buff
{
    public Buff_Chaos()
    {
        Name    = "混乱";
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(5);
    }

    public override void Init()
    {
        var player = Belong.GetComponent<Player>();
        if (player == null) return;

        player.MoveDirection = -1;

        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.CHAOS, Vector3.zero, Belong.HeadPivot.gameObject);
    }

    public override void Dispose()
    {
        base.Dispose();

        var player = Belong.GetComponent<Player>();
        if (player == null) return;

        player.MoveDirection = 1;
    }
}
#endregion

#region 疾速
public class Buff_FastSPD : Buff
{
    public Buff_FastSPD()
    {
        Name    = "疾速";
        Duration= new CDTimer(9999999);
    }

    public override void Init()
    {
        Belong.CPS.PutAUL(this, Value / 100.0f * Count);
    }

    public override void Dispose()
    {
        Belong.CPS.Pop(this);
    }

    public override void Flush(float time, int count)
    {
        base.Flush(time, count);

        Count += count;

        Belong.CPS.PutAUL(this, Value / 100.0f * Count);
    }
}
#endregion


#region 杀意
public class Buff_Kill : Buff
{
    public Buff_Kill()
    {
        Name    = "杀意";
        Duration= new CDTimer(9999999);
    }

    public override void Init()
    {
        Belong.ATT.ATK.PutAUL(this, Value / 100.0f * Count);
    }

    public override void Dispose()
    {
        Belong.ATT.ATK.Pop(this);
    }

    public override void Flush(float time, int count)
    {
        base.Flush(time, count);

        Count += count;

        Belong.ATT.ATK.PutAUL(this, Value / 100.0f * Count);
    }
}
#endregion

#region 冰冻
public class Buff_Frozen : Buff
{
    public Buff_Frozen()
    {
        Name    = "冰冻";
        TYPE    = CONST.BUFF_TYPE.DE;
    }

    public override void Init()
    {
        Belong.StunReference++;
        Belong.Stop();

        Belong.AffectedEffect.Frozen(true);
    }

    public override void Dispose()
    {
        base.Dispose();

        Belong.StunReference--;
        Belong.Resume();

        Belong.AffectedEffect.Frozen(false);
    }
}
#endregion


#region 中毒
public class Buff_Poison : Buff
{
    private CDTimer m_Timer = new CDTimer(1f);
    public Buff_Poison()
    {
        Name    = "中毒";
        TYPE    = CONST.BUFF_TYPE.DE;
    }

    public override void Init()
    {
        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.POISON, Vector3.zero, Belong.HeadPivot.gameObject);
    }

    public override void Flush(float time, int count)
    {
        base.Flush(time, count);

        Count += count;
    }

    public override void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;
        m_Timer.Reset();

        var hit = new Hit(Caster);
        hit.Type = CONST.HIT_TYPE.POISON;
        hit.HitColor = Color.green;
        hit.ATK.SetBase(Value);
        hit.ATK_INC.SetBase(1);
        hit.ATK_INC.PutMUL(this, Count);
        hit.CP.SetBase(0);

        Field.Instance.SettleHit(hit, Belong);

        Count--;
    }

    public override void Dispose()
    {
        base.Dispose();

    }
}
#endregion


#region 会心
public class Buff_Crit : Buff
{
    public Buff_Crit()
    {
        Name    = "会心";
        Duration= new CDTimer(9999999);
    }

    public override void Init()
    {
        Belong.ATT.CP.PutADD(this, Value * 10 * Count);
    }

    public override void Dispose()
    {
        Belong.ATT.CP.Pop(this);
    }

    public override void Flush(float time, int count)
    {
        base.Flush(time, count);

        Count += count;

        Belong.ATT.CP.PutADD(this, Value * 10 * Count);
    }
}
#endregion


















#region 可拾取BUFF








#region 攻击百分比提高
public class Buff_ATKUP : Buff
{
    public Buff_ATKUP()
    {
        Name    = "攻击增强";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.ATT.ATK.PutMUL(this, 1.5f);
    }

    public override void Dispose()
    {
        Belong.ATT.ATK.Pop(this);
    }
}
#endregion


#region 攻击百分比下降
public class Buff_ATKDOWN : Buff
{
    public Buff_ATKDOWN()
    {
        Name    = "攻击削弱";
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.ATT.ATK.PutMUL(this, 0.5f);
    }

    public override void Dispose()
    {
        Belong.ATT.ATK.Pop(this);
    }
}
#endregion


#region 攻速提高
public class Buff_ASPUP : Buff
{
    public Buff_ASPUP()
    {
        Name    = "攻速提高";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.CPS.PutMUL(this, 1.5f);
    }

    public override void Dispose()
    {
        Belong.CPS.Pop(this);
    }
}
#endregion


#region 攻速降低
public class Buff_ASPDOWN : Buff
{
    public Buff_ASPDOWN()
    {
        Name    = "攻速降低";
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.CPS.PutMUL(this, 0.5f);
    }

    public override void Dispose()
    {
        Belong.CPS.Pop(this);
    }
}
#endregion


#region 移动速度提高
public class Buff_SPEEDUP : Buff
{
    public Buff_SPEEDUP()
    {
        Name    = "移速提高";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.ATT.SPEED.PutMUL(this, 1.3f);
    }

    public override void Dispose()
    {
        Belong.ATT.SPEED.Pop(this);
    }
}
#endregion


#region 移动速度降低
public class Buff_SPEEDDOWN : Buff
{
    public Buff_SPEEDDOWN()
    {
        Name    = "移速降低";
        TYPE    = CONST.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.ATT.SPEED.PutMUL(this, 0.7f);
    }

    public override void Dispose()
    {
        Belong.ATT.SPEED.Pop(this);
    }
}
#endregion


#region 暴击率提高
public class Buff_CP : Buff
{
    public Buff_CP()
    {
        Name    = "暴击提高";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Belong.ATT.CP.PutADD(this, 500);
    }

    public override void Dispose()
    {
        Belong.ATT.CP.Pop(this);
    }
}
#endregion


#region 闪避率提高
public class Buff_DODGEUP : Buff
{
    public Buff_DODGEUP()
    {
        Name    = "闪避增强";
        Duration= new CDTimer(8f);
    }

    public override void Init()
    {
        Belong.ATT.DODGE.PutADD(this, 800);
    }

    public override void Dispose()
    {
        Belong.ATT.DODGE.Pop(this);
    }
}
#endregion


#region 子弹时间
public class Buff_SPDMUL : Buff
{
    public Buff_SPDMUL()
    {
        Name    = "子弹时间";
        Duration= new CDTimer(3.5f);
    }

    public override void Init()
    {

    }

    public override void CustomUpdate(float deltaTime)
    {
        base.CustomUpdate(deltaTime);

        foreach (var e in Field.Instance.Spawn.Enemys)
        {
            e.ATT.SPEED.PutMUL(this, 0.4f);
            e.SyncSpeed();
        }

        foreach (var b in Field.Instance.Bullets)
        {
            if (b.Caster == Field.Instance.Player) continue;
            b.Speed.PutMUL(this, 0.4f);
            b.SyncSpeed();
        }
    }

    public override void Dispose()
    {
        foreach (var e in Field.Instance.Spawn.Enemys)
        {
            e.ATT.SPEED.Pop(this);
            e.SyncSpeed();
        }

        foreach (var b in Field.Instance.Bullets)
        {
            if (b.Caster == Field.Instance.Player) continue;
            b.Speed.Pop(this);
            b.SyncSpeed();
        }
    }
}
#endregion

#endregion



public class Buff
{
    public Unit Caster; //Buff释放者
    public Unit Belong; //Buff拥有者

    public int ID;
    public int Value = 0;   //参数
    public int Count = 1;   //层数
    public CONST.BUFF_TYPE TYPE = CONST.BUFF_TYPE.GAIN;
    public string Name = "未知";
    public CDTimer Duration = new CDTimer(999999);  //持续时间
    

    protected Effect m_Effect = null;


    private static Dictionary<int, Func<Buff>> m_classDictionary = new Dictionary<int, Func<Buff>> {
        {(int)CONST.BUFF.STUN,      () => new Buff_Stun()},
        {(int)CONST.BUFF.YISHANG,   () => new Buff_YiShang()},
        {(int)CONST.BUFF.SHIELD,    () => new Buff_Shield()},
        {(int)CONST.BUFF.CHAOS,     () => new Buff_Chaos()},
        {(int)CONST.BUFF.FASTSPD,   () => new Buff_FastSPD()},
        {(int)CONST.BUFF.KILL,      () => new Buff_Kill()},
        {(int)CONST.BUFF.FROZEN,    () => new Buff_Frozen()},
        {(int)CONST.BUFF.POISON,    () => new Buff_Poison()},
        {(int)CONST.BUFF.CRIT,      () => new Buff_Crit()},

        //场上Buff
        {(int)CONST.BUFF.ATK_UP,   () => new Buff_ATKUP()},
        {(int)CONST.BUFF.ATK_DOWN, () => new Buff_ATKDOWN()},
        {(int)CONST.BUFF.ASP_UP,   () => new Buff_ASPUP()},
        {(int)CONST.BUFF.ASP_DOWN, () => new Buff_ASPDOWN()},
        {(int)CONST.BUFF.SPEED_UP, () => new Buff_SPEEDUP()},
        {(int)CONST.BUFF.SPEED_DOWN,   () => new Buff_SPEEDDOWN()},
        {(int)CONST.BUFF.CP,       () => new Buff_CP()},
        {(int)CONST.BUFF.DODGE_UP, () => new Buff_DODGEUP()},
        {(int)CONST.BUFF.SPD_MUL,  () => new Buff_SPDMUL()},
    };


    public static Buff Create(int buff_id, int value, Unit caster, Unit belong, float time = 0)
    {
        Buff buff;
        if (m_classDictionary.ContainsKey(buff_id)) {
            buff = m_classDictionary[buff_id]();
        }
        else {
            buff = new Buff();
            Debug.LogError("未实现的Buff：" + buff_id);
        }

        buff.ID     = buff_id;
        buff.Caster = caster;
        buff.Belong = belong;
        buff.Value  = value;

        if (time > 0) buff.Duration = new CDTimer(time);  //持续时间

        return buff;
    }


    public virtual void Init() {}

    public virtual void CustomUpdate(float deltaTime) 
    {
        Duration.Update(deltaTime);
    }

    public virtual void Flush(float time, int count)
    {
        if (time != 0) Duration.Reset(time);
        else Duration.ForceReset();
    }

    public bool IsEnd()
    {
        return Duration.IsFinished() || Count <= 0;
    }

    public virtual void Dispose() 
    {
        if (m_Effect != null) {
            GameFacade.Instance.EffectManager.RemoveEffect(m_Effect);
            m_Effect = null;
        }
    }
}
