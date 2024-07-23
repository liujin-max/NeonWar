
using System;
using System.Collections.Generic;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "fingerblock-3gqj06sx5ebc6035";    //云开发环境ID

    public static float DEFAULT_RADIUS = 5.4f;  //圆弧半径
    public static float DEFAULT_BLINKCD= 1.5f;  //闪现冷却


    public static int UPGRADE_ATK       = 1;        //每级提高的攻击力
    public static float UPGRADE_ASP     = 0.05f;    //每级提高的攻速百分比






    public static int DEFAULT_RANK = 999;       //默认排名



    public static string COLOR_RED = "<#FF0000>";

 
    //状态机的状态列表
    public enum FSMSTATE
    {
        IDLE,       //待机阶段(可以进行局外操作)
        PLAY,       //游戏阶段
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

    //攻击3级解锁攻击槽1
    //攻击15级解锁攻击槽2
    //攻速8级解锁攻速槽1
    //攻速25级解锁攻速槽2
    public enum SKILLTYPE
    {
        ATK_1   = 1,
        ATK_2   = 2,

        ASP_1   = 3,
        ASP_2   = 4,
    }

    public static Dictionary<_C.SKILLTYPE, int> SKILLUNLOCKLEVELS = new Dictionary<_C.SKILLTYPE, int>()
    {
        [_C.SKILLTYPE.ATK_1] = 3,
        [_C.SKILLTYPE.ATK_1] = 15,

        [_C.SKILLTYPE.ASP_1] = 8,
        [_C.SKILLTYPE.ASP_2] = 25
    };



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

    //任务状态
    public enum TASK_STATE
    {
        NONE,       
        FINISH,
        RECEIVED
    }

    public enum RANK
    {
        STAGE
    }


    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
