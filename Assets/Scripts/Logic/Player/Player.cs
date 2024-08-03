using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Unit
{
    public CDTimer InvincibleTimer = new CDTimer(1f);   //无敌时间

    protected List<Skill> m_Skills = new List<Skill>();
    protected List<Pear> m_Pears = new List<Pear>();


    public void Init(float angle)
    {
        Side    = _C.SIDE.PLAYER;
        m_Angle = angle;
        ATT.HP  = ATT.HPMAX;



        InvincibleTimer.Full();

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    //无敌
    public bool IsInvincible()
    {
        return !InvincibleTimer.IsFinished();
    }

    // Update is called once per frame
    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;

        InvincibleTimer.Update(deltaTime);

        return true;
    }


    void LateUpdate()
    {
        //始终朝向圆心
        transform.right = Vector3.zero - transform.localPosition;
    }

    public void Dispose()
    {
        base.Dispose();
        
        m_Skills.ForEach(s => s.Dispose());
        m_Skills.Clear();

        m_Pears.ForEach(p => p.Dispose());
        m_Pears.Clear();

        Destroy(gameObject);
    }


    #region 表现处理
    public override void HitAnim()
    {
        Field.Instance.Land.DoShake();
        GameFacade.Instance.EffectManager.Load(EFFECT.CRASH, Vector3.zero, Field.Instance.Land.ELEMENT_ROOT.gameObject);
    }

    public override void Dead(Bullet bullet)
    {
        Field.Instance.Land.DoShake();
    }
    
    #endregion
    

    #region 逻辑处理
    //angle : 0 -> 360
    public void Move(float direction)
    {
        float speed = ATT.SPEED.ToNumber() / 100.0f;

        float t = Mathf.Clamp01(Time.deltaTime / 0.01f);
        m_Angle = Mathf.LerpAngle(m_Angle, m_Angle + direction * speed, t);

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
        int atk_level = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK;
        int asp_level = GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP;


        ATT.ATK.SetBase(NumericalManager.FML_ATK(atk_level));
        ATT.ASP.SetBase(NumericalManager.FML_ASP(ATT.ASP.GetBase(), asp_level));
        ASP.Reset(ATT.ASP.ToNumber() / 1000.0f);


        //同步技能
        m_Skills.ForEach(skill => skill.Dispose());
        m_Skills.Clear();
        GameFacade.Instance.DataCenter.User.CurrentPlayer.SkillSlots.ForEach(skill_msg => {
            if (skill_msg.ID > 0) {
                Skill sk = Skill.Create(GameFacade.Instance.DataCenter.League.GetSkillData(skill_msg.ID), this, skill_msg.Level);
                m_Skills.Add(sk);
            }
        });

        //测试技能
        // m_Skills.Add(Skill.Create(GameFacade.Instance.DataCenter.League.GetSkillData(10160), this, 3));

        //同步宝珠
        m_Pears.ForEach(pear => pear.Dispose());
        m_Pears.Clear();
        GameFacade.Instance.DataCenter.User.CurrentPlayer.PearSlots.ForEach(pear_msg => {
            if (pear_msg.ID > 0)
            {
                // Pear pear = Pear.Create(pear_msg.ID);
            }
        });
        
    }

    #endregion


    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
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
