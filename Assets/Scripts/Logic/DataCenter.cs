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


    public void Init()
    {
        //账号数据
        User    = new User();

        League  = new League();
        League.Init();

        Levels  = new Levels();
    }


    public void Update(float dt)
    {
        User.Update(dt);
    }
}

