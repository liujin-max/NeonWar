using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//怪物工厂
//负责敌人的生产逻辑
public class Spawn
{
    private List<Enemy> m_Enemys = new List<Enemy>();

    private Pair m_Pool = new Pair(0, 0);

    //现在生产怪物看的是时间间隔
    private CDTimer m_Timer = new CDTimer(6f);


    public void Init(int enemy_count)
    {
        Debug.Log("怪物数量：" + enemy_count);
        m_Pool.Reset(0, enemy_count);
    }

    void PutEnemy()
    {
        int hp = NumericalManager.FML_EnemyHP(Field.Instance.Level.ID);

        Vector2 point = new Vector2(RandomUtility.Random(-200, 201) / 100.0f, RandomUtility.Random(-200, 201) / 100.0f);

        int enemy_id    = RandomUtility.Random(100, 104);

        var enemy = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Enemy/" + enemy_id, point, Field.Instance.Land.ENEMY_ROOT).GetComponent<Enemy>();
        enemy.Init(enemy_id, hp);
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
        return m_Pool.IsFull() && m_Enemys.Count == 0;
    }

    public void Update(float deltaTime)
    {
        if (!m_Pool.IsFull())
        {
            if (m_Enemys.Count == 0) m_Timer.Full();

            //按时间间隔产出敌人
            m_Timer.Update(deltaTime);
            if (m_Timer.IsFinished() == true)
            {
                m_Timer.Reset();
                m_Timer.SetCurrent(RandomUtility.Random(0, Mathf.FloorToInt(m_Timer.Duration * 100)) / 100.0f);

                m_Pool.UpdateCurrent(1);

                PutEnemy();
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
        m_Pool.Reset(0, 0);
        
        m_Enemys.ForEach(e => {
            e.Dispose();
        });
        m_Enemys.Clear();
    }
}
