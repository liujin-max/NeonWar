using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


//基础属性
[System.Serializable]
public class EnemyATT : ATT
{
    
}




public class Enemy : Unit
{
    private Rigidbody2D m_Rigidbody;
    private SpriteRenderer m_Sprite;
    private CircleHP m_HPBar;


    private MonsterJSON m_Data;
    public _C.ENEMY_TYPE TYPE {get {return m_Data.Type;}}
    public int Glass { get {return m_Data.Glass;}}

    private Vector2 m_LastVelocity;
    private float m_LastAngularVelocity;
    private bool m_InHitAnim = false;




    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        m_Sprite    = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    public virtual void Init(MonsterJSON monster_data)
    {
        m_Data  = monster_data;
        Side    = _C.SIDE.ENEMY;
        ID      = monster_data.ID;

        ATT.HPMAX  = monster_data.HP;
        ATT.HP  = ATT.HPMAX;

        ASP.Reset(ATT.ASP.ToNumber() / 1000.0f);


        InitHPBar();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * ATT.SPEED.ToNumber() / 100.0f;
    }


    #region 表现处理

    void InitHPBar()
    {
        m_HPBar = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Enemy/CircleHP", Vector2.zero, transform).GetComponent<CircleHP>();
        m_HPBar.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        m_HPBar.Init(this);

    }


    public override void HitAnim()
    {
        if (m_InHitAnim) return;

        m_InHitAnim = true;
        m_Sprite.transform.DOPunchScale(new Vector3(0.15f, 0.2f, 0.15f), 0.15f).OnComplete(()=>{ m_InHitAnim = false;});
    }

    public override void Dead(Bullet bullet)
    {
        //掉落Buff逻辑
        if (m_Data.Buffs.Length > 0)
        {
            int rand    = RandomUtility.Random(0, m_Data.Buffs.Length);
            int buff_id = m_Data.Buffs[rand];
            Field.Instance.PushBuffBubble(buff_id, 1);
        }

        Field.Instance.Land.DoSmallShake();

        GameFacade.Instance.EffectManager.Load(EFFECT.BROKEN, transform.localPosition, Field.Instance.Land.ELEMENT_ROOT.gameObject).transform.right = bullet.Velocity;

    }
    #endregion

    #region 逻辑处理
    public void SetValid(bool flag)
    {
        m_ValidFlag = flag;
    }

    public override void UpdateHP(int value)
    {
        base.UpdateHP(value);

        m_HPBar.FlushHP();
    }

    //strength :力的强度，意味着移动速度
    public void Push()
    {
        float angle = RandomUtility.Random(0, 360);

        m_Rigidbody.velocity = ToolUtility.FindPointOnCircle(Vector2.zero, ATT.SPEED.ToNumber() / 100.0f, angle);
    }

    //暂停运动
    public override void Stop()
    {
        if (m_Rigidbody.isKinematic == true) return;

        m_LastVelocity          = m_Rigidbody.velocity;
        m_LastAngularVelocity   = m_Rigidbody.angularVelocity;

        m_Rigidbody.velocity    = Vector3.zero;
        m_Rigidbody.angularVelocity = 0;
        m_Rigidbody.isKinematic = true;
    }

    //恢复运动
    public override void Resume()
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
    #endregion
}
