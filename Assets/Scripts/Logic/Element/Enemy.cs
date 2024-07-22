using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//基础属性
[System.Serializable]
public class EnemyATT : ATT
{
    public int CrashATK = 1;    //撞击伤害
    public int Speed;    //移动速度 
}

public class Enemy : Unit
{
    private Rigidbody2D m_Rigidbody;
    private TextMeshPro m_HPText;


    public int CrashATK = 1;    //撞击伤害
    public int Speed;    //移动速度 

    [HideInInspector] public int Glass;

    private Vector2 m_LastVelocity;
    private float m_LastAngularVelocity;


    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_HPText    = transform.Find("HP").GetComponent<TextMeshPro>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    public void Init(int id, int hp)
    {
        Side    = _C.SIDE.ENEMY;
        ID      = id;
        ATT.HP  = hp;
        Glass   = Mathf.Max(NumericalManager.FML_HP2Glass(hp), 1);

        FlushHP();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    void FlushHP()
    {
       m_HPText.text = ATT.HP.ToString(); 
    }

    void LateUpdate()
    {
        m_HPText.transform.eulerAngles = Vector3.zero;
    }


    #region 逻辑处理
    public override void UpdateHP(int value)
    {
        base.UpdateHP(value);

        FlushHP();
    }

    //strength :力的强度，意味着移动速度
    public void Push()
    {
        Vector2 force = ToolUtility.FindPointOnCircle(Vector2.zero, Speed, RandomUtility.Random(0, 360));
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
