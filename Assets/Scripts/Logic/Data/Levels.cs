using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡管理器
public class Levels
{
    //关卡数据
    private Dictionary<int, Level> m_Levels = new Dictionary<int, Level>();

    //怪物数据

    public void Init()
    {

    }


    //获取关卡
    public Level GetLevel(int n)
    {
        Level level;
        if (m_Levels.TryGetValue(n, out level)) {
            return level;
        }

        level = new Level(n);
        m_Levels[n] = level;
        return level;
    }
}
