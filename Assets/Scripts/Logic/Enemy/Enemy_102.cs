using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//血量较低，且会发射子弹
public class Enemy_102 : Enemy
{
    protected override void Attack()
    {
        //向玩家发射子弹
        var bullet = CreateBullet();
        bullet.Shoot(ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition));
    }
}
