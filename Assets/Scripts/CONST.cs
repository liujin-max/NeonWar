
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "fingerblock-3gqj06sx5ebc6035";    //云开发环境ID


    public static int DEFAULT_RANK = 999;



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
