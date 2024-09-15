using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


//怪物工厂
//负责敌人的生产逻辑
//关卡数据由表格配置，包括出场怪物ID、出场时间，当前血量等
//通过计时器的方式有序出场
//当场上没有怪物时，使序列中的下一个怪物立即出场

//每场战斗由20波组成，每波结束后玩家进行升级
public class Spawn
{
    private List<Wave> m_Waves;
    private int m_WaveOrder;
    public string Order {get {return (m_WaveOrder + 1).ToString();}}

    public Wave CurrentWave { get {return m_Waves[m_WaveOrder]; }}
    public List<Enemy> Enemys {get {return CurrentWave.Enemys; }}


    //击杀进度
    public float KillProgress {get {return (m_WaveOrder + 1) / (float) m_Waves.Count;}}
    

    public void Init(LevelJSON level_json)
    {
        m_Waves    = new List<Wave>(level_json.Waves.Length);
        m_WaveOrder= 0;

        foreach (var wave_json in level_json.Waves) {
            var wave = new Wave(wave_json);
            m_Waves.Add(wave);
        }
    }

    public void Pause()
    {
        CurrentWave.Pause();
    }

    public void Resume()
    {
        CurrentWave.Resume();
    }

    public bool IsCurrentClear()
    {
        return CurrentWave.IsClear();
    }

    public bool IsClear()
    {
        foreach (var wave in m_Waves) {
            if (!wave.IsClear()) {
                return false;
            }
        }
        return true;
    }

    public void NextWave()
    {
        m_WaveOrder++;
    }

    //分裂
    public void Summon(MonsterJSON monsterJSON, Vector2 point)
    {
        CurrentWave.Summon(monsterJSON, point);
    }

    //挑选周围怪物最多的怪
    public Enemy FindEnemyGather(float radius)
    {
        int round_count = 0;
        Enemy target    = null;

        foreach (var enemy in Enemys)
        {
            Vector2 o_pos = enemy.transform.localPosition;
            int count = Enemys.Count(e => Vector3.Distance(o_pos, e.transform.localPosition) <= radius);
            if (count > round_count)
            {
                round_count = count;
                target = enemy;
            }
        }

        return target;
    }

    public void CustomUpdate(float deltaTime)
    {
        CurrentWave.CustomUpdate(deltaTime);
    }

    public void Dispose()
    {
        foreach (var wave in m_Waves) wave.Dispose();
        m_Waves.Clear();
    }
}
