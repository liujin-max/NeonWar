
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CONST
{
    public static int DEFAULT_FRAME = 30;   //默认帧数





    public static int PEAR_UNLOCK_LEVEL= 0;    //通过15关后开放道具系统
    public static float DEFAULT_RADIUS = 5.4f;  //圆弧半径
    public static float DEFAULT_BLINKCD= 1.5f;  //闪现冷却

    public static int COMPOSE_COUNTMAX  = 3;    //合成数量上限


    //碰撞体标记
    public static string COLLIDER_BOARD = "Board";
    public static string COLLIDER_WALL  = "Wall";
    public static string COLLIDER_PLAYER= "Player";
    public static string COLLIDER_ENEMY = "Enemy";



    public static string COLOR_RED      = "<#FF0000>";
    public static string COLOR_ORANGE   = "<#FF9704>";
    public static string COLOR_GREEN    = "<#33FF07>";
    public static string COLOR_GREEN2   = "<#1D9F00>";
    public static string COLOR_BROWN    = "<#574040>";

    //品质对应的名字颜色
    public static Dictionary<int, string> LEVEL_COLOR_PAIRS = new Dictionary<int, string>()
    {
        [1] = "<#9C9C9C>",
        [2] = "<#33FF07>",
        [3] = "<#1790B2>",
        [4] = "<#9717B2>",
        [5] = "<#B21F17>",
    };

    //品质对应的属性词条数量
    public static Dictionary<int, int> LEVEL_PROPERTY_PAIRS = new Dictionary<int, int>()
    {
        [1] = 1,
        [2] = 2,
        [3] = 3,
        [4] = 4,
        [5] = 4,
    };

    //品质对应的属性点
    public static Dictionary<int, int> LEVEL_POINT_PAIRS = new Dictionary<int, int>()
    {
        [1] = 100,
        [2] = 250,
        [3] = 500,
        [4] = 1000,
        [5] = 1500,
    };





    public enum ATT
    {
        ATK,
        ASP,
        WORTH
    }

    public enum PROPERTY
    {
        NORMAL = 1,
        SPECIAL
    }
 
    //状态机的状态列表
    public enum FSMSTATE
    {
        IDLE,       //待机阶段(可以进行局外操作)
        PLAY,       //游戏阶段
        UPGRADE,    //升级阶段
        RESULT      //结算奖励阶段
    }

    //游戏状态
    public enum GAME_STATE
    {
        PLAY,
        PAUSE
    }

    //
    public enum SIDE
    {
        PLAYER,
        ENEMY
    }

    public enum PLAYER
    {
        BOW = 10000,    //弓
    }

    //怪物类型(普通、Boss)
    public enum ENEMY_TYPE
    {
        NORMAL = 1,
        BOSS
    }


    //伤害类型
    public enum HIT_TYPE
    {
        NORMAL,
        POISON
    }


    public enum BUFF
    {
        STUN    = 50000,
        YISHANG,
        SHIELD,
        CHAOS,
        FASTSPD,    //疾速
        KILL,
        FROZEN,     //冰冻
        POISON,     //中毒
        CRIT,       //会心
        CRITDEMAGE,  //爆伤



        ATK_UP      = 60000,    //攻击力提高
        ATK_DOWN    = 60001,    //攻击力降低
        ASP_UP      = 60002,    //攻速提升
        ASP_DOWN    = 60003,    //攻速降低
        SPEED_UP    = 60004,    //移动速度提高
        SPEED_DOWN  = 60005,    //移动速度降低
        CP          = 60006,    //暴击率提高
        DODGE_UP    = 60007,    //闪避率提高
        SPD_MUL     = 60008,    //减速全场敌人

        END
    }

    public enum BUFF_TYPE
    {
        GAIN,
        DE
    }

    public enum TRACE
    {
        PARABOLA,       //抛物线
    }

    public enum RESULT
    {
        VICTORY,
        LOSE,
        UPGRADE,
        NONE
    }


    //动画节点的状态
    public enum DISPLAY_STATE
    {
        IDLE,
        PLAYING,
        END
    }

    //任务状态
    public enum TASK_STATE
    {
        NONE,       
        FINISH,
        RECEIVED
    }




    
    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
