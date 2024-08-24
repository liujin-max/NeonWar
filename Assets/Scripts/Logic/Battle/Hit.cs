using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//一次受击的数据结构
public class Hit
{
    public Unit Caster;
    public AttributeValue ATK   = new AttributeValue(0);
    public AttributeValue CP    = new AttributeValue(0);
    public AttributeValue CT    = new AttributeValue(0);

    //伤害倍率
    public AttributeValue ATK_INC   = new AttributeValue(1, false);
    //对Boss额外伤害倍率
    public AttributeValue BOSS_INC  = new AttributeValue(1, false);



    [HideInInspector] public HashSet<Unit> IgnoreUnits = new HashSet<Unit>();
    [HideInInspector] public int KillRate = 0;          //必杀概率


    //物理特性
    public Vector3 Position;    //受击位置
    public Vector3 Velocity;    //击打的方向
    public Color HitColor = Color.white;      //受击颜色

    public Hit(Unit caster)
    {
        Caster = caster;
        
        ATK.SetBase(caster.ATT.ATK.ToNumber());
        CP.SetBase(caster.ATT.CP.ToNumber());
        CT.SetBase(caster.ATT.CT.ToNumber());

        BOSS_INC.SetBase(caster.ATT.BOSS_INC.ToNumber());
    }
}
