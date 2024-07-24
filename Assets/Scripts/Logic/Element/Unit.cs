using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
[System.Serializable]
public class ATT
{
    [HideInInspector] public int HPMAX   = 3;
    [HideInInspector] public int HP = 3;
    [HideInInspector] public int ATK  = 1;
    [Header("攻速/ms")] public int ASP;    //攻速 
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
    }

    #region 逻辑处理
    public virtual void UpdateHP(int value)
    {
        ATT.HP += value;
    }

    // //同步最新的加成等级
    // public virtual void Sync()
    // {
    //     ATT.ATK = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK * _C.UPGRADE_ATK;

    //     //每级提高攻速百分比
    //     ASP.Reset((ATT.ASP / 1000.0f) / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP - 1)));
    // }

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

    #endregion


}
