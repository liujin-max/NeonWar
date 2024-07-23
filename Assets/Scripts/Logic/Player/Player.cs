using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Unit
{
    public CDTimer InvincibleTimer = new CDTimer(1f);   //无敌时间

    private List<Skill> m_Skills = new List<Skill>();



    public void Init(float angle)
    {
        Side    = _C.SIDE.PLAYER;
        m_Angle = angle;

        InvincibleTimer.Full();

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));
    }


    //angle : 0 -> 360
    public void Move(float direction)
    {
        float speed = 2;

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
    protected override void Update()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        float deltaTime = Time.deltaTime;

        InvincibleTimer.Update(deltaTime);

        base.Update();
    }


    void LateUpdate()
    {
        //始终朝向圆心
        transform.localEulerAngles = new Vector3(0, 0, m_Angle + 90);
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    

    #region 逻辑处理
    //同步最新的加成等级
    public void Sync()
    {
        ATT.ATK = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK * _C.UPGRADE_ATK;

        //每级提高攻速百分比
        ASP.Reset((ATT.ASP / 1000.0f) / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP - 1)));

        //同步技能
        m_Skills.Clear();
        GameFacade.Instance.DataCenter.User.CurrentPlayer.Skills.ForEach(skill_msg => {
            Skill sk = new Skill(GameFacade.Instance.DataCenter.League.GetSkillData(skill_msg.ID), this, skill_msg.Level);
            sk.Init();
            m_Skills.Add(sk);
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
