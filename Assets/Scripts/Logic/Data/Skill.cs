using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//弓箭：普通攻击
public class Skill_10010 : Skill
{
    public Skill_10010(SkillData data, Unit caster, int level) : base(data, caster, level){}

    public override void Init()
    {
        var player = Caster.GetComponent<Player_10000>();
        if (player == null) return;

        player.BulletCount = Value + 1;
    }
}

public class Skill
{
    public SkillData Data;
    public int Level;
    public Unit Caster;

    //参数
    public int Value {get {return Data.Values[Level - 1];}}

    public Skill(SkillData data, Unit caster, int level)
    {
        Data    = data;
        Caster  = caster;
        Level   = level;
    }

    public bool IsLevelMax()
    {
        return Level >= Data.LevelMax;
    }

    

    public virtual void Init()
    {

    }
}
