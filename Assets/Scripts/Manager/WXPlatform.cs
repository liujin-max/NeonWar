using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

#if WEIXINMINIGAME  //&& !UNITY_EDITOR
using WeChatWASM;


public class WXPlatform : Platform
{
    private Dictionary<string, WXBannerAd> m_BannerADPairs = new Dictionary<string, WXBannerAd>();
    private Dictionary<string, WXCustomAd> m_CustomADPairs = new Dictionary<string, WXCustomAd>();
    private WXGameClubButton m_BtnClub;

    public override void INIT(Action callback)
    {
        GameObject.Find("EventSystem").AddComponent<WXTouchInputOverride>();

        WX.InitSDK((code) => {
            //设置帧率
            WX.SetPreferredFramesPerSecond(CONST.DEFAULT_FRAME);

            //初始化云开发
            // CallFunctionInitParam param = new CallFunctionInitParam();
            // param.env = CONST.CLOUD_ENV;
            // WX.cloud.Init(param);

            //上报启动
            WX.ReportGameStart();

            
            if (callback != null) {
                callback.Invoke();
            }
        });
    }

    public override void AUTH(Action callback)
    {
        GetSettingOption info = new GetSettingOption();
        info.complete = (aa) => { /*获取完成*/ };
        info.fail = (aa) => { Debug.Log("获取失败"); };
        info.success = (aa) =>
        {
            if (!aa.authSetting.ContainsKey("scope.userInfo") || !aa.authSetting["scope.userInfo"])
            {
                //《三、调起授权》
                //调用请求获取用户信息
                WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", true);
                btn.OnTap((res) => {
                    Debug.Log("点击授权按钮");
                    if (res.errCode == 0) {
                        //用户已允许获取个人信息，返回的data即为用户信息
                        DataCenter.Instance.User.Data.Name       = res.userInfo.nickName;
                        DataCenter.Instance.User.Data.HeadUrl    = res.userInfo.avatarUrl;

                        DataCenter.Instance.User.Upload();

                    } else {
                        Debug.Log("用户未允许获取个人信息");
                    }
                    btn.Hide();
                });
            }
            else
            {
                //《四、获取用户信息》
                GetUserInfoOption userInfo = new GetUserInfoOption()
                {
                    withCredentials = true,
                    lang = "zh_CN",
                    success = (data) => {
                        // //用户已允许获取个人信息，返回的data即为用户信息
                        DataCenter.Instance.User.Data.Name       = data.userInfo.nickName;
                        DataCenter.Instance.User.Data.HeadUrl    = data.userInfo.avatarUrl;
                    }
                };
                WX.GetUserInfo(userInfo);
            }
        };
        WX.GetSetting(info);
    }

    //启动同步账号数据
    public override void SYNC()
    {
        string json = PlayerPrefs.GetString(SystemManager.KEY_USER);

        if (string.IsNullOrEmpty(json)) {
            DataCenter.Instance.User.SyncRecords();
            DataCenter.Instance.User.SyncFinish();
            return;
        }
        
        // Debug.Log("加载存档：" + json);

        DataCenter.Instance.User.Data = JsonUtility.FromJson<GameUserData>(json);

        DataCenter.Instance.User.SyncRecords();
        DataCenter.Instance.User.SyncFinish();

        // Debug.Log("====开始获取账号数据====");
        // EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));
        // //云开发：加载积分数据
        // WX.cloud.CallFunction(new CallFunctionParam()
        // {
        //     name = "GetUserData",
        //     data = JsonUtility.ToJson(""),
        //     success = (res) =>
        //     {
        //         Debug.Log("====获取账号数据成功==== : " + res.result);
        //         //云数据保存到本地
        //         var data = JsonMapper.ToObject(res.result);
        //         if (data.ContainsKey("data"))
        //         {
        //             //将Json转换成临时的GameUserData
        //             DataCenter.Instance.User.Data    = JsonUtility.FromJson<GameUserData>(JsonMapper.ToJson(data["data"]));

        //             //基础数据
        //             DataCenter.Instance.User.Base    = JsonUtility.FromJson<BaseData>(JsonMapper.ToJson(data["data"]["userInfo"]));
        //         }
        //     },
        //     fail = (res) =>
        //     {
        //         Debug.LogError("====获取账号数据失败====" + res.errMsg);
        //     },
        //     complete = (res) =>
        //     {
        //         Debug.Log("====获取账号数据结束====");
        //         DataCenter.Instance.User.SyncRecords();


        //         DataCenter.Instance.User.SyncFinish();
                
        //         EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));
        //     }
        // });
        

    }

    public override void UPLOAD(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(SystemManager.KEY_USER, json);
        PlayerPrefs.Save();

        //上传账号数据
        // Debug.Log("====开始上传账号数据====");
        // WX.cloud.CallFunction(new CallFunctionParam()
        // {
        //     name = "SetUserData",
        //     data = JsonUtility.ToJson(userData),
        //     success = (res) =>
        //     {
        //         Debug.Log("====上传账号数据成功====");
        //     },
        //     fail = (res) =>
        //     {
        //         Debug.LogError("====上传账号数据失败====" + res.errMsg);
        //     }
        // });
    }

    //分享
    public override void SHARE(string text, bool show_image)
    {
        //任务：每日分享
        // DataCenter.Instance.Daily.FinishTask((int)CONST.TASK.SHARE);

        if (show_image == true)
        {
            WX.ShareAppMessage(new ShareAppMessageOption()
            {
                title       = text, //"差一点就破纪录啦！",
                imageUrlId  = "6OuzIXxIR66YZcDxPH3xYA==",
                imageUrl    = "https://mmocgame.qpic.cn/wechatgame/0C1XOHqFMnwDicJRibia0zOBDv0Wowc8M40UAnXjax7jTdEdDsGTyswAfeR7MsiaHvJic/0",
            });
        }
        else
        {
            WX.ShareAppMessage(new ShareAppMessageOption()
            {
                title       = text,
            });
        }
        
    }

    void LoadAD(WXRewardedVideoAd ad, Action callback)
    {
        ad.Load((WXTextResponse reponse)=>{
            callback();
        }, (WXADErrorResponse error_reponse)=>{
            LoadAD(ad, callback);
        });
    }

    //激励广告
    public override void REWARD_VIDEOAD(string ad_id, Action callback)
    {
        if (callback != null) callback();
        
        // WXCreateRewardedVideoAdParam param = new WXCreateRewardedVideoAdParam();
        // param.adUnitId = ad_id;

        // WXRewardedVideoAd ad = WX.CreateRewardedVideoAd(param);
        // ad.OnError((WXADErrorResponse response)=>{
        //     //上报事件:广告错误

        // });

        // this.LoadAD(ad, ()=>{
        //     ad.Show((WXTextResponse reponse)=>{
                
        //     }, (WXTextResponse error_reponse)=>{
        //         //上报事件:广告错误
        //         EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "暂无广告"));
        //     });
        // });

        // ad.OnClose((WXRewardedVideoAdOnCloseResponse reponse)=>{
        //     // Debug.Log("是否完成观看：" + reponse.isEnded);
        //     if (reponse != null && reponse.isEnded == true) {
        //         //上报事件:观看广告

        //         if (callback != null) {
        //             callback();
        //         }  
        //     }
        // });  
    }

    //Banner广告
    public override void BANNER_VIDEOAD(string ad_id, bool is_show, int top = -1)
    {
        WXBannerAd ad;
        if (m_BannerADPairs.TryGetValue(ad_id, out ad)) {
            
        } else {
            var info = WX.GetSystemInfoSync();

            WXCreateBannerAdParam param = new WXCreateBannerAdParam();
            param.adUnitId      = ad_id;
            param.adIntervals   = 30;
            param.style.left    = (int)info.screenWidth / 2 - 150; //60;
            param.style.top     = top == -1 ? (int)info.screenHeight - 120 : top;
            param.style.width   = 300;
            param.style.height  = 200;

            ad = WX.CreateBannerAd(param);
            m_BannerADPairs.Add(ad_id, ad);

            ad.OnError((WXADErrorResponse response)=>{
                //上报事件:广告错误

            });
        }

        if (is_show == true) {
            ad.Show((WXTextResponse reponse)=>{

            }, (WXTextResponse reponse)=>{
                //上报事件:广告错误

            });
        } else {
            ad.Hide();
        }
    }

    //插屏广告
    public override void INTER_VIDEOAD(string ad_id)
    {
        WXCreateInterstitialAdParam param = new WXCreateInterstitialAdParam();
        param.adUnitId = ad_id;
        

        WXInterstitialAd ad = WX.CreateInterstitialAd(param);

        ad.OnError((WXADErrorResponse response)=>{
            //上报事件:广告错误

        });

        ad.Show((WXTextResponse reponse)=>{
            // Debug.Log("Show 成功：" + reponse.errCode);
        }, (WXTextResponse reponse)=>{
            //上报事件:广告错误

        });
    }

    //格子广告
    public override void GRID_VIDEOAD(string ad_id, bool is_show , float rate = 1.0f)
    {
        WXCustomAd ad;
        if (m_CustomADPairs.TryGetValue(ad_id, out ad)) {

        } else {
            var info = WX.GetSystemInfoSync();

            WXCreateCustomAdParam param = new WXCreateCustomAdParam();
            param.adUnitId      = ad_id;
            param.adIntervals   = 30;
            param.style.left    = (int)info.screenWidth / 2 - (int)(180 * rate); //60;
            param.style.top     = (int)info.screenHeight - (int)(120 * rate);

            ad = WX.CreateCustomAd(param);
            m_CustomADPairs.Add(ad_id, ad);

            ad.OnError((WXADErrorResponse response)=>{
                //上报事件:广告错误
            });
        }

        
        if (is_show == true) {
            ad.Show((WXTextResponse reponse)=>{

            }, (WXTextResponse reponse)=>{
                //上报事件:广告错误
            });
        } else {
            ad.Hide();
        }  
    }

    //上报数据
    public override void UPLOADLEVELRANK(int level)
    {
        KVData data = new KVData();
        data.key    = "Level";
        data.value  = level.ToString();

        SetUserCloudStorageOption option = new SetUserCloudStorageOption();
        option.KVDataList = new KVData[1];
        option.KVDataList[0] = data;

        WX.SetUserCloudStorage(option);
    }


    //设备振动
    public override void VIBRATE(string level)
    {
        if (!GameFacade.Instance.SystemManager.VibrateFlag) return;
        
        VibrateShortOption op = new VibrateShortOption();
        op.type = level;
        WX.VibrateShort(op);
    }

    //适配UI
    public override void ADAPTATION(RectTransform rectTransform, float offset)
    {
        var info = WX.GetSystemInfoSync();
 
        // 调整屏幕移到刘海屏下面, 
        float rate = ((float)info.safeArea.top + offset) / (float)info.windowHeight;
        rectTransform.anchorMin = new Vector2(0,  rate);

        rectTransform.anchorMax = new Vector2(1, 1 - rate);

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    public override void UPDATETARGETFRAME(int frame)
    {
        //设置帧率
        WX.SetPreferredFramesPerSecond(frame);
    }

    //打开其他小游戏
    public override void OPENMINIGAME(string appid)
    {
        //
        NavigateToMiniProgramOption option = new NavigateToMiniProgramOption();
        option.appId    = appid;
        option.fail     = (GeneralCallbackResult callback)=>{
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "跳转失败"));
        };


        WX.NavigateToMiniProgram(option);

    
    }

    //游戏圈
    public override void SHOWCLUBBUTTON(bool flag)
    {
        if (flag == true)
        {
            if (m_BtnClub == null)
            {
                var info = WX.GetSystemInfoSync();

                WXCreateGameClubButtonParam param = new WXCreateGameClubButtonParam();
                param.type = GameClubButtonType.image;
                param.icon = GameClubButtonIcon.light;

  
                param.style.left    = 14;//(int)info.screenWidth / 2 - 144; //60;
                param.style.top     = (int)info.screenHeight / 2 + 35;
                param.style.width   = 40;
                param.style.height  = 40;
                m_BtnClub = WX.CreateGameClubButton(param);
            }
            
            m_BtnClub.Show();
        }
        else
        {
            if (m_BtnClub != null) m_BtnClub.Hide();
        }
        

    }
}

#endif