using System;
using System.Collections.Generic;
using UnityEngine;




//数据体
public class CardData
{
    public int ID;
    public string Name;
    public _C.CARD_TYPE Type;
    public bool Breakable;
    public bool Dragable;
    public bool IsFixed;
    public bool Emoji;
}



//从本地Json文件读取
[System.Serializable]
public class StageJSON
{
    public int ID;
    public int Weight;
    public int Height;
    public int Step;
    public int Time;
    public int Coin;
    public int Food;
    public List<string> Conditions;
    public List<_C.CARD> CardPool;
}

//从本地Json文件读取
[System.Serializable]
public class GridJSON
{
    public int Order;
    public int X;
    public int Y;

    public bool IsValid = true;
    public _C.CARD JellyID;
    public _C.CARD_TAG CardTag = _C.CARD_TAG.NONE;
    public Vector2 Portal;

    public bool IsBan;
    public _C.DIRECTION BeltDirection = _C.DIRECTION.NONE;
}



//全局数据类
public class DataCenter
{
    //方块信息
    private Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    private List<CardData> m_Cards = new List<CardData>();



    //账号信息
    public User User;


    public void Init()
    {
        //账号数据
        User = new User();
    }
}

