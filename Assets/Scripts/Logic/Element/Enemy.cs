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


    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    //strength :力的强度，意味着移动速度
    public void Push(float strength)
    {
        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, strength, RandomUtility.Random(0, 360));
        m_Rigidbody.AddForce(force);
    }

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
