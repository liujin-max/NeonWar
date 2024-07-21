using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
public class ATT
{
    public int HP   = 3;
    public int ATK  = 1;
    public CDTimer ASP   = new CDTimer(_C.DEFAULT_ASP);    //攻速 
}

//基础单位
public class Unit : MonoBehaviour
{
    [SerializeField] protected Transform m_ShootPivot;
    [SerializeField] protected GameObject m_BulletTemplate;


    public ATT ATT = new ATT();

    public bool IsDead()
    {
        return ATT.HP <= 0;
    }


    #region 逻辑处理
    //同步最新的加成等级
    public virtual void Sync()
    {
        ATT.ATK = GameFacade.Instance.DataCenter.User.ATK * _C.UPGRADE_ATK;

        //每级提高攻速百分比
        ATT.ASP.Reset(_C.DEFAULT_ASP / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.ASP - 1)));
    }
    #endregion


}
