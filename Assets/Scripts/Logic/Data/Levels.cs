using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡Json数据
[System.Serializable]
public struct LevelJSON
{
    public int Glass;
    public int PearPool;
    public string PearCount;
    public WaveJSON[] Waves;
}

//战斗波数据
[System.Serializable]
public struct WaveJSON
{
    public MonsterJSON[] Monsters;
}

//怪物Json数据
[System.Serializable]
public class MonsterJSON
{
    public float Time;
    public int ID;
    public _C.ENEMY_TYPE Type = _C.ENEMY_TYPE.NORMAL;
    public int HP;
    //掉落的碎片数量
    public int Glass;
    //和圆点的角度
    public int Angle = -1;
    public int Radius;
    public int[] Buffs = {};
}



//关卡管理器
public class Levels
{
    //关卡数据
    private Dictionary<int, Level> m_Levels = new Dictionary<int, Level>();


    //获取关卡(用到的时候再生成)
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

    public LevelJSON? LoadLevelJSON(int id)
    {
        var path = "Json/" + id;

        TextAsset jsonAsset = Resources.Load<TextAsset>(path);
        if (jsonAsset != null) 
        {
            return JsonUtility.FromJson<LevelJSON>(jsonAsset.text);
        }
        
        Debug.LogError("未读取到配置：" + path);
        return null;
    }
}
