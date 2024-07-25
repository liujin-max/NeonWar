using System;
using System.Collections;
using System.Collections.Generic;
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
        Caster.ATT.VUN_INC.Pop(this);
    }
}
#endregion


public class Buff
{
    public int ID;
    public int Value = 0;   //参数


    //持续时间
    public CDTimer Duration = new CDTimer(999999);
    
    public Unit Caster;


    private static Dictionary<int, Func<Buff>> m_classDictionary = new Dictionary<int, Func<Buff>> {
        {(int)_C.BUFF.STUN,     () => new Buff_Stun()},
        {(int)_C.BUFF.YISHANG,  () => new Buff_YiShang()}
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

    public virtual void Dispose() {}
}
