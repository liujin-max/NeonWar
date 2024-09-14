using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//弓 普通攻击|多重射击
public class Attack_Arrow_Normal : AttackMode
{
    public Attack_Arrow_Normal(Player player, int attackCount) : base(player, attackCount) {}

    public override void Execute()
    {
        SoundManager.Instance.Load(SOUND.BOW);


        switch (Count)
        {
            case 1:
            {
                var bullet = Field.Instance.CreateBullet(Belong);
                bullet.Shoot(Belong.GetAngle() + 180);
            }
            break;

            case 2:
            {
                var dir = ToolUtility.AngleToVector(Belong.GetAngle());
                Vector2 shoot_point = Belong.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }
            }
            break;

            case 3:
            {
                var dir = ToolUtility.AngleToVector(Belong.GetAngle());
                Vector2 shoot_point = Belong.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180 - 15);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180 + 15);
                }
            }
            break;

            case 4:
            {
                var dir = ToolUtility.AngleToVector(Belong.GetAngle());
                Vector2 shoot_point = Belong.ShootPivot.position;
                
                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180 - 15);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.25f;
                    bullet.Shoot(Belong.GetAngle() + 180 + 15);
                }
            }
            break;

            case 5:
            {
                var dir = ToolUtility.AngleToVector(Belong.GetAngle());
                Vector2 shoot_point = Belong.ShootPivot.position;

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180);
                }


                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2(-dir.y, dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180 - 20);
                }

                {
                    var bullet = Field.Instance.CreateBullet(Belong);
                    bullet.transform.position = shoot_point + new Vector2( dir.y, -dir.x) * 0.35f;
                    bullet.Shoot(Belong.GetAngle() + 180 + 20);
                }
            }
            break;
        }
    }
}
