using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//数值公式系统
public static class NumericalManager
{
    //敌人数量和关卡的公式
    public static int FML_EnemyCount(int stage_level)
    {
        //第一关敌人的数量
        int e1  = 10; 
        //线性增长系数
        float m = 2f;
        //后期增长系数
        float a = 0.01f;
        //增长指数
        float b = 2f;

        return Mathf.FloorToInt(e1 + m * (stage_level - 1) + a * Mathf.Pow(stage_level - 1, b));
    }

    
    //敌人血量和关卡的关系公式
    public static int FML_EnemyHP(int stage_level)
    {
        //第一关敌人的基础血量
        int hp_base = 2;  
        //血量增长率，设定为0.1（每关增加10%） 
        float pr    = 0.3f;

        int hp_now  = Mathf.FloorToInt(hp_base * Mathf.Pow(1 + pr, stage_level - 1));

        //上下浮动
        int hp_min  = Mathf.FloorToInt(hp_now * 0.7f);
        int hp_max  = Mathf.CeilToInt(hp_now * 1.3f);

        return RandomUtility.Random(hp_min, hp_max);
    }


    //掉落碎片和敌人血量的公式
    public static int FML_HP2Glass(int hp)
    {
        // k 是比例系数(0.5， 表示每2滴血掉落1颗碎片)
        float k = 1f;

        return Mathf.FloorToInt(k * hp);
    }


    //升级攻击力消耗的碎片数量和等级的公式
    public static int FML_ATKCost(int atk_level)
    {
        //第一次升级消耗的数量
        int cost_base = 2;

        //增长指数
        float cost_pa = 2.6f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(atk_level, cost_pa));
    }

    //升级攻速消耗的碎片数量和等级的公式
    public static int FML_ASPCost(int asp_level)
    {
        //第一次升级消耗的数量
        int cost_base = 2;

        //增长指数
        float cost_pa = 1.8f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(asp_level, cost_pa));
    }

}
