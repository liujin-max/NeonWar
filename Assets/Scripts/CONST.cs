
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "fingerblock-3gqj06sx5ebc6035";    //云开发环境ID


    public static string JSON_PATH = Application.dataPath + "/Resources/Json";

    public static float DEFAULT_GRID_WEIGHT = 1.25f;
    public static float DEFAULT_GRID_HEIGHT = 1.25f;

    
    public static int BOMB_UNLOCK_LEVEL     = 11;       //开放合成炸弹
    public static int ENDLESS_UNLOCK_LEVEL  = 10;

    public static int DEFAULT_FOOD      = 30;       //体力上限
    public static int FOOD_RECOVERYTIME = 300;      //体力恢复时间 300秒

    public static int DEFAULT_RANK  = 999;


    public static string COLOR_RED = "<#FF0000>";

 
    //状态机的状态列表
    public enum FSMSTATE
    {
        IDLE,       //待机
        ELIMINATE,  //消除阶段
        CHAIN,      //连锁反应阶段,
        CHECK,
        RESULT,      //

        TIP
    }

    //方向
    public enum DIRECTION
    {
        NONE = 99,

        UP = 0,
        DOWN,
        LEFT,
        RIGHT,
    }

    //方块类型
    public enum CARD_TYPE
    {
        JELLY   = 1,
        SPECIAL = 2,
        FRAME   = 3,

        DERIVE  = 4,    //玩法衍生物
    }

    //方块状态
    public enum CARD_STATE
    {
        NORMAL,
        GHOST    
    }

    //方块标记
    public enum CARD_TAG
    {
        NONE = 0,
        INFECTION,      //传染
        CHAIN,          //锁链
    }

    //方块ID
    public enum CARD
    {
        NONE = 0,

        UNIVERSAL   = 10000,
        RED,
        YELLOW,
        BLUE,
        GREEN,
        PURPLE,
        PINK,

        MISSILE     = 10010,
        BOMB,

        STONE       = 10020,
        WOOD,
        PORTAL,    //传送门


        POWER   = 10100,    //电力
        BAG,
        FOG,
    }

    //消除方式
    public enum DEAD_TYPE
    {
        NORMAL,     //变高然后消除
        DIGESTE,    //直接消除
        BOMB,       //爆炸
    }

    //游戏状态
    public enum GAME_STATE
    {
        NONE,
        PLAY,
        PAUSE,
        END
    }

    public enum RESULT
    {
        VICTORY,
        LOSE,
        NONE
    }

    //动画节点的状态
    public enum DISPLAY_STATE
    {
        IDLE,
        PLAYING,
        END
    }

    //关卡类型
    public enum MODE
    {
        CHAPTER = 0,
        ENDLESS
    }


    //任务ID
    public enum TASK
    {
        LOGIN   = 8000,
        SHARE,
        ENDLESS,
        USEPROP
    }

    //任务状态
    public enum TASK_STATE
    {
        NONE,       
        FINISH,
        RECEIVED

    }

    //排行榜
    public enum RANK
    {
        LEVEL,
        ENDLESS
    }


    //道具
    public enum PROP
    {
        TIME    = 101,
        STEP    = 102,
        SHUFFLE = 103,
        BOMB    = 104,
        KNOCK   = 105
    }



    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
