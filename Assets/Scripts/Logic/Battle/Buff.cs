using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


#region 晕眩
//晕眩0.5秒，无法移动
public class Buff_Stun : Buff
{
    public Buff_Stun()
    {
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(0.8f);
    }

    public override void Init()
    {
        Caster.StunFlag++;
        Caster.Stop();
    }

    public override void Dispose()
    {
        base.Dispose();

        Caster.StunFlag--;
        Caster.Resume();
    }
}
#endregion


#region 易伤
//5秒内受到的伤害提高#%
public class Buff_YiShang : Buff
{
    public Buff_YiShang()
    {
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Caster.ATT.VUN_INC.PutADD(this, Value / 100.0f);
    }

    public override void Dispose()
    {
        base.Dispose();

        Caster.ATT.VUN_INC.Pop(this);
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
        m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.SHIELD, Vector3.zero, Caster.gameObject);
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
        if (target != Caster) return;

        Value--;
        
        // SoundManager.Instance.Load(SOUND.HIT_SHIELD);
        
        m_Effect.transform.GetComponent<Animation>().Play("Shield_Hit");

        if (Value <= 0) Caster.RemoveBuff(this);
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
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(5);
    }

    public override void Init()
    {
        var player = Caster.GetComponent<Player>();
        if (player == null) return;

        player.MoveDirection = -1;
    }

    public override void Dispose()
    {
        base.Dispose();

        var player = Caster.GetComponent<Player>();
        if (player == null) return;

        player.MoveDirection = 1;
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
        Caster.ATT.ATK.PutMUL(this, 1.5f);
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
    public Buff_ATKDOWN()
    {
        Name    = "攻击削弱";
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Caster.ATT.ATK.PutMUL(this, 0.5f);
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
    public Buff_ASPUP()
    {
        Name    = "攻速提高";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Caster.CPS.PutMUL(this, 1.5f);
    }

    public override void Dispose()
    {
        Caster.CPS.Pop(this);
    }
}
#endregion


#region 攻速降低
public class Buff_ASPDOWN : Buff
{
    public Buff_ASPDOWN()
    {
        Name    = "攻速降低";
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Caster.CPS.PutMUL(this, 0.5f);
    }

    public override void Dispose()
    {
        Caster.CPS.Pop(this);
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
        Caster.ATT.SPEED.PutMUL(this, 1.3f);
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
    public Buff_SPEEDDOWN()
    {
        Name    = "移速降低";
        TYPE    = _C.BUFF_TYPE.DE;
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
        Caster.ATT.SPEED.PutMUL(this, 0.7f);
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
    public Buff_CP()
    {
        Name    = "暴击提高";
        Duration= new CDTimer(5f);
    }

    public override void Init()
    {
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
    public Buff_DODGEUP()
    {
        Name    = "闪避增强";
        Duration= new CDTimer(8f);
    }

    public override void Init()
    {
        Caster.ATT.DODGE.PutADD(this, 800);
    }

    public override void Dispose()
    {
        Caster.ATT.DODGE.Pop(this);
    }
}
#endregion


#region 减速敌人
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

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        foreach (var e in Field.Instance.Spawn.Enemys)
        {
            e.ATT.SPEED.PutMUL(this, 0.4f);
            e.SyncSpeed();
        }
    }

    public override void Dispose()
    {
        foreach (var e in Field.Instance.Spawn.Enemys)
        {
            e.ATT.SPEED.Pop(this);
            e.SyncSpeed();
        }
    }
}
#endregion

#endregion



public class Buff
{
    public Unit Caster;

    public int ID;
    public int Value = 0;   //参数
    public _C.BUFF_TYPE TYPE = _C.BUFF_TYPE.GAIN;
    public string Name = "未知";
    public CDTimer Duration = new CDTimer(999999);  //持续时间
    

    protected Effect m_Effect = null;


    private static Dictionary<int, Func<Buff>> m_classDictionary = new Dictionary<int, Func<Buff>> {
        {(int)_C.BUFF.STUN,     () => new Buff_Stun()},
        {(int)_C.BUFF.YISHANG,  () => new Buff_YiShang()},
        {(int)_C.BUFF.SHIELD,   () => new Buff_Shield()},
        {(int)_C.BUFF.CHAOS,    () => new Buff_Chaos()},


        //场上Buff
        {(int)_C.BUFF.ATK_UP,   () => new Buff_ATKUP()},
        {(int)_C.BUFF.ATK_DOWN, () => new Buff_ATKDOWN()},
        {(int)_C.BUFF.ASP_UP,   () => new Buff_ASPUP()},
        {(int)_C.BUFF.ASP_DOWN, () => new Buff_ASPDOWN()},
        {(int)_C.BUFF.SPEED_UP, () => new Buff_SPEEDUP()},
        {(int)_C.BUFF.SPEED_DOWN,   () => new Buff_SPEEDDOWN()},
        {(int)_C.BUFF.CP,       () => new Buff_CP()},
        {(int)_C.BUFF.DODGE_UP, () => new Buff_DODGEUP()},
        {(int)_C.BUFF.SPD_MUL,  () => new Buff_SPDMUL()},
    };


    public static Buff Create(int buff_id, int value, Unit caster, float time = 0)
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
        buff.Value  = value;

        if (time > 0) buff.Duration = new CDTimer(time);  //持续时间

        return buff;
    }


    public virtual void Init() {}

    public virtual void Update(float deltaTime) 
    {
        Duration.Update(deltaTime);
    }

    public virtual void Flush(float time)
    {
        if (time != 0) Duration.Reset(time);
        else Duration.ForceReset();
    }

    public bool IsEnd()
    {
        return Duration.IsFinished();
    }

    public virtual void Dispose() 
    {
        if (m_Effect != null) {
            GameFacade.Instance.EffectManager.RemoveEffect(m_Effect);
            m_Effect = null;
        }
    }
}
