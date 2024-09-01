using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    //前往主页
    public static void GotoLogin()
    {

    }

    //前往关卡模式
    public static void GotoGame()
    {
        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            Field.Instance.Enter();
        });
    }

    //打开背包
    public static void GotoBackpack(Pear pear)
    {
        if (!DataCenter.Instance.IsPearUnlock())
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "通过关卡" + _C.PEAR_UNLOCK_LEVEL + "后解锁"));
            return;
        }

        var obj = GameFacade.Instance.UIManager.GetWindow("BackpackWindow");
        if (obj == null) {
            GameFacade.Instance.UIManager.LoadWindowAsync("BackpackWindow", UIManager.MAJOR, (obj)=>{
                obj.GetComponent<BackpackWindow>().Init(pear);
            });
        }
        else
        {
            obj.GetComponent<BackpackWindow>().Init(pear);
        }
    }
}


