using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Parts : Unit
{
    private Player m_Caster;


    public void Init(Player player)
    {
        m_Caster = player;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Caster.IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        float deltaTime = Time.deltaTime;

        //攻击间隔
        ATT.ASP.Update(deltaTime);
        if (ATT.ASP.IsFinished() == true) {
            ATT.ASP.Reset();

            Shoot();
        }
    }


    #region 逻辑处理
    //同步最新的加成等级
    public override void Sync()
    {
        ATT.ATK = GameFacade.Instance.DataCenter.User.ATK * _C.UPGRADE_ATK;

        //每级提高攻速百分比
        ATT.ASP.Reset(_C.DEFAULT_ASP / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.ASP - 1)));
    }

    void Shoot()
    {
        var bullet = GameFacade.Instance.PoolManager.AllocateBullet(m_BulletTemplate, Vector3.zero);
        bullet.transform.position = m_ShootPivot.position;
        bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, 1000, m_Caster.Angle + 180));
    }
    #endregion
}
