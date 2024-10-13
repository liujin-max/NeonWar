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
            UICtrl_GameWindow.Instance.Enter();
            UICtrl_NavWindow.Instance.Enter();

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
            new Event_PopupTip(){Text = "未完待续..."}.Notify();
            return false;
        }

        
        Field.Instance.Play(level_id);

        return true;
    }

    //打开技能树
    public static bool GotoSkillTree()
    {
        if (!DataCenter.Instance.IsSkillUnlock())
        {
            new Event_PopupTip(){Text = "通过关卡1后解锁"}.Notify();
            return false;
        }

        UICtrl_SkillTreeWindow.Instance.Enter();

        return true;
    }

    //打开背包
    public static bool GotoBackpack()
    {
        if (!DataCenter.Instance.IsPearUnlock())
        {
            new Event_PopupTip(){Text = "通过关卡" + CONST.PEAR_UNLOCK_LEVEL + "后解锁"}.Notify();
            return false;
        }

        UICtrl_BackpackWindow.Instance.Enter();

        return true;
    }
}


