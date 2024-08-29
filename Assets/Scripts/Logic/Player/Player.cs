using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Unit
{
    protected Animator m_Animator;

    public CDTimer InvincibleTimer = new CDTimer(1f);   //无敌时间
    public int MoveDirection = 1;

    protected List<Skill> m_Skills = new List<Skill>();
    protected List<Pear> m_Pears = new List<Pear>();


    //碎片奖励
    [HideInInspector] public AttributeValue GlassRate = new AttributeValue(1, false);


    void Awake()
    {
        m_Animator = transform.Find("Entity").GetComponent<Animator>();
    }

    public void Init(int id, float angle)
    {
        ID      = id;
        Side    = _C.SIDE.PLAYER;
        m_Angle = angle;

        //修改到源文件
        ATT     = Instantiate(ATT);
        ATT.HP  = ATT.HPMAX;


        InvincibleTimer.Full();

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    //无敌
    public override bool IsInvincible()
    {
        return !InvincibleTimer.IsFinished();
    }

    // Update is called once per frame
    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;

        foreach (var skill in m_Skills) {
            skill.CustomUpdate(deltaTime);
        }

        InvincibleTimer.Update(deltaTime);

        return true;
    }


    void LateUpdate()
    {
        //始终朝向圆心
        transform.right = Vector3.zero - transform.localPosition;
    }

    public override void Dispose()
    {
        base.Dispose();
        
        m_Skills.ForEach(s => s.Dispose());
        m_Skills.Clear();

        m_Pears.ForEach(p => p.UnEquip());
        m_Pears.Clear();

        Destroy(gameObject);
    }


    #region 表现处理

    //碰撞造成伤害时， hit为空 
    public override void Affected(Hit hit = default)
    {
        InvincibleTimer.ForceReset();

        Field.Instance.Land.DoShake();
        GameFacade.Instance.EffectManager.Load(EFFECT.CRASH, Vector3.zero, Field.Instance.Land.ELEMENT_ROOT.gameObject);
    }

    public override void Dead(Hit hit = default)
    {
        Field.Instance.Land.DoShake();
    }
    
    //攻击动画，在Animation clip中插入帧事件来执行真正的攻击逻辑
    protected override void Attack()
    {
        //播放速度随攻速的变化而变化
        m_Animator.speed  = CPS.ToNumber();
        m_Animator.Play("Attack", 0, 0);
    }
    #endregion
    

    #region 逻辑处理
    //angle : 0 -> 360
    public void Move(float direction)
    {
        float speed = ATT.SPEED.ToNumber() / 100.0f;

        float t = Mathf.Clamp01(Time.deltaTime / 0.01f);
        m_Angle = Mathf.LerpAngle(m_Angle, m_Angle + (direction * MoveDirection) * speed, t);

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, m_Angle));
    }

    //对角线闪现
    public void Blink()
    {
        m_Angle += 180;

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, m_Angle));
    }

    //同步最新的加成等级
    public void Sync()
    {
        int atk_level = DataCenter.Instance.User.CurrentPlayer.ATK;
        int asp_level = DataCenter.Instance.User.CurrentPlayer.ASP;
        int wor_level = DataCenter.Instance.User.CurrentPlayer.WORTH;


        ATT.ATK.SetBase(NumericalManager.FML_ATK(atk_level));
        ASP.Reset(ATT.ASP.ToNumber() / 1000.0f);
        CPS.SetBase(NumericalManager.FML_ASP(ATT.ASP.GetBase(), asp_level));

        GlassRate.PutAUL(this, NumericalManager.FML_WORTH(wor_level) / 100.0f);



        //同步技能
        {
            m_Skills.ForEach(skill => skill.Dispose());
            m_Skills.Clear();
            DataCenter.Instance.User.CurrentPlayer.SkillSlots.ForEach(skill_msg => {
                if (skill_msg.ID > 0) {
                    Skill sk = Skill.Create(DataCenter.Instance.League.GetSkillData(skill_msg.ID), this, skill_msg.Level);
                    m_Skills.Add(sk);
                }
            });

            //测试技能
            // m_Skills.Add(Skill.Create(DataCenter.Instance.League.GetSkillData(10230), this, 3));
        }

        //同步宝珠
        {
            m_Pears.ForEach(pear => pear.UnEquip());
            m_Pears.Clear();
            DataCenter.Instance.User.CurrentPlayer.PearSlots.ForEach(pear_msg => {
                if (pear_msg.ID > 0)
                {
                    Pear pear = Pear.Create(pear_msg.ID);
                    pear.Equip(this);
                    m_Pears.Add(pear);
                }
            });

            //测试宝珠
            // Pear pear = Pear.Create(DataCenter.Instance.Backpack.GetPearData(20052));
            // pear.Equip(this);
            // m_Pears.Add(pear);
        }
        
    }

    #endregion


    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == _C.COLLIDER_ENEMY)
        {
            Field.Instance.Crash(collider.gameObject.GetComponent<Enemy>(), this);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {

    }

    void OnTriggerExit2D(Collider2D collider)
    {

    }
    #endregion
}
