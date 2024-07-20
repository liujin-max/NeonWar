using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//怪物工厂
//负责敌人的生产逻辑
public class Spawn
{
    private List<Enemy> m_Enemys = new List<Enemy>();

    private Pair m_Pool = new Pair(0, 0);

    //现在生产怪物看的是时间间隔
    private CDTimer m_Timer = new CDTimer(1.5f);


    public void Init(int enemy_count)
    {

    }

    void InitEnemy()
    {
        int hp = Field.Instance.FML_EnemyHP(Field.Instance.Level.ID);

        Vector2 point = new Vector2(RandomUtility.Random(-200, 201) / 100.0f, RandomUtility.Random(-200, 201) / 100.0f);
        var enemy = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Enemy", point, Field.Instance.Land.ENEMY_ROOT).GetComponent<Enemy>();
        enemy.Push(160);
        enemy.Init(hp);

        m_Enemys.Add(enemy);
    }

    public void Pause()
    {
        m_Enemys.ForEach(e => e.Pause());
    }

    public void Resume()
    {
        m_Enemys.ForEach(e => e.Resume());
    }

    public void Update(float deltaTime)
    {
        //按时间间隔产出敌人
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished() == true)
        {
            m_Timer.Reset();

            InitEnemy();
        }

        //销毁死亡的敌人
        List<Enemy> _Removes = new List<Enemy>();
        m_Enemys.ForEach(e => {
            if (e.IsDead() == true) {
                Field.Instance.UpdateGlass(e.ATT.Glass);

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
