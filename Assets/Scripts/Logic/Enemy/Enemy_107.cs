using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_107 : Enemy
{
    protected override void Attack()
    {
        float base_angle = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);

        CreateBullet().Shoot(base_angle);
        CreateBullet().Shoot(base_angle - 45);
        CreateBullet().Shoot(base_angle + 45);
    }
}
