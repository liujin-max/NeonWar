using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//沙虫头部
//管理所有身体节点
//周期性释放子弹
public class Enemy_207 : Enemy
{
    [SerializeField] int m_BodyCount;

    List<Enemy> m_Bodys = new List<Enemy>();
    private Vector3 m_LastPosition = Vector3.zero;
    private bool HasTail = true;

    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        //免疫位移、控制
        ImmuneDisplaceReference++;
        ImmuneControlReference++;
        
        InitBodys();

        Event_KillEnemy.OnEvent += OnKillEnemy;
    }

    public override void Dispose()
    {
        base.Dispose();

        Event_KillEnemy.OnEvent -= OnKillEnemy;
    }

    void InitBodys()
    {
        SetSortingOrder(m_BodyCount + 1);

        //身体
        for (int i = 0; i < m_BodyCount; i++)
        {
            int count = i;
            Field.Instance.Spawn.Summon(new MonsterJSON(){ ID = 255, HP = Mathf.CeilToInt(ATT.HPMAX * 0.5f)}, transform.localPosition, (e)=>{
                e.GetComponent<Enemy_255>().Head = this;

                e.SetSortingOrder(m_BodyCount - count);
                e.transform.position = transform.position;

                m_Bodys.Add(e);
            });
        }

        //尾巴
        Field.Instance.Spawn.Summon(new MonsterJSON(){ ID = 256, HP = Mathf.CeilToInt(ATT.HPMAX * 0.4f)}, transform.localPosition, (e)=>{
            e.GetComponent<Enemy_256>().Head = this;
            e.SetSortingOrder(0);
            m_Bodys.Add(e);
        });
    }

    public override void DoAttack()
    {
        Roar();
        
        float random = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);
        
        Field.Instance.CreateBullet(this).Shoot(random);
        Field.Instance.CreateBullet(this).Shoot(random + 120);
        Field.Instance.CreateBullet(this).Shoot(random - 120);
    }

    public override void Dead(Hit hit = null)
    {
        var temp_bodys = new List<Enemy>(m_Bodys);
        m_Bodys.Clear();

        foreach (var e in temp_bodys) {
            e.ForceDead();
            e.Dead(hit);
        }

        base.Dead(hit);
    }

    void FixedUpdate()
    {
        Enemy focus = this;
        float speed = ATT.SPEED.ToNumber() / 100.0f;
        foreach (var body in m_Bodys)
        {
            //逐渐移动物体 A 到物体 B 的位置
            body.transform.position = Vector3.Lerp(body.transform.position, focus.transform.position, speed * Time.deltaTime);
            body.SetRotation(ToolUtility.VectorToAngle(focus.transform.localPosition - body.transform.position) + 90);

            focus = body;
        }

        //转向
        SetRotation(ToolUtility.VectorToAngle(transform.localPosition - m_LastPosition) + 90);
        m_LastPosition = transform.localPosition;
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
    private void OnKillEnemy(Event_KillEnemy evt)
    {
        Enemy e = evt.Enemy;
        if (e == this) return;

        var hit = evt.Hit;

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
        
        var temp = new List<Enemy>();
        for (int i = m_Bodys.Count - 1; i >= order; i--)
        {
            var b = m_Bodys[i];
            temp.Add(b);
            m_Bodys.Remove(b);
        }

        foreach (var b in temp)
        {
            b.ForceDead();
            b.Dead(hit);
        }

        //如果身体全毁 则自身死亡
        if (m_Bodys.Count == 0) 
        {
            ForceDead();
            Dead(hit);
        }
    }
    #endregion
    
}
