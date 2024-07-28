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
        int e1  = 6; 
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
        float pr    = 0.5f;

        int hp_now  = Mathf.FloorToInt(hp_base * Mathf.Pow(1 + pr, stage_level - 1));

        return hp_now;
    }


    //掉落碎片和敌人血量的公式
    public static int FML_HP2Glass(int hp)
    {
        // k 是比例系数(0.5， 表示每2滴血掉落1颗碎片)
        float k = 1f;

        return Mathf.FloorToInt(k * hp);
    }


    //攻击力成长公式
    //属性 = 初始属性 + (等级 - 1) * 成长系数
    public static int FML_ATK(int level)
    {
        //初始属性
        int base_atk = 1;
        //成长系数
        int gc = Mathf.FloorToInt((level - 1) / 3) + 1;

        return base_atk + (level - 1) * gc;
    }

    //攻速成长公式
    //=$B$4/(1+((A7-1)*D7*$B$5)/100)
    public static float FML_ASP(float base_asp, int level)
    {
        //倍率变化
        float asp_rate = 1f;
        //成长系数
        int gc = Mathf.FloorToInt((level - 1) / 3) + 1;

        return base_asp / (1 + ((level - 1) * gc * asp_rate) / 100.0f);
    }


    //升级攻击力消耗的碎片数量和等级的公式
    public static int FML_ATKCost(int atk_level)
    {
        //第一次升级消耗的数量
        int cost_base = 3;

        //增长指数
        float cost_pa = 2f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(atk_level, cost_pa));
    }

    //升级攻速消耗的碎片数量和等级的公式
    public static int FML_ASPCost(int asp_level)
    {
        //第一次升级消耗的数量
        int cost_base = 2;

        //增长指数
        float cost_pa = 1.6f;

        return Mathf.FloorToInt(cost_base * Mathf.Pow(asp_level, cost_pa));
    }

}
