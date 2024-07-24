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


    [HideInInspector] public HashSet<Unit> SpliteIgnoreUnits = new HashSet<Unit>();
    [HideInInspector] public bool IsSplit = false;      //是否是分裂出的
    [HideInInspector] public int PassTimes = 0;         //可穿透次数
    [HideInInspector] public int ReboundTimes = 0;      //可反弹次数
    [HideInInspector] public int KillRate = 0;          //必杀概率

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    
    public void Shoot(float angle , bool is_shoot = true)
    {
        m_Rigidbody.velocity = Vector2.zero;

        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, Speed.ToNumber(), angle);
        m_Rigidbody.AddForce(force);

        //朝向前进的方向
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg);

        EventManager.SendEvent(new GameEvent(EVENT.ONBULLETSHOOT, this));
    }

    void Rebound()
    {
        ReboundTimes--;

        // 计算反弹方向
        float angle = Mathf.Atan2(m_Rigidbody.velocity.y, m_Rigidbody.velocity.x) * Mathf.Rad2Deg;
        //给一个随机的偏移
        angle += 180 + RandomUtility.Random(-45, 45);

        Shoot(angle, false);
    }

    void Dispose()
    {
        SpliteIgnoreUnits.Clear();
        IsSplit         = false;
        PassTimes       = 0;
        ReboundTimes    = 0;
        KillRate        = 0;

        GameFacade.Instance.PoolManager.RecycleBullet(this);
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        //子弹撞墙 则销毁
        if (collider.gameObject.tag == "Wall")
        {
            SpliteIgnoreUnits.Clear();

            if (ReboundTimes >= 0) Rebound();
            if (ReboundTimes < 0) Dispose();

            return;
        }


        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        if (Field.Instance.Hit(this, unit) == true)
        {
            PassTimes--;

            if (PassTimes < 0) Dispose();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        // 当碰撞持续时调用
        // Debug.Log("Player Collision Stay : " + collision.gameObject.name);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // 当碰撞结束时调用
        // Debug.Log("Player Collision Exit : " + collision.gameObject.name);
    }
    #endregion
}
