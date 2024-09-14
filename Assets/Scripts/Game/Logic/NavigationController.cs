using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    //前往主界面
    public static void GotoGame()
    {
        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            GameFacade.Instance.UIManager.LoadWindowAsync("GameWindow", UIManager.BOTTOM, (obj)=>{
                obj.GetComponent<GameWindow>().Init();
            });

            GameFacade.Instance.UIManager.LoadWindowAsync("NavigationWindow", UIManager.NAV, (obj)=>{
                // obj.GetComponent<NavigationWindow>().Init();
            });



            Field.Instance.Enter();
        });
    }

    //前往关卡
    public static bool GotoBattle()
    {
        int level_id = DataCenter.Instance.User.Level + 1;

        if (GameFacade.Instance.TestMode == true)
        {
            level_id    = GameFacade.Instance.TestStage;
        }

        //判断是不是通关了
        if (DataCenter.Instance.Levels.LoadLevelJSON(level_id) == null) {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "未完待续..."));
            return false;
        }

        
        Field.Instance.Play(level_id);

        return true;
    }

    //打开技能树
    public static bool GotoSkillTree()
    {
        GameFacade.Instance.UIManager.LoadWindowAsync("SkillTreeWindow", UIManager.MAJOR, (obj)=>{
            obj.GetComponent<SkillTreeWindow>().Init();
        });

        return true;
    }

    //打开背包
    public static bool GotoBackpack(Pear pear)
    {
        if (!DataCenter.Instance.IsPearUnlock())
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "通过关卡" + _C.PEAR_UNLOCK_LEVEL + "后解锁"));
            return false;
        }

        GameFacade.Instance.UIManager.LoadWindowAsync("BackpackWindow", UIManager.BOARD, (obj)=>{
            obj.GetComponent<BackpackWindow>().Init(pear);
        });

        return true;
    }
}


