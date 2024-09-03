using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//一次受击的数据结构
public class Hit
{
    public Unit Caster;
    public AttributeValue ATK;
    public AttributeValue CP;
    public AttributeValue CT;

    //伤害倍率
    public AttributeValue ATK_INC;
    //对Boss额外伤害倍率
    public AttributeValue BOSS_INC;
    
    //最终伤害
    public int FINAL;   


    [HideInInspector] public HashSet<Unit> IgnoreUnits = new HashSet<Unit>();
    [HideInInspector] public int KillRate;          //必杀概率


    //物理特性
    public Vector3 Position;    //受击位置
    public Vector3 Velocity;    //击打的方向
    public Color HitColor = Color.white;      //受击颜色

    public Hit(Unit caster)
    {
        Caster = caster;
        
        ATK = new AttributeValue(caster.ATT.ATK.ToNumber());
        CP  = new AttributeValue(caster.ATT.CP.ToNumber());
        CT  = new AttributeValue(caster.ATT.CT.ToNumber());

        ATK_INC     = new AttributeValue(1, false);
        BOSS_INC    = new AttributeValue(caster.ATT.BOSS_INC.ToNumber(), false);
    }
}
