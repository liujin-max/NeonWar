using System;
using System.Collections.Generic;
using UnityEngine;







//全局数据类
public class DataCenter
{
    //账号信息
    public User User;
    public League League;
    public Levels Levels;
    public Backpack Backpack;


    private static DataCenter m_Instance;
    public static DataCenter Instance {
        get {
            if (m_Instance == null) m_Instance = new DataCenter();

            return m_Instance;
        }
    }


    public void Init()
    {
        //账号数据
        User    = new User();

        League  = new League();
        League.Init();

        Levels  = new Levels();

        Backpack= new Backpack();
        Backpack.Init();
    }

    public void Update(float dt)
    {
        User.Update(dt);
    }

    public bool IsPearUnlock()
    {
        return User.Level >= _C.PEAR_UNLOCK_LEVEL;
    }

    public string GetLevelString()
    {
        int level   = User.Level;

        int chapter = level / 20 + 1;
        int stage   = level % 20 + 1;

        return string.Format("{0}-{1}", chapter, stage);
    }
}

