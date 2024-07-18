using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;

    void Awake()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    void Start()
    {
        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, 200, RandomUtility.Random(0, 360));
        m_Rigidbody.AddForce(force);
    }

    #region 碰撞检测
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 当碰撞开始时调用
        Debug.Log("Enemy OnCollisionEnter2D : " + collision.gameObject.name);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 当碰撞持续时调用
        Debug.Log("Enemy OnCollisionStay2D : " + collision.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 当碰撞结束时调用
        Debug.Log("Enemy OnCollisionExit2D : " + collision.gameObject.name);
    }
    #endregion
}
