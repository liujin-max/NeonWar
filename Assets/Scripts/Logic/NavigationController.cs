using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    //前往主页
    public static void GotoLogin()
    {
        // SoundManager.Instance.PlayBGM(SOUND.BGM);

        //加载loading界面
        // GameFacade.Instance.ScenePool.LoadSceneAsync("Login", () => {
        //     var window = GameFacade.Instance.UIManager.LoadWindow("LoginWindow", UIManager.BOTTOM).GetComponent<LoginWindow>();
        //     window.Init();
        // });
    }

    //前往关卡模式
    public static void GotoGame()
    {
        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            Field.Instance.Enter();
        });



        // GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
        //     // SoundManager.Instance.PlayBGM(SOUND.BGM);

        //     GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
        //         Field.Instance.Enter(level);
        //     });
        // });
    }

    //打开背包
    public static void GotoBackpack(Pear pear)
    {
        if (!GameFacade.Instance.DataCenter.IsPearUnlock())
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "通过关卡" + _C.PEAR_UNLOCK_LEVEL + "后解锁"));
            return;
        }

        var window = GameFacade.Instance.UIManager.LoadWindow("BackpackWindow", UIManager.MAJOR).GetComponent<BackpackWindow>();
        window.Init();

        if (pear != null) window.ShowDetail(pear);
    }
}


