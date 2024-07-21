using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;

    [HideInInspector] public string Name;
    [HideInInspector] public Unit Caster;

    public Vector2 Velocity {get { return m_Rigidbody.velocity;}}

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }
    
    public void Shoot(Unit caster, Vector2 force)
    {
        Caster = caster;

        m_Rigidbody.AddForce(force);
    }

    void Dispose()
    {
        // Destroy(gameObject);
        GameFacade.Instance.PoolManager.RecycleBullet(this);
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Field.Instance.Hit(this, collider.gameObject.GetComponent<Enemy>());
        }


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
