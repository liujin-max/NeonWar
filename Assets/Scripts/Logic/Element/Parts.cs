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

    public override bool IsDead()
    {
        return m_Caster.IsDead();
    }

    public override float GetAngle()
    {
        return m_Caster.GetAngle();
    }


    #region 逻辑处理
    //同步最新的加成等级
    // public override void Sync()
    // {
    //     ATT.ATK = GameFacade.Instance.DataCenter.User.CurrentPlayer.ATK * _C.UPGRADE_ATK;

    //     //每级提高攻速百分比
    //     ASP.Reset((ATT.ASP / 1000.0f) / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.CurrentPlayer.ASP - 1)));
    // }


    #endregion
}
