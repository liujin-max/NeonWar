using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弓
public class Player_10000 : Player
{
    public override void Init(int id, float angle)
    {
        base.Init(id, angle);

        SetAttackMode(new Attack_Arrow_Normal(this, 1));
    }

    //发射子弹
    // public override void DoAttack()
    // {
    //     SoundManager.Instance.Load(SOUND.BOW);

    //     //判断技能是否控制发射
    //     foreach (var sk in m_Skills) {
    //         if (sk.OnShoot() == true) return;
    //     }

    //     //默认攻击
    //     var bullet = Field.Instance.CreateBullet(this);
    //     bullet.Shoot(this.GetAngle() + 180);
    // }
}
