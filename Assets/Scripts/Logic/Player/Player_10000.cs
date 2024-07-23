using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弓
public class Player_10000 : Player
{
    [HideInInspector] public int BulletCount = 1;

    private int BulletSpeed = 500;

    //发射子弹
    protected override void Shoot()
    {
        BulletCount = 4;

        switch (BulletCount)
        {
            case 1:
            {
                var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                bullet.transform.position = ShootPivot.position;
                bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
            }
            break;

            case 2:
            {
                var dir = ToolUtility.AngleToVector(m_Angle);
                Vector2 shoot_point = ShootPivot.position;

                var offset1 = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                var offset2 = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset1;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
                }

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset2;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
                }
            }
            break;

            case 3:
            {
                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = ShootPivot.position;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180 - 15));
                }

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = ShootPivot.position;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
                }

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = ShootPivot.position;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180 + 15));
                }
            }
            break;

            case 4:
            {
                var dir = ToolUtility.AngleToVector(m_Angle);
                Vector2 shoot_point = ShootPivot.position;

                var offset1 = shoot_point + new Vector2(-dir.y, dir.x) * 0.15f;
                var offset2 = shoot_point + new Vector2( dir.y, -dir.x) * 0.15f;

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset1;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
                }

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset2;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180));
                }


                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset1;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180 - 20));
                }

                {
                    var bullet = GameFacade.Instance.PoolManager.AllocateBullet(BulletTemplate, Vector3.zero);
                    bullet.transform.position = offset2;
                    bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, BulletSpeed, this.GetAngle() + 180 + 20));
                }
            }
            break;
        }
    }
}
