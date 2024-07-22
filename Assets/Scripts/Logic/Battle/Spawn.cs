using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//怪物工厂
//负责敌人的生产逻辑
//关卡数据由表格配置，包括出场怪物ID、出场时间，当前血量等
//通过计时器的方式有序出场
//当场上没有怪物时，使序列中的下一个怪物立即出场
public class Spawn
{
    private LevelJSON m_LevelJSON;
    private List<Enemy> m_Enemys = new List<Enemy>();
    public List<Enemy> Enemys {get {return m_Enemys;}}

    public void Init(int level_id)
    {
        m_LevelJSON = GameFacade.Instance.DataCenter.Levels.LoadLevelJSON(level_id);

        Debug.Log("怪物数量：" + m_LevelJSON.Monsters.Count);
    }

    void PutEnemy(MonsterJSON monsterJSON)
    {
        Vector2 point   = new Vector2(RandomUtility.Random(-200, 201) / 100.0f, RandomUtility.Random(-200, 201) / 100.0f);

        int enemy_id    = monsterJSON.ID;
        int enemy_hp    = monsterJSON.HP;

        var enemy = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Enemy/" + enemy_id, point, Field.Instance.Land.ENEMY_ROOT).GetComponent<Enemy>();
        enemy.Init(enemy_id, enemy_hp);
        enemy.gameObject.SetActive(false);
        m_Enemys.Add(enemy);



        var hole = GameFacade.Instance.EffectManager.Load(EFFECT.BLACKHOLE, point, Field.Instance.Land.ELEMENT_ROOT.gameObject).transform;
        hole.localScale = Vector3.zero;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(hole.DOScale(Vector3.one, 0.5f));
        seq.AppendInterval(0.4f);
        
        seq.AppendCallback(()=>{
            enemy.gameObject.SetActive(true);
            enemy.Push();
        });

        seq.AppendInterval(0.2f);
        seq.Append(hole.DOScale(1.3f, 0.15f));
        seq.Append(hole.DOScale(0, 0.4f));
        seq.Play();
    }

    public void Pause()
    {
        m_Enemys.ForEach(e => e.Pause());
    }

    public void Resume()
    {
        m_Enemys.ForEach(e => e.Resume());
    }

    public bool IsClear()
    {
        return m_LevelJSON.Monsters.Count == 0 && m_Enemys.Count == 0;
    }

    public void Update(float deltaTime)
    {
        if (m_LevelJSON.Monsters.Count > 0)
        {
            if (m_Enemys.Count == 0) m_LevelJSON.Monsters[0].Time = 0;

            m_LevelJSON.Monsters.ForEach(monster_json => {
                monster_json.Time -= deltaTime * 1000f;
            });

            for (int i = m_LevelJSON.Monsters.Count - 1; i >= 0; i--)
            {
                var monster_json    = m_LevelJSON.Monsters[i];
                monster_json.Time   -= deltaTime * 1000f;

                if (monster_json.Time <= 0)
                {
                    PutEnemy(monster_json);

                    m_LevelJSON.Monsters.Remove(monster_json);
                }
            }
        }
        

        //销毁死亡的敌人
        List<Enemy> _Removes = new List<Enemy>();
        m_Enemys.ForEach(e => {
            if (e.IsDead() == true) {
                Field.Instance.UpdateGlass(e.Glass);

                _Removes.Add(e);
            }
        });

        _Removes.ForEach(e => {
            m_Enemys.Remove(e);

            e.Dispose();
        });
    }

    public void Dispose()
    {
        m_Enemys.ForEach(e => {
            e.Dispose();
        });
        m_Enemys.Clear();
    }
}
