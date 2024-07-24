using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;

    [HideInInspector] public string Name;
    [HideInInspector] public Unit Caster;

    public Vector2 Velocity {get { return m_Rigidbody.velocity;}}
    public float Speed = 500;
    private float SpeedRate = 1.0f;


    [HideInInspector] public HashSet<Unit> SpliteIgnoreUnits = new HashSet<Unit>();
    [HideInInspector] public bool IsSplit = false;      //是否是分裂出的
    [HideInInspector] public int PassTimes = 0;         //可穿透次数

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }
    
    public void Shoot(float angle, float speed_rate = 1.0f)
    {
        SpeedRate = speed_rate;

        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, this.Speed * speed_rate, angle);
        m_Rigidbody.AddForce(force);

        transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg);
    }

    void Dispose()
    {
        SpliteIgnoreUnits.Clear();
        IsSplit = false;

        GameFacade.Instance.PoolManager.RecycleBullet(this);
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        //子弹撞墙 则销毁
        if (collider.gameObject.tag == "Wall")
        {
            SpliteIgnoreUnits.Clear();

            Dispose();
            return;
        }


        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        if (unit.Side == Caster.Side) return;
        if (SpliteIgnoreUnits.Contains(unit)) return;

        Field.Instance.Hit(this, unit);

        PassTimes--;

        if (PassTimes < 0) Dispose();
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
