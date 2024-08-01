using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
[System.Serializable]
public class ATT
{
    public int HPMAX   = 3;
    [HideInInspector] public int HP = 3;
    [HideInInspector] public AttributeValue ATK  = new AttributeValue(1);
    [Header("攻速(毫秒)")] public AttributeValue ASP = new AttributeValue(1000);    //攻速 
    [Header("暴击率(千分制)")] public AttributeValue CP = new AttributeValue(0);
    [Header("暴击伤害(千分制)")] public AttributeValue CT = new AttributeValue(1500);
    [Header("闪避率(千分制)")] public AttributeValue DODGE = new AttributeValue(0);
    [Header("移动速度")] public AttributeValue SPEED = new AttributeValue(0);
    
    //易伤倍率
    [HideInInspector] public AttributeValue VUN_INC   = new AttributeValue(1f, false);
}

//基础单位
public class Unit : MonoBehaviour
{
    public Transform ShootPivot;
    public GameObject BulletTemplate;

    [HideInInspector] public int ID;
    [HideInInspector] public _C.SIDE Side = _C.SIDE.PLAYER;
    public ATT ATT = new ATT();
    public CDTimer ASP = new CDTimer(0f);

    //Buff
    private Dictionary<int, Buff> m_BuffDic = new Dictionary<int, Buff>();

    //各种状态
    

    protected float m_Angle;      //角度
    protected bool m_ValidFlag = true;
    public bool IsValid {
        get {
            if (IsDead()) return false;

            return m_ValidFlag;
        } 
    }


    public virtual bool IsDead()
    {
        return ATT.HP <= 0;
    }

    public virtual float GetAngle()
    {
        return m_Angle;
    }
    
    public virtual bool CustomUpdate(float deltaTime)
    {
        if (!IsValid) return false;

        //攻击间隔
        if (ASP.Duration > 0)
        {
            ASP.Update(deltaTime);
            if (ASP.IsFinished() == true) {
                ASP.Reset();

                Attack();
            }
        }


        List<Buff> remove_buffs = new List<Buff>();
        //刷新Buff
        foreach (var item in m_BuffDic)
        {
            Buff buff = item.Value;
            buff.Update(deltaTime);

            if (buff.IsEnd()) remove_buffs.Add(buff);
        }

        remove_buffs.ForEach(buff => this.RemoveBuff(buff));

        return true;
    }

    #region 表现处理
    public virtual void HitAnim()
    {

    }

    public virtual void Dead(Bullet bullet)
    {

    }
    #endregion


    #region 逻辑处理
    //无法移动
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

    public virtual Bullet CreateBullet()
    {
        var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
        bullet.transform.position = ShootPivot.position;
        bullet.Init(this);

        EventManager.SendEvent(new GameEvent(EVENT.ONBULLETCREATE, bullet));

        return bullet;
    }

    protected virtual void Attack()
    {

    }

    public Buff AddBuff(int buff_id, int value)
    {
        Buff b;

        //已经有对应Buff了
        if (m_BuffDic.ContainsKey(buff_id)) 
        {
            b = m_BuffDic[buff_id];
            b.Value = value;
            b.Flush();  //刷新CD
        }
        else
        {
            b = Buff.Create(buff_id, value, this);
            b.Init();

            m_BuffDic[buff_id] = b;
        }

        return b;
    }

    public void RemoveBuff(Buff buff)
    {
        buff.Dispose();
        
        if (m_BuffDic.ContainsKey(buff.ID)) {
            m_BuffDic.Remove(buff.ID);
        }
    }

    public Buff GetBuff(int buff_id)
    {
        Buff buff = null;
        if (m_BuffDic.TryGetValue(buff_id, out buff)) {
            return buff;
        }
        return null;
    }

    #endregion


}
