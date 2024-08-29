using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public static class ToolUtility
{
    private static StringBuilder m_FormateBuilder = new StringBuilder();

    public static long GetUnixTimestamp()
    {
        long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        return unixTimestamp;
    }


    public static string Second2Minute(int seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secondsLeft = seconds % 60;
        string result = minutes.ToString("D1") + ":" + secondsLeft.ToString("D2");

        return result;
    }

    //根据圆心、半径和角度，计算出坐标点
    public static Vector2 FindPointOnCircle(Vector2 center, float radius, float angle)
    {
        // 将角度转换为弧度
        float radians = angle * Mathf.Deg2Rad;

        // 计算圆上一定角度外的某个点的坐标
        Vector2 point = new Vector2(
            center.x + radius * Mathf.Cos(radians),
            center.y + radius * Mathf.Sin(radians));

        return point;
    }

    // 将角度转换为单位向量
    public static Vector2 AngleToVector(float angleInDegrees)
    {
        // 将角度转换为弧度
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        
        // 计算单位向量的 x 和 y 分量
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);
        
        // 返回单位向量
        return new Vector2(x, y);
    }

    // 将向量转换成角度
    public static float VectorToAngle(Vector3 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    //数字转换成单位
    public static string FormatNumber(double num)
    {
        m_FormateBuilder.Clear();

        if (num >= 1000000000)
        {
            m_FormateBuilder.Append((num / 1000000000D).ToString("0.#")).Append("B");
        }
        else if (num >= 1000000)
        {
            m_FormateBuilder.Append((num / 1000000D).ToString("0.#")).Append("M");
        }
        else if (num >= 1000)
        {
            m_FormateBuilder.Append((num / 1000D).ToString("0.#")).Append("K");
        }
        else
        {
            m_FormateBuilder.Append(num);
        }

        return m_FormateBuilder.ToString();
    }
}
