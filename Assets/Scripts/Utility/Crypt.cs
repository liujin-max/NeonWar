using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 加密工具
/// 目前主要负责给int数值做加密
/// </summary>
public class Crypt
{
    //定义一个密钥
    private static readonly int Key = RandomUtility.Random(100, 9999); // 任意一个整数作为密钥

    // 加密
    public static int EN(int value)
    {
        return value ^ Key; // 使用异或操作进行加密
    }

    // 解密
    public static int DE(int value)
    {
        return value ^ Key; // 使用相同的密钥进行异或操作解密
    }
}
