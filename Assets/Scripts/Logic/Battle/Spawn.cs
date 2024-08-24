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
public class Spawn
{
    //不在这里初始化
    private List<MonsterJSON> m_EnemyPool;
    private List<Enemy> m_Enemys;
    public List<Enemy> Enemys {get {return m_Enemys;}}

    private List<Enemy> m_EnemyRemoves  = new List<Enemy>();
    private List<Enemy> m_EnemyTemps    = new List<Enemy>();

    private int m_AsyncCount    = 0;

    //击杀进度
    private Pair m_KillProgress;
    public float KillProgress {get {return m_KillProgress.Progress;}}
    

    public void Init(LevelJSON level_json)
    {
        int enemy_max = level_json.Monsters.Count;

        //根据已知的大小做初始化、避免频繁扩容
        m_EnemyPool = new List<MonsterJSON>(enemy_max);
        m_EnemyPool.AddRange(level_json.Monsters);

        m_Enemys    = new List<Enemy>(enemy_max);


        m_KillProgress  = new Pair(0, enemy_max);
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
        return m_EnemyPool.Count == 0 && m_Enemys.Count == 0 && m_AsyncCount == 0;
    }

    void PutEnemy(MonsterJSON monsterJSON)
    {
        Vector2 point = new Vector2(RandomUtility.Random(-200, 201) / 100.0f, RandomUtility.Random(-200, 201) / 100.0f);

        if (monsterJSON.Angle != -1) 
        {
            point = ToolUtility.FindPointOnCircle(Vector3.zero, monsterJSON.Radius, monsterJSON.Angle);
        }

        m_AsyncCount++;
        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Enemy/" + monsterJSON.ID, point, Field.Instance.Land.ENEMY_ROOT, (obj)=>{
            var enemy = obj.GetComponent<Enemy>();
            m_Enemys.Add(enemy);
            enemy.gameObject.SetActive(false);
            enemy.SetValid(false);

            var hole = GameFacade.Instance.EffectManager.Load(EFFECT.BLACKHOLE, point, Field.Instance.Land.ELEMENT_ROOT.gameObject).transform;
            hole.localScale = Vector3.zero;
            
            Sequence seq = DOTween.Sequence();
            seq.Append(hole.DOScale(Vector3.one, 0.5f));
            seq.AppendInterval(0.4f);
            
            seq.AppendCallback(()=>{
                enemy.gameObject.SetActive(true);
                enemy.SetValid(true);
                enemy.Init(monsterJSON);
                enemy.Push();

                if (monsterJSON.Type == _C.ENEMY_TYPE.BOSS) enemy.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            });

            seq.AppendInterval(0.2f);
            seq.Append(hole.DOScale(1.3f, 0.15f));
            seq.Append(hole.DOScale(0, 0.4f));
            seq.Play();

            m_AsyncCount--;
        });
    }

    //分裂
    public void Summon(MonsterJSON monsterJSON, Vector2 point)
    {
        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Enemy/" + monsterJSON.ID, point, Field.Instance.Land.ENEMY_ROOT, (obj)=>{
            var enemy = obj.GetComponent<Enemy>();
            enemy.Init(monsterJSON);
            enemy.IsSummon = true;
            enemy.Push();
            
            m_Enemys.Add(enemy);
        });
    }

    //挑选周围怪物最多的怪
    public Enemy FindEnemyGather(float radius)
    {
        int round_count = 0;
        Enemy target    = null;

        foreach (var enemy in m_Enemys)
        {
            Vector2 o_pos = enemy.transform.localPosition;
            int count = m_Enemys.Count(e => Vector3.Distance(o_pos, e.transform.localPosition) <= radius);
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
        if (m_EnemyPool.Count > 0)
        {
            //场上没有怪物了，则集体速减CD
            if (m_Enemys.Count == 0) {
                float time = m_EnemyPool[0].Time;

                m_EnemyPool.ForEach(monster_json => {
                    monster_json.Time -= time;
                });
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
        

        //销毁死亡的敌人
        m_EnemyTemps.Clear();
        m_EnemyTemps.AddRange(m_Enemys);
        
        m_EnemyTemps.ForEach(e => {
            e.CustomUpdate(deltaTime);

            if (e.IsDead() == true) {
                if (!e.IsSummon) {
                    m_KillProgress.UpdateCurrent(1);
                    Field.Instance.UpdateGlass(e.Glass);
                }

                m_EnemyRemoves.Add(e);
            }
        });

        if (m_EnemyRemoves.Count > 0) {
            m_EnemyRemoves.ForEach(e => {
                m_Enemys.Remove(e);

                e.Dispose();
            });

            m_EnemyRemoves.Clear();
        }
    }

    public void Dispose()
    {
        m_AsyncCount = 0;

        m_EnemyPool.Clear();
        m_KillProgress.Clear();

        m_Enemys.ForEach(e => {
            e.Dispose();
        });
        m_Enemys.Clear();
    }
}
