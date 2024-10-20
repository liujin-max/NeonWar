using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//死亡时分裂成3只兔子
public class Enemy_203 : Enemy
{
    public override void Dead(Hit hit = default)
    {
        float fly_time = 0.5f;

        //白兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 250, HP = Mathf.CeilToInt(ATT.HPMAX * 0.3f)}, transform.localPosition, (e)=>{
            e.SetValid(false);
            e.Stop();
            var projectile = e.transform.AddComponent<Projectile>();
            projectile.Init(TRACE.PARABOLA, this, ()=>{
                e.SetValid(true);
                e.Resume();
            });
            projectile.GO(ToolUtility.FindPointOnCircle(Vector3.zero, 4f, RandomUtility.Random(0, 360)), fly_time);
            projectile.AutoDestroy = false;
        });


        //棕兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 251, HP = Mathf.CeilToInt(ATT.HPMAX * 0.5f)}, transform.localPosition, (e)=>{
            e.SetValid(false);
            e.Stop();
            var projectile = e.transform.AddComponent<Projectile>();
            projectile.Init(TRACE.PARABOLA, this, ()=>{
                e.SetValid(true);
                e.Resume();
            });
            projectile.GO(ToolUtility.FindPointOnCircle(Vector3.zero, 4f, RandomUtility.Random(0, 360)), fly_time);
            projectile.AutoDestroy = false;
        });

        //绿兔
        Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 252, HP = Mathf.CeilToInt(ATT.HPMAX * 0.8f)}, transform.localPosition,(e)=>{
            e.SetValid(false);
            e.Stop();
            var projectile = e.transform.AddComponent<Projectile>();
            projectile.Init(TRACE.PARABOLA, this, ()=>{
                e.SetValid(true);
                e.Resume();
            });
            projectile.GO(ToolUtility.FindPointOnCircle(Vector3.zero, 4f, RandomUtility.Random(0, 360)), fly_time);
            projectile.AutoDestroy = false;
        });
    }
}
