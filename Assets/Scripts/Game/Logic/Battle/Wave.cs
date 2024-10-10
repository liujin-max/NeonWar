using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//波
//一场战斗有20波，每波结束可以升级
public class Wave
{
    //不在这里初始化
    private List<MonsterJSON> m_EnemyPool;
    private List<Enemy> m_Enemys;
    public List<Enemy> Enemys {get {return m_Enemys;}}
    public List<SpawnThread> m_Threads = new List<SpawnThread>();
    

    private List<Enemy> m_EnemyRemoves  = new List<Enemy>();
    private List<Enemy> m_EnemyTemps    = new List<Enemy>();
    private int m_AsyncReference = 0;

    public Pair Progress;


    public Wave(WaveJSON waveJSON)
    {
        Progress = new Pair(0, waveJSON.Monsters.Length);

        //根据已知的大小做初始化、避免频繁扩容
        m_EnemyPool = new List<MonsterJSON>(Progress.Total);
        m_EnemyPool.AddRange(waveJSON.Monsters);

        m_Enemys    = new List<Enemy>(Progress.Total);
    }

    public void Pause()
    {
        m_Enemys.ForEach(e => e.Stop());
    }

    public void Resume()
    {
        m_Enemys.ForEach(e => e.Resume());
    }

    public bool IsClear()
    {
        return m_EnemyPool.Count == 0 && m_Enemys.Count == 0 && m_Threads.Count == 0 && m_AsyncReference == 0;
    }

    void PutEnemy(MonsterJSON monsterJSON)
    {
        Vector2 point = new Vector2(RandomUtility.Random(-200, 201) / 100.0f, RandomUtility.Random(-200, 201) / 100.0f);

        if (monsterJSON.Angle != -1) 
        {
            point = ToolUtility.FindPointOnCircle(Vector3.zero, monsterJSON.Radius, monsterJSON.Angle);
        }

        SpawnThread thread = new SpawnThread();
        thread.Start(m_Enemys, monsterJSON, point);
        m_Threads.Add(thread);
    }

    //分裂
    public void Summon(MonsterJSON monsterJSON, Vector2 point, Action<Enemy> callback)
    {
        m_AsyncReference++;
        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Enemy/" + monsterJSON.ID, point, Field.Instance.Land.ENEMY_ROOT, (obj)=>{
            var enemy = obj.GetComponent<Enemy>();
            enemy.Init(monsterJSON);
            enemy.IsSummon = true;
            enemy.Appear();
            enemy.Push(RandomUtility.Random(0, 360));
            
            m_Enemys.Add(enemy);
            m_AsyncReference--;

            if (callback != null) callback(enemy);
        });
    }

    public void CustomUpdate(float deltaTime)
    {
        //怪物池线程
        if (m_EnemyPool.Count > 0)
        {
            //场上没有怪物了，则集体速减CD
            if (m_Enemys.Count == 0) {
                float time = m_EnemyPool[0].Time;

                foreach (var monster_json in m_EnemyPool) {
                    monster_json.Time -= time;
                }
            }

            for (int i = m_EnemyPool.Count - 1; i >= 0; i--)
            {
                var monster_json    = m_EnemyPool[i];
                monster_json.Time   -= deltaTime * 1000f;

                if (monster_json.Time <= 0)
                {
                    PutEnemy(monster_json);

                    m_EnemyPool.Remove(monster_json);
                }
            }
        }

        //怪物出场线程
        for (int i = m_Threads.Count - 1; i >= 0; i--)
        {
            var thread = m_Threads[i];
            thread.CustomUpdate(deltaTime);

            if (thread.IsFinished) m_Threads.Remove(thread);
        }
        

        //销毁死亡的敌人
        m_EnemyTemps.Clear();
        m_EnemyTemps.AddRange(m_Enemys);
        foreach (var e in m_EnemyTemps)
        {
            e.CustomUpdate(deltaTime);

            if (e.IsDead() == true) {
                m_EnemyRemoves.Add(e);
                if (!e.IsSummon) {
                    Progress.UpdateCurrent(1);
                    EventManager.SendEvent(new GameEvent(EVENT.UI_ENEMYDEAD, Progress.Current));
                }
            }
        }

        if (m_EnemyRemoves.Count > 0) {
            foreach (var e in m_EnemyRemoves) {
                m_Enemys.Remove(e);

                e.Dispose();
            }

            m_EnemyRemoves.Clear();
        }
    }

    public void Dispose()
    {
        m_EnemyPool.Clear();

        m_Enemys.ForEach(e => {
            e.Dispose();
        });
        m_Enemys.Clear();

        m_Threads.ForEach(t => {
            t.Dispose();
        });
        m_Threads.Clear();
    }
}
