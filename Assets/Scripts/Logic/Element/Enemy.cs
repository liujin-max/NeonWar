using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
public class EnemyATT
{
    public int HP = 1;
    public int ATK = 1;

}

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;


    public EnemyATT ATT = new EnemyATT();

    private Vector2 m_LastVelocity;
    private float m_LastAngularVelocity;

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return ATT.HP <= 0;
    }


    #region 逻辑处理
    public void UpdateHP(int value)
    {
        ATT.HP += value;
    }

    //strength :力的强度，意味着移动速度
    public void Push(float strength)
    {
        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, strength, RandomUtility.Random(0, 360));
        m_Rigidbody.AddForce(force);
    }

    //暂停运动
    public void Pause()
    {
        if (m_Rigidbody.isKinematic == true) return;

        m_LastVelocity          = m_Rigidbody.velocity;
        m_LastAngularVelocity   = m_Rigidbody.angularVelocity;

        m_Rigidbody.velocity    = Vector3.zero;
        m_Rigidbody.angularVelocity = 0;
        m_Rigidbody.isKinematic = true;
    }

    //恢复运动
    public void Resume()
    {
        if (!m_Rigidbody.isKinematic) return;

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity    = m_LastVelocity;
        m_Rigidbody.angularVelocity = m_LastAngularVelocity;
    }

    #endregion

    #region 碰撞检测
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Enemy OnCollisionEnter2D : " + collision.gameObject.name);

        if (collision.gameObject.tag == "Player")
        {
            Field.Instance.Crash(this, collision.gameObject.GetComponent<Player>());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Debug.Log("Enemy OnCollisionStay2D : " + collision.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log("Enemy OnCollisionExit2D : " + collision.gameObject.name);
    }
    #endregion
}
