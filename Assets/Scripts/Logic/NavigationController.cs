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


}


