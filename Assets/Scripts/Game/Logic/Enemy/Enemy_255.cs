using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//沙虫躯干
//无法获得Buff
//死亡时转化成小沙虫
public class Enemy_255 : Enemy
{
    private Vector3 m_LastPosition = Vector3.zero;
    
    public override void Init(MonsterJSON monster_data)
    {
        base.Init(monster_data);

        ImmuneDisplaceFlag  = true;
        ImmuneControlFlag   = true;

        EventManager.AddHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    public override void Dispose()
    {
        base.Dispose();

        EventManager.DelHandler(EVENT.ONKILLENEMY,  OnKillEnemy);
    }

    void FixedUpdate()
    {
        m_Sprite.transform.localEulerAngles = new Vector3(0 , 0, ToolUtility.VectorToAngle(transform.localPosition - m_LastPosition) + 90);
        m_LastPosition = transform.localPosition;
    }

    #region 监听事件
    private void OnKillEnemy(GameEvent @event)
    {
        Enemy e = (Enemy) @event.GetParam(0);
        if (e != this) return;

        for (int i = 0; i < 6; i++)
        {
            Field.Instance.Spawn.Summon(new MonsterJSON(){ID = 257, HP = 300}, transform.localPosition, e => e.transform.localScale = new Vector3(0.6f, 0.6f, 1));
        }
    }
    #endregion
}
