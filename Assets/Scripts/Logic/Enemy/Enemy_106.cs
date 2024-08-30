using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_106 : Enemy
{
    public override void DoAttack()
    {
        float base_angle = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);

        Field.Instance.CreateBullet(this).Shoot(base_angle);
        Field.Instance.CreateBullet(this).Shoot(base_angle - 45);
        Field.Instance.CreateBullet(this).Shoot(base_angle + 45);
    }
}
