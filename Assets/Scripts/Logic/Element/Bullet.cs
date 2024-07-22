using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;

    [HideInInspector] public string Name;
    [HideInInspector] public Unit Caster;

    public Vector2 Velocity {get { return m_Rigidbody.velocity;}}

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }
    
    public void Shoot(Unit caster, Vector2 force)
    {
        Caster = caster;

        m_Rigidbody.AddForce(force);

        float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    void Dispose()
    {
        GameFacade.Instance.PoolManager.RecycleBullet(this);
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        //子弹撞墙 则销毁
        if (collider.gameObject.tag == "Wall")
        {
            Dispose();
            return;
        }


        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        if (unit.Side == Caster.Side) return;


        Field.Instance.Hit(this, unit);

        Dispose();
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
