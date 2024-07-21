using System;
using System.Collections.Generic;
using UnityEngine;







//全局数据类
public class DataCenter
{
    //账号信息
    public User User;

    public Levels Levels;


    public void Init()
    {
        //账号数据
        User    = new User();

        Levels  = new Levels();
        Levels.Init();
    }
}

