using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;

    [HideInInspector] public string Name;
    [HideInInspector] public Unit Caster;

    public Vector2 Velocity {get { return m_Rigidbody.velocity;}}
    public AttributeValue Speed = new AttributeValue(500);
    public Hit Hit;


    [HideInInspector] public int SplitCount;            //分裂数量
    [HideInInspector] public bool IsSplit = false;      //是否是分裂出的
    [HideInInspector] public int HitRemaining = 1;      //剩余可击中敌人次数(穿透逻辑)
    [HideInInspector] public int ReboundTimes;          //可反弹次数
    [HideInInspector] public bool ReboundUpgrade;       //击中敌人也可反弹
    

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    public void InitStatus()
    {
        Speed.Clear();

        SplitCount      = 0;
        IsSplit         = false;
        HitRemaining    = 1;
        ReboundTimes    = 0;
    }

    public void Init(Unit caster)
    {
        Caster  = caster;
        Hit     = new Hit(caster);

        InitStatus();
    }
    
    public void SyncSpeed()
    {
        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * Speed.ToNumber() / 100.0f;
    }

    public void Shoot(float angle)
    {
        Turn(angle);

        new Event_BulletShoot(){Bullet = this}.Notify();
    }

    public void Turn(float angle)
    {
        m_Rigidbody.velocity = ToolUtility.FindPointOnCircle(Vector2.zero, Speed.ToNumber() / 100.0f, angle);
    }

    void Rebound()
    {
        ReboundTimes--;

        // 计算反弹方向
        float angle = ToolUtility.VectorToAngle(m_Rigidbody.velocity);
        //给一个随机的偏移
        angle += 180 + RandomUtility.Random(-45, 45);

        Turn(angle);
    }

    //分裂逻辑
    void Split(Bullet origin, Unit target)
    {
        if (SplitCount == 0) return;

        for (int i = 0; i < SplitCount; i++)
        {
            var bullet = Field.Instance.CreateBullet(Caster);
            bullet.transform.position = origin.transform.position;
            bullet.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            bullet.IsSplit = true;
            bullet.Hit.IgnoreUnits.Add(target);

            bullet.Shoot(RandomUtility.Random(0, 360));
        }
    }

    //

    public void Dispose()
    {
        Field.Instance.RecycleBullet(this);
    }

    void LateUpdate()
    {
        //朝向前进的方向
        transform.right = m_Rigidbody.velocity;
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        //子弹撞到边界 则销毁
        if (collider.gameObject.tag == CONST.COLLIDER_BOARD)
        {
            Dispose();
            return;
        }

        //子弹撞墙 判断是否反弹
        if (collider.gameObject.tag == CONST.COLLIDER_WALL)
        {
            Hit.IgnoreUnits.Clear();

            if (ReboundTimes > 0) Rebound();
            else if (ReboundTimes <= 0) Dispose();
            
            return;
        }

        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;
        if (unit.Side == Caster.Side) return;
        if (HitRemaining <= 0) return;


        Hit.Position = transform.localPosition;
        Hit.Velocity = this.Velocity;

        if (Field.Instance.SettleHit(Hit, unit) == true) {
            new Event_BulletHit(){Bullet = this, Target = unit}.Notify();

            //分裂检测
            Split(this, unit);

            //目标反弹
            if (ReboundUpgrade == true)
            {
                if (ReboundTimes > 0) Rebound();
                else if (ReboundTimes <= 0) Dispose();
                return;
            }

            HitRemaining--;

            if (HitRemaining <= 0) Dispose();
        }
    }
    #endregion
}
