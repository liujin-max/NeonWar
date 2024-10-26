using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

// 沙蛇
// 发射沙尘暴，沙尘暴有持续时间，撞击墙壁会反弹
// 被沙尘暴击中会扣血、并且失去一定的视线范围
public class Enemy_204 : Enemy
{
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        Event_BulletHit.OnEvent += OnBulletHit;
    }

    public override void Dispose()
    {
        base.Dispose();

        Event_BulletHit.OnEvent -= OnBulletHit;
    }

    public override void DoAttack()
    {
        Roar();
        
        float angle = ToolUtility.VectorToAngle(Field.Instance.Player.transform.localPosition - transform.localPosition);

        //作为Boss和普通怪物时 逻辑不同
        if (this.TYPE == ENEMY_TYPE.BOSS)
        {
            //向玩家发射子弹
            {
                var bullet = Field.Instance.CreateBullet(this);
                bullet.Shoot(angle + 15);
                bullet.ReboundTimes = 999;
                bullet.InitLiftTime(12);
            }

            {
                var bullet = Field.Instance.CreateBullet(this);
                bullet.Shoot(angle - 15);
                bullet.ReboundTimes = 999;
                bullet.InitLiftTime(12);
            }
        }
        else
        {
            var bullet = Field.Instance.CreateBullet(this);
            bullet.Shoot(angle);
            bullet.ReboundTimes = 999;
            bullet.InitLiftTime(8);
        }
        
    }


    #region 监听事件
    private void OnBulletHit(Event_BulletHit e)
    {
        var caster = e.Bullet.Caster;
        if (caster != this) return;

        e.Target.AddBuff(caster, (int)BUFF.FOV, 0, 6);
    }
    #endregion
}
