using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Collections.Generic.Dictionary<int, Buff>;


//基础单位
public class Unit : MonoBehaviour
{
    public GameObject BulletTemplate;

    protected SpriteRenderer m_Sprite;
    [HideInInspector] public Transform ShootPivot;
    [HideInInspector] public Transform HeadPivot;
    

    [HideInInspector] public int ID;
    [HideInInspector] public CONST.SIDE Side = CONST.SIDE.PLAYER;
    public ATTConfig ATT;
    public CDTimer ASP = new CDTimer(0f);
    [HideInInspector] public AttributeValue CPS = new AttributeValue(1f, false);  //冷却值的恢复倍率

    //易伤倍率
    [HideInInspector] public AttributeValue VUN_INC     = new AttributeValue(1f, false);
    //伤害倍率
    [HideInInspector] public AttributeValue ATK_INC     = new AttributeValue(1f, false);
    //对头目伤害加成
    [HideInInspector] public AttributeValue BOSS_INC    = new AttributeValue(1f, false);
    //对健康敌人额外伤害倍率
    [HideInInspector] public AttributeValue HEALTH_INC  = new AttributeValue(1f, false);
    //对减速敌人额外伤害倍率
    [HideInInspector] public AttributeValue SLOW_INC    = new AttributeValue(1f, false);
    //对受控制敌人额外伤害倍率
    [HideInInspector] public AttributeValue CONTROL_INC = new AttributeValue(1f, false);





    //Buff
    private Dictionary<int, Buff> m_BuffDic = new Dictionary<int, Buff>();
    public ValueCollection Buffs {get {return m_BuffDic.Values;}}
    private List<Buff> m_RemoveBuffs = new List<Buff>();
    
    
    private Affected m_Affected;
    public Affected AffectedEffect {
        get {
            if (m_Affected == null) m_Affected = m_Sprite.GetComponent<Affected>(); 
            return m_Affected;
        }
    }

    //各种状态
    [HideInInspector] public int StunReference;    //晕眩
    [HideInInspector] public int FrozenReference;    //冰冻
    [HideInInspector] public bool ImmuneDisplaceFlag;   //免疫位移
    [HideInInspector] public bool ImmuneControlFlag;    //免疫控制

    protected float m_Angle;      //角度
    protected bool m_DeadFlag = false;
    protected bool m_ValidFlag = true;
    public bool IsValid {
        get {
            if (IsDead()) return false;
            return m_ValidFlag;
        } 
    }


    #region 状态判断
    public bool IsHPFull()
    {
        return ATT.HP >= ATT.HPMAX;
    }

    public virtual bool IsDead()
    {
        return ATT.HP <= 0 || m_DeadFlag;
    }

    public virtual float GetAngle()
    {
        return m_Angle;
    }
    
    //无敌
    public virtual bool IsInvincible()
    {
        return false;
    }

    //是否Boss
    public virtual bool IsBoss()
    {
        return false;
    }

    //能否攻击
    public virtual bool CanAttack()
    {
        if (StunReference > 0 || FrozenReference > 0) return false;

        return true;
    }

    //是否被减速
    public bool IsSlow()
    {
        return ATT.SPEED.GetBase() > ATT.SPEED.ToNumber();
    }

    //是否被控制
    public virtual bool IsControlled()
    {
        return StunReference > 0 || FrozenReference > 0;
    }

    //是否免疫位移
    public virtual bool IsImmuneDisplace()
    {
        return ImmuneDisplaceFlag;
    }

    //是否免疫控制
    public virtual bool IsImmuneControl()
    {
        return ImmuneControlFlag;
    }

    #endregion




    public virtual bool CustomUpdate(float deltaTime)
    {
        if (!IsValid) return false;

        
        //刷新Buff
        foreach (Buff buff in m_BuffDic.Values)
        {
            buff.CustomUpdate(deltaTime);

            if (buff.IsEnd()) m_RemoveBuffs.Add(buff);
        }

        foreach (var buff in m_RemoveBuffs) {
            this.RemoveBuff(buff);
        }
        m_RemoveBuffs.Clear();

        //攻击间隔
        if (ASP.Duration > 0)
        {
            if (CanAttack() == true)
            {
                ASP.Update(deltaTime * CPS.ToNumber());
                if (ASP.IsFinished() == true) {
                    ASP.Reset();

                    Attack();
                }
            }
        } 

        return true;
    }


    #region 表现处理
    //受击
    public virtual void Affected(Hit hit = default)
    {

    }

    public virtual void Dead(Hit hit = default)
    {

    }

    //播放攻击动画，真正的攻击逻辑在动画的某一帧执行
    protected virtual void Attack()
    {

    }
    #endregion


    #region 逻辑处理
    //停止运动
    public virtual void Stop()
    {

    }

    //恢复运动
    public virtual void Resume()
    {

    }

    public virtual void UpdateHP(int value)
    {
        ATT.HP = Mathf.Clamp(ATT.HP + value, 0, ATT.HPMAX);
    }

    public void ForceDead()
    {
        m_DeadFlag = true;
    }

    public virtual Projectile CreateProjectile(string projectile ,CONST.TRACE trace_type, Vector2 to_pos, float time, Action callback)
    {
        var project = GameFacade.Instance.AssetManager.LoadPrefab(projectile, Vector3.zero, Field.Instance.Land.ELEMENT_ROOT).GetComponent<Projectile>();
        project.transform.position = ShootPivot.position;
        project.Init(trace_type, this, callback);
        project.GO(to_pos, time);

        return project;
    }

    //真正的攻击逻辑
    public virtual void DoAttack()
    {

    }

    public virtual void SyncSpeed()
    {
        
    }


    public virtual Buff AddBuff(Unit caster, int buff_id, int value, float time = 0f, int count = 1)
    {
        //免疫控制
        if (Buff.IsControl(buff_id) && IsImmuneControl())  return null;


        //已经有对应Buff了
        if (m_BuffDic.ContainsKey(buff_id)) 
        {
            Buff buff = m_BuffDic[buff_id];
            buff.Value = value;
            buff.Flush(time, count);  //刷新CD

            return buff;
        }
      

        Buff b = Buff.Create(buff_id, value, caster, this, time);
        b.Init();
        b.Count = count;

        m_BuffDic[buff_id] = b;

        EventManager.SendEvent(new GameEvent(EVENT.ONBUFFADD, b));

        return b;
    }

    public void RemoveBuff(Buff buff)
    {
        buff.Dispose();

        EventManager.SendEvent(new GameEvent(EVENT.ONBUFFREMOVE, buff));
        
        if (m_BuffDic.ContainsKey(buff.ID)) {
            m_BuffDic.Remove(buff.ID);
        }
    }

    public Buff GetBuff(int buff_id)
    {
        return m_BuffDic.TryGetValue(buff_id, out var buff) ? buff : null;
    }

    //清空Buff
    public void ClearBuffs()
    {
        foreach (var buff in m_BuffDic.Values) {
            buff.Dispose();
            EventManager.SendEvent(new GameEvent(EVENT.ONBUFFREMOVE, buff));
        }
        m_BuffDic.Clear();
    }

    #endregion



    public virtual void Dispose()
    {
        ClearBuffs();
    }
}
