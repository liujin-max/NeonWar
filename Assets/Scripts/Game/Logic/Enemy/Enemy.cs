using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;



public class Enemy : Unit
{
    protected Rigidbody2D m_Rigidbody;
    private Transform m_HPPivot;
    private CircleHP m_HPBar;
    protected BlackHole m_BlackHole;
    

    [SerializeField] private ParticleSystem.MinMaxGradient m_Color;


    private MonsterJSON m_Data;
    public CONST.ENEMY_TYPE TYPE {get {return m_Data.Type;}}

    [HideInInspector] public bool IsSummon = false;
    private bool m_IsRepel = false;   //击退中
    private CDTimer m_RepelTimer = new CDTimer(0.8f);




    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        m_Sprite    = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        m_HPPivot   = transform.Find("HPPivot");

        ShootPivot  = transform.Find("Shoot");
        HeadPivot   = transform.Find("Head");
    }

    void Start()
    {
        // 确保物体不受重力影响
        m_Rigidbody.gravityScale = 0;
    }

    public virtual void Init(MonsterJSON monster_data)
    {
        m_Data  = monster_data;
        Side    = CONST.SIDE.ENEMY;
        ID      = monster_data.ID;

        //不希望ScriptableObject的对象在相同的怪物之间共享,且避免修改到源文件
        //所以怪物在生成时需要重新实例化新的ATTConfig
        ATT     = Instantiate(ATT);
        ATT.HPMAX  = monster_data.HP;
        ATT.HP  = ATT.HPMAX;

        ASP.Reset(ATT.ASP.ToNumber() / 1000.0f);

        InitHPBar();
    }

    public override bool IsBoss()
    {
        return TYPE == CONST.ENEMY_TYPE.BOSS;
    }

    public override bool IsControlled()
    {
        return base.IsControlled() || m_IsRepel;
    }


    public override void Dispose()
    {
        base.Dispose();

        if (m_HPBar != null) {
            Destroy(m_HPBar.gameObject);
            m_HPBar = null;
        }
        
        Destroy(gameObject);
    }

    public override bool CustomUpdate(float deltaTime)
    {
        //击退
        if (m_IsRepel) {
            m_RepelTimer.Update(deltaTime);

            // 逐渐恢复速度
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, ToolUtility.AngleToVector(m_Angle) * ATT.SPEED.ToNumber() / 100.0f, m_RepelTimer.Progress);

            if (m_RepelTimer.IsFinished() == true) {
                m_IsRepel = false;
            }
        }
        


        return base.CustomUpdate(deltaTime);
    }

    #region 表现处理
    public void SetSortingOrder(int value)
    {
        m_Sprite.sortingOrder = value;
    }

    public void InitBlackHole(BlackHole hole)
    {
        m_BlackHole = hole;
    }

    void InitHPBar()
    {
        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Element/CircleHP", Vector3.zero, m_HPPivot, (obj)=>{
            m_HPBar = obj.GetComponent<CircleHP>();
            m_HPBar.Init(this);
        });
    }


    public override void Affected(Hit hit = default)
    {
        SoundManager.Instance.Load(SOUND.HIT);

        AffectedEffect.DoHit(hit);

        if (m_HPBar != null) m_HPBar.Affected();

    }

    public override void Dead(Hit hit = default)
    {
        SoundManager.Instance.Load(SOUND.DEAD);

        Field.Instance.OnBuffFall(hit, this);

        var e = GameFacade.Instance.EffectManager.Load(EFFECT.BROKEN, transform.localPosition, Field.Instance.Land.ELEMENT_ROOT.gameObject).GetComponent<PixelBroken>();
        e.transform.right = hit.Velocity;
        e.Init(m_Color);

        Field.Instance.Land.DoSmallShake();

        EventManager.SendEvent(new GameEvent(EVENT.ONKILLENEMY, this, hit));
    }

    //怪物通常没有攻击动画，所以直接执行DoAttack
    //有攻击动画的怪物需要重写此方法
    protected override void Attack()
    {
        DoAttack();
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

        if (m_HPBar != null) m_HPBar.FlushHP();
    }

    public virtual void Appear()
    {
        m_BlackHole?.Dispose();
        m_BlackHole = null;
    }

    //strength :力的强度，意味着移动速度
    public void Push(float angle)
    {
        m_Angle = angle;

        m_Rigidbody.velocity = ToolUtility.FindPointOnCircle(Vector2.zero, ATT.SPEED.ToNumber() / 100.0f, m_Angle);
    }

    //击退
    public void Repel(Vector2 force)
    {
        //免疫击退
        if (ATT.SPEED.GetBase() == 0) return;

        m_IsRepel = true;
        m_RepelTimer.Reset();

        m_Rigidbody.AddForce(force, ForceMode2D.Impulse);    
    }

    public override void SyncSpeed()
    {
        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * ATT.SPEED.ToNumber() / 100.0f;
    }


    //暂停运动
    public override void Stop()
    {
        ATT.SPEED.PutMUL(this, 0);
        SyncSpeed();
    }

    //恢复运动
    public override void Resume()
    {
        ATT.SPEED.Pop(this);
        m_Rigidbody.velocity = ToolUtility.FindPointOnCircle(Vector2.zero, ATT.SPEED.ToNumber() / 100.0f, m_Angle);
    }

    #endregion

    #region 碰撞检测
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != CONST.COLLIDER_WALL) return;

        m_Angle = ToolUtility.VectorToAngle(m_Rigidbody.velocity);
    }
    #endregion
}
