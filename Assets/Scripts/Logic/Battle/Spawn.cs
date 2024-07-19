using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//怪物工厂
//负责敌人的生产逻辑
public class Spawn
{
    private List<Enemy> m_Enemys = new List<Enemy>();

    //现在生产怪物看的是时间间隔
    private CDTimer m_Timer = new CDTimer(1.5f);



    void InitEnemy()
    {
        Vector2 point = new Vector2(RandomUtility.Random(-100, 101) / 100.0f, RandomUtility.Random(-100, 101) / 100.0f);
        var enemy = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Enemy", point, Field.Instance.Land.ENEMY_ROOT).GetComponent<Enemy>();
        enemy.Push(160);

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
            if (e.IsDead() == true) _Removes.Add(e);
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
