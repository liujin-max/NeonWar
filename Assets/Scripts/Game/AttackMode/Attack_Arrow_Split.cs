using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//弓：分裂箭矢
public class Attack_Arrow_Split : AttackMode
{
    public Attack_Arrow_Split(Player player, int attackCount) : base(player, attackCount)
    {
    }

    public override void Execute()
    {
        var bullet = Field.Instance.CreateBullet(Belong);
        bullet.Shoot(Belong.GetAngle() + 180);
        bullet.SplitCount = Count;
    }
}
