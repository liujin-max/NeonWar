using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//血量较低，且会发射子弹
public class Enemy_102 : Enemy
{
    protected override void Shoot()
    {
        //向玩家发射子弹
        Vector2 dir = Field.Instance.Player.transform.localPosition - transform.localPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var bullet = CreateBullet();
        bullet.Shoot(angle);
    }
}
