using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public static class ToolUtility
{
    public static void ApplicationQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Application.Quit();
        #endif
    }

    public static bool Approximately(Vector3 v1, Vector3 v2) {
        return Vector3.SqrMagnitude(v1 - v2) < float.Epsilon;
    }

    public static Vector2[] GenerateRandomPoints(Vector2 topLeft, Vector2 bottomRight, int N, float minDistance)
    {
        List<Vector2> points = new List<Vector2>();
        HashSet<Vector2> usedPoints = new HashSet<Vector2>();

        int count = 0;  //防卡死机制
        while (points.Count < N)
        {
            Vector2 randomPoint = new Vector2(RandomUtility.Random((int)(topLeft.x * 100), (int)(bottomRight.x * 100)) / 100.0f, RandomUtility.Random((int)(bottomRight.y * 100), (int)(topLeft.y * 100)) / 100.0f);

            bool isValid = true;
            foreach (Vector2 existingPoint in usedPoints)
            {
                if (Vector2.Distance(randomPoint, existingPoint) < minDistance)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                count = 0;
                points.Add(randomPoint);
                usedPoints.Add(randomPoint);
            }
            else
            {
                count++;
            }

            //单次随机次数超出1000次，直接跳出，免得死循环了
            if (count >= 1000) {
                Debug.LogError("测试输出 order ： " + points.Count);
                break;
            }
        }

        return points.ToArray();
    }

    public static double TimeStamp()
    {
        DateTime currentTime = DateTime.UtcNow;

        DateTime unixEpoch  = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan timeSpan   = currentTime - unixEpoch;
        double timestamp    = timeSpan.TotalSeconds;
        return timestamp;
    }

    public static string Second2Minute(int seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secondsLeft = seconds % 60;
        string result = minutes.ToString("D1") + ":" + secondsLeft.ToString("D2");

        return result;
    }

    public static Vector2 FindPointOnCircle(Vector3 center, float radius, float angle)
    {
        // 将角度转换为弧度
        float radians = angle * Mathf.Deg2Rad;

        // 计算圆上一定角度外的某个点的坐标
        Vector2 point = new Vector2(
            center.x + radius * Mathf.Cos(radians),
            center.y + radius * Mathf.Sin(radians));

        return point;
    }

    public static long GetUnixTimestamp()
    {
        long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        return unixTimestamp;
    }

    
}
