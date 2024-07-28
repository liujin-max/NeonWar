using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡Json数据
[System.Serializable]
public class LevelJSON
{
    public List<MonsterJSON> Monsters;
}

//怪物Json数据
[System.Serializable]
public class MonsterJSON
{
    public float Time;
    public int ID;
    public int HP;
    //掉落的碎片数量
    public int Glass;
    //出现时的运行角度
    public int Angle;
}



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

    public LevelJSON LoadLevelJSON(int id)
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
