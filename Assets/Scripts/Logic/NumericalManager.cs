using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//数值公式系统
public static class NumericalManager
{

    #region 攻击力成长系数
    public static int FML_ATK_GC(int level)
    {
        return Mathf.FloorToInt((level - 1) / 3) + 1;
    }
    #endregion

    #region 攻速成长系数
    public static int FML_ASP_GC(int level)
    {
        return Mathf.FloorToInt((level - 1) / 3) + 1;
    }
    #endregion


    #region 价值成长系数
    public static int FML_WORTH_GC(int level)
    {
        return Mathf.FloorToInt((level - 1) / 1) + 1;
    }
    #endregion




    #region 攻击力成长公式
    //属性 = 初始属性 + (等级 - 1) * 成长系数
    public static int FML_ATK(int level)
    {
        //初始属性
        int base_atk = 1;
        //成长系数
        int gc = FML_ATK_GC(level);

        return base_atk + (level - 1) * gc;
    }
    #endregion

    #region 攻速成长公式
    public static float FML_ASP(float base_asp, int level)
    {
        //每级攻速提高1%
        float asp_rate = 1f;
        //成长系数
        int gc = FML_ASP_GC(level);

        return base_asp / (1 + ((level - 1) * gc * asp_rate) / 100.0f);
    }
    #endregion

    #region 价值成长公式
    public static float FML_WORTH(int level)
    {
        //成长系数
        // int gc = FML_WORTH_GC(level);

        return Mathf.Max(0, level - 1);
    }
    #endregion








    #region 攻击消耗公式
    //消耗=初始消耗*等级^成长指数*成长系数
    public static int FML_ATKCost(int level)
    {
        //第一次升级消耗的数量
        int cost_base = 1;
        //成长指数
        float power = 2;
        //成长系数
        int gc = FML_ATK_GC(level);


        return Mathf.FloorToInt(cost_base * Mathf.Pow(level, power) * gc);
    }
    #endregion

    #region 攻速消耗公式
    //消耗=初始消耗*等级^成长指数*成长系数
    public static int FML_ASPCost(int level)
    {
        //第一次升级消耗的数量
        int cost_base = 2;
        //成长指数
        float power = 1.7f;
        //成长系数
        int gc = FML_ASP_GC(level);

        return Mathf.FloorToInt(cost_base * Mathf.Pow(level, power) * gc);
    }
    #endregion

    #region 价值消耗公式
    public static int FML_WORTHCost(int level)
    {
        //第一次升级消耗的数量
        int cost_base = 1;
        //成长指数
        float power = 1.8f;
        //成长系数
        int gc = FML_WORTH_GC(level);


        return Mathf.FloorToInt(cost_base * Mathf.Pow(level + 1, power) * gc);
    }
    #endregion
}
