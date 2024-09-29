using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//一次受击的数据结构
public class Hit
{
    public Unit Caster;
    public CONST.HIT_TYPE Type = CONST.HIT_TYPE.NORMAL;

    public AttributeValue ATK;
    public AttributeValue CP;
    public AttributeValue CT;

    //伤害倍率
    public AttributeValue ATK_INC;
    //箭矢伤害加成
    public AttributeValue ARROW_INC   = new AttributeValue(1f, false);
    
    //对Boss额外伤害倍率
    public float BOSS_INC;
    //对健康敌人额外伤害倍率
    public float HEALTH_INC;
    //对减速敌人额外伤害倍率
    public float SLOW_INC;
    //对受控制敌人额外伤害倍率
    public float CONTROL_INC;



    


    //最终伤害
    public int FINAL;   


    public HashSet<Unit> IgnoreUnits = new HashSet<Unit>();
    public int KillRate = 0;    //必杀概率
    public bool IsHitKill;      //触发必杀
    public bool IsCrit;



    //物理特性
    public Vector3 Position;    //受击位置
    public Vector3 Velocity;    //击打的方向
    public Color HitColor = Color.white;      //受击颜色

    public Hit(Unit caster)
    {
        Caster = caster;
        
        ATK = new AttributeValue(caster.ATT.ATK.ToNumber(false));
        CP  = new AttributeValue(caster.ATT.CP.ToNumber());
        CT  = new AttributeValue(caster.ATT.CT.ToNumber());

        ATK_INC     = new AttributeValue(caster.ATK_INC.ToNumber(), false);
        BOSS_INC    = caster.BOSS_INC.ToNumber();
        HEALTH_INC  = caster.HEALTH_INC.ToNumber();
        SLOW_INC    = caster.SLOW_INC.ToNumber();
        CONTROL_INC = caster.CONTROL_INC.ToNumber();

        EventManager.SendEvent(new GameEvent(EVENT.ONCREATEHIT, this));
    }
}
