
using System;
using System.Collections.Generic;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "fingerblock-3gqj06sx5ebc6035";    //云开发环境ID

    public static float DEFAULT_RADIUS = 5.4f;  //圆弧半径
    public static float DEFAULT_BLINKCD= 1.5f;  //闪现冷却



    
    public static int DEFAULT_RANK = 999;       //默认排名



    public static string COLOR_RED      = "<#FF0000>";
    public static string COLOR_GREEN    = "<#33FF07>";

 
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

    //怪物类型(普通、精英、Boss)
    public enum ENEMY_TYPE
    {
        NORMAL = 1,
        BOSS
    }

    public enum BUFF
    {
        STUN    = 50000,
        YISHANG,
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
