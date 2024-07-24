using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弓
public class Player_10000 : Player
{
    //发射子弹
    protected override void Attack()
    {
        //判断技能是否控制发射
        foreach (var sk in m_Skills) {
            if (sk.OnShoot() == true) return;
        }

        //默认攻击
        var bullet = CreateBullet();
        bullet.Shoot(this.GetAngle() + 180);
    }
}
