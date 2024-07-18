using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class RandomUtility
{
    private static System.Random _Random = new System.Random();

    public static void InitSeed(int seed)
    {
        _Random = new System.Random(seed);
    }

    // max最大值（不包含）
    public static int Random(int min, int max) 
    {
        return _Random.Next(min, max);
    }

    public static bool IsHit(int value, int rate = 1)
    {
        return _Random.Next(1, 100 * rate) <= value;
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _Random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<object> Pick(int number, List<object> list)
    {
        List<object> temp = new List<object>();
        temp.AddRange(list);

        
        List<object> pickes = new List<object>();

        for (int i = 0; i < number; i++)
        {
            if (temp.Count == 0) {
                break;
            }

            int rand    = Random(0, temp.Count);
            var item    = temp[rand];
            pickes.Add(item);
            temp.Remove(item);
        }


        return pickes;
    }


    //根据权重
    public static object PickByWeight(Dictionary<object, int> objs)
    {
        // 计算总权重
        int totalWeight = 0;
        foreach (var item in objs) {
            totalWeight += item.Value;
        }

        // 生成随机数
        int randomNumber = _Random.Next(totalWeight);

        // 根据权重选择
        foreach (var o in objs)
        {
            if (randomNumber < o.Value) {
                return o.Key;
            }
            randomNumber -= o.Value;
        }

        // 如果所有的权重之和小于随机数，则返回最后一个
        return objs.Last().Key;
    }
}
