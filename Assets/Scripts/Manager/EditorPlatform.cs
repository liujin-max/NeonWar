using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPlatform : Platform
{
    public override void INIT(Action callback)
    {
        Application.targetFrameRate = CONST.DEFAULT_FRAME;

        if (callback != null) {
            callback.Invoke();
        }
    }

    public override void AUTH(Action callback)
    {

    }

    //不做别的处理了
    //编辑器模式下 存档数据在Login的时候直接都从本地获取到了
    public override void SYNC()
    {
        string json = PlayerPrefs.GetString(SystemManager.KEY_USER);

        if (string.IsNullOrEmpty(json)) {
            DataCenter.Instance.User.SyncRecords();
            DataCenter.Instance.User.SyncFinish();
            return;
        }
        
        Debug.Log("加载：" + json);

        DataCenter.Instance.User.Data = JsonUtility.FromJson<GameUserData>(json);

        DataCenter.Instance.User.SyncRecords();
        DataCenter.Instance.User.SyncFinish();
    }

    public override void UPLOAD(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(SystemManager.KEY_USER, json);
        PlayerPrefs.Save();

        Debug.Log("存储：" + json);
    }

    public override void SHARE(string text, bool show_image)
    {
        //任务：每日分享
        // DataCenter.Instance.Daily.FinishTask((int)CONST.TASK.SHARE);
    }

    //激励广告
    public override void REWARD_VIDEOAD(string ad_id, Action callback) 
    {
        if (callback != null) callback();
    }

    public override void BANNER_VIDEOAD(string ad_id, bool is_show, int top = -1) {}
    public override void INTER_VIDEOAD(string ad_id) {}
    public override void GRID_VIDEOAD(string ad_id, bool is_show , float rate = 1.0f) {}

    public override void UPLOADLEVELRANK(int level) {}

    public override void VIBRATE(string level)
    {
        if (!GameFacade.Instance.SystemManager.VibrateFlag) return;

    }

    //适配UI
    public override void ADAPTATION(RectTransform rectTransform, float offset)
    {

    }

    public override void UPDATETARGETFRAME(int frame)
    {
        Application.targetFrameRate = frame;
    }

    public override void OPENMINIGAME(string appid)
    {

    }

    //游戏圈
    public override void SHOWCLUBBUTTON(bool flag)
    {

    }
}
