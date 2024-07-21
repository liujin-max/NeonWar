using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;




//基础数据
[System.Serializable]
public class BaseData
{
    public string openId;
}

//道具数据
[System.Serializable]
public class PropMsg
{
    public int ID;
    public int Count;
}

//任务记录数据
[System.Serializable]
public class TaskMsg
{
    public int ID;
    public int Day; //上次领取的日期
}


//账号数据
[System.Serializable]
public class GameUserData
{
    public string Name = "未知";
    public string HeadUrl;
    public int Level;       //通关记录
    public int Coin;        //金币
    public int Glass;       //碎片
    public int ATK = 1;     //默认1J
    public int ASP = 1;

    public long RecoveryTimestamp;    //上次体力的恢复时间
    public int RegisterDay;     //注册时间(一年中的第几天)
    public int LastLoginDay;    //上次登录的天数
    public int LoginDay;        //登录天数
    public int SignInStatus;  //签到状态

}




//负责管理账号的各种数据
public class User
{
    //用户数据
    private BaseData m_Base = new BaseData();
    public BaseData Base { 
        get { return m_Base; } 
        set { m_Base = value; }
    }

    private GameUserData m_Data = new GameUserData();
    public GameUserData Data {
        get { return m_Data; }
        set { m_Data = value; }
    }

    public string OpenID { get{ return m_Base.openId;}}
    public string Name { get{ return m_Data.Name;}}
    public string HeadURL { get{ return m_Data.HeadUrl;}}
    public int Level { get{ return m_Data.Level;}}
    public int Coin { get{ return m_Data.Coin;}}
    public int Glass { get{ return m_Data.Glass;}}
    public int ATK { get{ return m_Data.ATK;}}
    public int ASP { get{ return m_Data.ASP;}}

    private bool m_userUpdate = false;  //账号数据变动标记
    public bool IsDirty {get { return m_userUpdate;}}

    

    public bool IsSyncFinished = false;
    
    //从存档里同步数据
    public void Sync()
    {
        // //同步账号基础数据(头像、名字)
        // Platform.Instance.LOGIN(()=>{
        //     //同步账号游玩数据(记录、成就等)
        //     Platform.Instance.SYNC();
        // });

        Platform.Instance.SYNC();
        // Platform.Instance.PULLRANK(_C.RANK.ENDLESS, null);
    }

    public void SyncFinish()
    {
        IsSyncFinished = true;
    }

    public void SyncRecords()
    {
        int day_of_year = DateTime.Now.DayOfYear;


        //同步任务信息
        // m_Data.TaskRecords.ForEach(msg => {
        //     if (msg.Day == day_of_year)
        //     {
        //         var t = GameFacade.Instance.DataCenter.Daily.GetTask(msg.ID);
        //         if (t != null) {
        //             t.Receive();
        //         }
        //     }
        // });


        //任务：每日登录
        // GameFacade.Instance.DataCenter.Daily.FinishTask((int)_C.TASK.LOGIN);


        //未注册|新的一年重新开始
        // if (m_Data.RegisterDay == 0 || day_of_year < m_Data.RegisterDay)
        // {
        //     m_Data.RegisterDay = day_of_year;
        //     m_Data.LoginDay    = 0;
 
        //     m_userUpdate = true;
        // }

        // //记录登录天数
        // if (day_of_year != m_Data.LastLoginDay) 
        // {
        //     m_Data.LastLoginDay = day_of_year;
        //     m_Data.LoginDay++;

        //     m_userUpdate = true;
        // }

        

    }


    public void Upload()
    {
        if (!m_userUpdate) return;

        Platform.Instance.UPLOAD(m_Data);

        //重置标记
        m_userUpdate    = false;
    }

    public void SetLevel(int value)
    {
        if (value <= m_Data.Level ) return;

        m_Data.Level = value;

        m_userUpdate = true;
    }

    public void SetCoin(int value)
    {
        m_Data.Coin = value;

        m_userUpdate = true;
    }

    public void UpdateCoin(int value)
    {
        m_Data.Coin  = Mathf.Max(0, m_Data.Coin + value);

        m_userUpdate = true;
    }

    public void UpdateGlass(int value)
    {
        m_Data.Glass = Mathf.Max(0, m_Data.Glass + value);

        m_userUpdate = true;
    }

    public void UpdateATK(int value)
    {
        m_Data.ATK += value;

        m_userUpdate = true;
    }

    public void UpdateASP(int value)
    {
        m_Data.ASP += value;

        m_userUpdate = true;
    }

    public int GetATKCost()
    {
        return NumericalManager.FML_ATKCost(m_Data.ATK);
    }

    public int GetASPCost()
    {
        return NumericalManager.FML_ASPCost(m_Data.ASP);
    }

    public void SetRecoveryTimestamp(long value)
    {
        m_Data.RecoveryTimestamp = value;

        m_userUpdate = true;
    }


    //七日签到
    public void RecordsSignIn(int day)
    {
        m_Data.SignInStatus |= (1 << (day - 1));

        m_userUpdate = true;
    }


    public void Update(float dt)
    {   

        if (m_userUpdate) {
             Upload();
        }
    }
}


