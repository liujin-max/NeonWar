using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_105 : Enemy
{
    protected override void Shoot()
    {

        for (int i = 0; i < 6; i++)
        {
            float angle = 60.0f * i;

            var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
            bullet.transform.position = ShootPivot.position;
            bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, 200, angle));
        }
    }
}
