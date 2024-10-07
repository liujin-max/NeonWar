using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//沙虫头部
//管理所有身体节点
//周期性释放子弹
public class Enemy_204 : Enemy
{
    [SerializeField] int m_BodyCount;


    List<Enemy> m_Bodys = new List<Enemy>();
    //每截身体的坐标间隔是5帧
    int m_Step = 20;
    int m_PosCount;
    List<Vector3> m_Posotions = new List<Vector3>();



    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        //免疫位移、控制
        ImmuneDisplaceFlag  = true;
        ImmuneControlFlag   = true;
        
        m_PosCount = (m_BodyCount + 1) * m_Step;

        InitBodys();

        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        base.Dispose();

        foreach (var e in m_Bodys) e.ForceDead();
        
        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void DoAttack()
    {
        int random = RandomUtility.Random(0, 360);
        
        Field.Instance.CreateBullet(this).Shoot(random);
        Field.Instance.CreateBullet(this).Shoot(random + 120);
        Field.Instance.CreateBullet(this).Shoot(random - 120);
    }

    void InitBodys()
    {
        //躯干
        for (int i = 0; i < m_BodyCount; i++)
        {
            Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 255, HP = Mathf.CeilToInt(ATT.HPMAX * 5f)}, transform.localPosition, (e)=>{
                m_Bodys.Add(e);
            });
        }

        //尾巴
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 256, HP = Mathf.CeilToInt(ATT.HPMAX * 1f)}, transform.localPosition, (e)=>{
            m_Bodys.Add(e);
        });
    }

    void FixedUpdate()
    {
        if (this.IsControlled()) return;

        if (m_Posotions.Count > m_PosCount) m_Posotions.RemoveAt(m_Posotions.Count - 1);
        m_Posotions.Insert(0, transform.position);

        for (int i = 0; i < m_Bodys.Count; i++)
        {
            var e = m_Bodys[i];
            int order = (i + 1) * m_Step;

            if (m_Posotions.Count > order) e.transform.position = m_Posotions[order];
        }
    }

    #region 碰撞检测
    void OnCollisionExit2D(Collision2D collision)
    {
        //撞墙随机反弹
        if (collision.gameObject.tag == CONST.COLLIDER_WALL)
        {
            float angle = ToolUtility.VectorToAngle(m_Rigidbody.velocity);
            //给一个随机的偏移
            angle += RandomUtility.Random(-45, 45);

            Push(angle);
        }
    }
    #endregion


    #region 事件监听
    private void OnKillEnemy(GameEvent @event)
    {
        Enemy e = (Enemy)@event.GetParam(0);
        if (e == this) return;

        int order = -1;
        for (int i = 0; i < m_Bodys.Count; i++)
        {
            var b = m_Bodys[i];
            if (b == e) {
                order = i;
                break;
            }
        }

        if (order == -1) return;
        
        for (int i = m_Bodys.Count - 1; i >= order; i--)
        {
            var b = m_Bodys[i];
            b.ForceDead();

            m_Bodys.Remove(b);
        }
    }
    #endregion
    
}
