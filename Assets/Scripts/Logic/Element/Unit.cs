using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
[System.Serializable]
public class ATT
{
    public int HPMAX   = 3;
    [HideInInspector] public int HP = 3;
    [HideInInspector] public int ATK  = 1;
    [Header("攻速/ms")] public int ASP;    //攻速 

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

    protected float m_Angle;      //角度


    public virtual bool IsDead()
    {
        return ATT.HP <= 0;
    }

    public virtual float GetAngle()
    {
        return m_Angle;
    }
    
    protected virtual void Update()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        float deltaTime = Time.deltaTime;

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
    }

    #region 表现处理
    public virtual void HitAnim()
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
        bullet.Caster = this;

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

    #endregion


}
